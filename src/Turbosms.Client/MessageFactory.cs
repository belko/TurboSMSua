using MySql.Data.MySqlClient;

namespace Turbosms.Client
{
    public class MessageFactory
    {
        public Message Create(MySqlDataReader dataReader)
        {
            var m = new Message();
            
                        
            m.id = dataReader.GetInt32("id");

            if (!string.IsNullOrEmpty(dataReader["msg_id"].ToString()))
                m.msg_id = dataReader.GetString("msg_id");

            if (!string.IsNullOrEmpty(dataReader["number"].ToString()))
                m.number = dataReader.GetString("number");
            
            if (!string.IsNullOrEmpty(dataReader["message"].ToString()))
                m.message = dataReader.GetString("message");

            if (!string.IsNullOrEmpty(dataReader["sign"].ToString()))
                m.sign = dataReader.GetString("sign");

            if (!string.IsNullOrEmpty(dataReader["cost"].ToString()))
                m.cost = dataReader.GetDecimal("cost");

            if (!string.IsNullOrEmpty(dataReader["balance"].ToString()))
                m.balance = dataReader.GetDecimal("balance");

            if (!string.IsNullOrEmpty(dataReader["added"].ToString()))
                m.added = dataReader.GetString("added");

            if (!string.IsNullOrEmpty(dataReader["sended"].ToString()))
                m.sended = dataReader.GetString("sended");

            if (!string.IsNullOrEmpty(dataReader["received"].ToString()))
                m.received = dataReader.GetString("received");

            if (!string.IsNullOrEmpty(dataReader["error_code"].ToString()))
                m.error_code = dataReader.GetString("error_code");

            if (!string.IsNullOrEmpty(dataReader["status"].ToString()))
                m.status = dataReader.GetString("status");

            return m;
        }
    }
}