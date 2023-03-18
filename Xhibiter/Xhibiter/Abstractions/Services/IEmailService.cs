namespace Xhibiter.Abstractions.Services
{
    public interface IEmailService
    {
        public void Send(string mailTo, string subject, string body, bool isBodyHtml = false);

    }
}
