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
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
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
                "INSERT INTO {0} (number, message, sign, send_time) VALUES('{1}', '{2}', '{3}', '{4}'); "+
                "SELECT LAST_INSERT_ID()",
                turboSMSLogin,sms.number, sms.message, sms.sign, sms.send_time);

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

        public List<SMSModel> GetSMSDetail(string number = null)
        {
            string query = string.Format( "SELECT * FROM {0}", turboSMSLogin);
            if (!string.IsNullOrEmpty(number)) 
            {
                query += string.Format(" where number = '{0}'", number);
            }

            List<SMSModel> list = new List<SMSModel>();

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    SMSModel sms = new SMSModel(dataReader);
                    list.Add(sms);
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
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
