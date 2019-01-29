namespace Turbosms.Client
{
    public class SoapSender : ISender
    {
        public string Exceptions { get; set; }

        public int SendMessage(string receiver, string sender, string message)
        {
            throw new System.NotImplementedException();
        }
    }
}