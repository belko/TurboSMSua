using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Turbosms.Client.Models
{
    public class SQLSMSSender
    {
        public string Exceptions { get; set; }
        
        private MySqlConnection _connection;
        private readonly string _login;
        private readonly string _password;
        private readonly string _database;
        private readonly string _server;

        public SQLSMSSender(string server,string database, string login, string password)
        {
            _server = server;
            _database = database;
            _login = login;
            _password = password;
            
            var connectionString = $"SERVER={_server};DATABASE={_database};UID={_login};PASSWORD={_password};";
            
            Initialize(connectionString);
        }

        public SQLSMSSender(string connectionString)
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


        public int SendMessage(string receiver,string sender, string message)
        {
            int id=-1;
            SMSModel sms = new SMSModel()
            {
                number = receiver,
                message = message,
                sign = sender,
                send_time = DateTime.Now.AddMinutes(3).ToString(SMSModel.DateTimeFormat)
            };
            string query = string.Format(
                "INSERT INTO {0} (number, message, sign) VALUES('{1}', '{2}', '{3}'); "+
                "SELECT LAST_INSERT_ID();",
                _login,sms.number, sms.message, sms.sign);

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, _connection);

                try
                {
                    id = int.Parse(cmd.ExecuteScalar().ToString());
                }
                catch {
                }
                this.CloseConnection();
            }
            return id;
        }

        public List<SMSModel> GetSMSDetail(int? smsid = null)
        {
            string query = string.Format( "SELECT * FROM {0}", _login);
            if (smsid!=null) 
            {
                query += string.Format(" where id = '{0}'", smsid);
            }

            List<SMSModel> list = new List<SMSModel>();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    SMSModel sms = new SMSModel(dataReader);
                    list.Add(sms);
                }
                dataReader.Close();

                this.CloseConnection();

                return list;
            }
            else
            {
                return list;
            }
        }

        public decimal GetBalance() 
        {
            decimal balance = 0;

            if (this.OpenConnection()) 
            {
                string query = string.Format(
                    @"select balance from {0} 
                        where msg_id IS NOT NULL 
                        order by id desc
                        LIMIT 1", _login);
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                 try{
                     var val = cmd.ExecuteScalar().ToString();
                     balance =decimal.Parse(val);
                }catch{}
            }
            return balance;
        }
    }
}
