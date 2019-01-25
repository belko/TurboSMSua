namespace Turbosms.Client
{
    public class SmppSender : ISender
    {
        public string Exceptions { get; set; }

        public int SendMessage(string receiver, string sender, string message)
        {
            throw new System.NotImplementedException();
        }
    }
}