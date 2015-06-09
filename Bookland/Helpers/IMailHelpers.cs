namespace Bookland.Helpers
{
    public interface IMailHelpers
    {
        bool SendAdminEmail(string toAddress, string subject, string body);
    }
}
