using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Turbosms.Client.Senders
{
    public class SqlSender : ISender
    {
        public string Exceptions { get; set; }

        private readonly string _login;
        private MySqlConnection _connection;
        private readonly MessageFactory _messageFactory;

        public SqlSender(string server, string database, string login, string password)
        {
            _login = login;
            _messageFactory = new MessageFactory();

            var connectionString = $"SERVER={server};DATABASE={database};UID={_login};PASSWORD={password};";

            Initialize(connectionString);
        }

        public SqlSender(string connectionString)
        {
            Initialize(connectionString);
        }

        private void Initialize(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                _connection.Open();
                MySqlCommand cmdutf8 = new MySqlCommand("SET NAMES utf8;", _connection);
                cmdutf8.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Exceptions = "Cannot connect to server.  Contact administrator";
                        break;

                    case 1045:
                        Exceptions = "Invalid username/password, please try again";
                        break;
                }

                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                _connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Exceptions = ex.Message;
                return false;
            }
        }


        public int SendMessage(string receiver, string sender, string message)
        {
            var id = -1;
            
            var sms = new Message()
            {
                number = receiver,
                message = message,
                sign = sender,
                send_time = DateTime.Now.AddMinutes(3).ToString(Message.DateTimeFormat)
            };
            
            var query =
                $"INSERT INTO {_login} (number, message, sign) VALUES('{sms.number}', '{sms.message}', '{sms.sign}'); " +
                "SELECT LAST_INSERT_ID();";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, _connection);

                try
                {
                    id = int.Parse(cmd.ExecuteScalar().ToString());
                }
                catch
                {
                }

                CloseConnection();
            }

            return id;
        }

        public List<Message> GetSMSDetail(int? smsid = null)
        {
            string query = $"SELECT * FROM {_login}";
            
            if (smsid != null)
            {
                query += $" where id = '{smsid}'";
            }

            var list = new List<Message>();

            if (OpenConnection())
            {
                var command = new MySqlCommand(query, _connection);
                var dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    list.Add(_messageFactory.Create(dataReader));
                }

                dataReader.Close();

                CloseConnection();

                return list;
            }

            return list;
        }

        public decimal GetBalance()
        {
            decimal balance = 0;

            if (OpenConnection())
            {
                var query = $@"select balance from {_login} 
                        where msg_id IS NOT NULL 
                        order by id desc
                        LIMIT 1";
                
                var command = new MySqlCommand(query, _connection);
                
                try
                {
                    var val = command.ExecuteScalar().ToString();
                    balance = decimal.Parse(val);
                }
                catch
                {
                }
            }

            return balance;
        }
    }
}