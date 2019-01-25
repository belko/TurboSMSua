using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurboSMSua.MySQL
{
    public class SQLSMSSender
    {
        private MySqlConnection connection;
        public string exceptions;

        private string turboSMSLogin;
        private string turboSMSPassword;
        private string turboSMSDataBase;
        private string turboSMSServer;

        public SQLSMSSender(string server,string database, string login, string password)
        {
            this.turboSMSServer = server;
            this.turboSMSDataBase = database;
            this.turboSMSLogin = login;
            this.turboSMSPassword = password;
            
            string connectionString = string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};",
                turboSMSServer, turboSMSDataBase,turboSMSLogin, turboSMSPassword);
            Initialize(connectionString);
        }

        public SQLSMSSender(string connectionString)
        {

            Initialize(connectionString);
        }



        private void Initialize(string connectionString)
        {

            connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                MySqlCommand cmdutf8 = new MySqlCommand("SET NAMES utf8;", connection);
                cmdutf8.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        exceptions = "Cannot connect to server.  Contact administrator";
                        break;

                    case 1045:
                        exceptions = "Invalid username/password, please try again";
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                exceptions = ex.Message;
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
                turboSMSLogin,sms.number, sms.message, sms.sign);

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);

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
            string query = string.Format( "SELECT * FROM {0}", turboSMSLogin);
            if (smsid!=null) 
            {
                query += string.Format(" where id = '{0}'", smsid);
            }

            List<SMSModel> list = new List<SMSModel>();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
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
                        LIMIT 1", turboSMSLogin);
                MySqlCommand cmd = new MySqlCommand(query, connection);
                 try{
                     var val = cmd.ExecuteScalar().ToString();
                     balance =decimal.Parse(val);
                }catch{}
            }
            return balance;
        }
    }
}
