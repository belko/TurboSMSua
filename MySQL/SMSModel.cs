using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurboSMSua.MySQL
{
    public class SMSModel
    {

        public SMSModel() { }
        public SMSModel(MySqlDataReader dataReader)
        {
            id = dataReader.GetInt32("id");

            if (!string.IsNullOrEmpty(dataReader["msg_id"].ToString()))
                msg_id = dataReader.GetString("msg_id");

            if (!string.IsNullOrEmpty(dataReader["number"].ToString()))
                number = dataReader.GetString("number");

            if (!string.IsNullOrEmpty(dataReader["sign"].ToString()))
                sign = dataReader.GetString("sign");

            if (!string.IsNullOrEmpty(dataReader["cost"].ToString()))
                cost = dataReader.GetDecimal("cost");

            if (!string.IsNullOrEmpty(dataReader["balance"].ToString()))
                balance = dataReader.GetDecimal("balance");

            if (!string.IsNullOrEmpty(dataReader["added"].ToString()))
                added = dataReader.GetString("added");

            if (!string.IsNullOrEmpty(dataReader["sended"].ToString()))
                sended = dataReader.GetString("sended");

            if (!string.IsNullOrEmpty(dataReader["received"].ToString()))
                received = dataReader.GetString("received");

            if (!string.IsNullOrEmpty(dataReader["error_code"].ToString()))
                error_code = dataReader.GetString("error_code");

            if (!string.IsNullOrEmpty(dataReader["status"].ToString()))
                status = dataReader.GetString("status");
        }
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm";

        /// <summary>
        /// read only
        /// </summary>
        public int id { get; set; }
        //read
        public string msg_id { get; set; }
        public string number { get; set; }
        public string sign { get; set; }
        public string message { get; set; }
        //read
        public decimal cost { get; set; }
        //read
        public decimal balance { get; set; }

        //read
        public string added { get; set; }

        public string send_time { get; set; }

        public DateTime SendTime { get; set; }
        //read
        public string sended { get; set; }

        public DateTime Sended { get; set; }

        //read
        public string received { get; set; }
        public DateTime Received { get; private set; }
        //read
        public string error_code { get; set; }

        private string _status;
        //read
        public string status
        {
            get { return _status; }
            set
            {
                _status = value;
                try
                {
                    Status = (SMSStatus)Enum.Parse(typeof(SMSStatus), value);
                }
                catch 
                {

                }
            }
        }

        public SMSStatus Status { get; private set; }

        public string ErrorDetail
        {
            get {
                string detail="";
                switch (error_code)
                {
                    case "0":
                        detail = "Ошибок нет";
                        break;
                    case "2":
                        detail = "Не удалось сохранить данные, свяжитесь с отделом поддержки если ошибка будет повторяться";
                        break;
                    case "23":
                        detail = "Ошибки в номере получателя";
                        break;
                    case "34":
                        detail = "Страна получателя не поддерживается, необходима дополнительная активация";
                        break;
                    case "36":
                        detail = "Не удалось отправить сообщение, свяжитесь с отделом поддержки, если ошибка будет повторяться";
                        break;
                    case "40":
                        detail = "Недостаточно кредитов на балансе";
                        break;
                    case "46":
                        detail = "Номер получателя в стоплисте";
                        break;
                    case "69":
                        detail = "Альфаимя (подпись отправителя) запрещено администратором";
                        break;
                    case "83":
                        detail = "Дубликат сообщения";
                        break;
                    case "84":
                        detail = "Отсутствует текст сообщения";
                        break;
                    case "85":
                        detail = "Неверное альфаимя (подпись отправителя)";
                        break;
                    case "86":
                        detail = "Текст сообщения содержит запрещённые слова";
                        break;
                    case "87":
                        detail = "Слишком длинный текст сообщения";
                        break;
                    case "88":
                        detail = "Ваша учётная запись заблокирована за нарушения, свяжитесь с отделом поддержки";
                        break;
                    case "999":
                        detail = "Специфическая ошибка конкретного оператора, необходимо уточнять дополнительно";
                        break;
                    default :
                        detail = "Неизвестный код ошибки.";
                        break;
                }
                return detail;
            }
        }

        
    }
}
