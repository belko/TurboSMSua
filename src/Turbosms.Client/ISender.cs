namespace Turbosms.Client
{
    public interface ISender
    {
        string Exceptions { get; set; }
        int SendMessage(string receiver,string sender, string message);
    }
}