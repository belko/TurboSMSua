using System;
using MySql.Data.MySqlClient;

namespace Turbosms.Client
{
    public class Message
    {

        public Message()
        {
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
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                
                try
                {
                    Status = (Status) Enum.Parse(typeof(Status), value);
                }
                catch
                {

                }
            }
        }

        public Status Status { get; private set; }

        public string ErrorDetail
        {
            get
            {
                var detail = "";

                switch (error_code)
                {
                    case "0":
                        detail = "Ошибок нет";
                        break;
                    case "2":
                        detail =
                            "Не удалось сохранить данные, свяжитесь с отделом поддержки если ошибка будет повторяться";
                        break;
                    case "23":
                        detail = "Ошибки в номере получателя";
                        break;
                    case "34":
                        detail = "Страна получателя не поддерживается, необходима дополнительная активация";
                        break;
                    case "36":
                        detail =
                            "Не удалось отправить сообщение, свяжитесь с отделом поддержки, если ошибка будет повторяться";
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
                    default:
                        detail = "Неизвестный код ошибки.";
                        break;
                }

                return detail;
            }
        }
    }
}
