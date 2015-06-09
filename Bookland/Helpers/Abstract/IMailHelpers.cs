namespace Bookland.Helpers.Abstract
{
    public interface IMailHelpers
    {
        bool SendAdminEmail(string toAddress, string subject, string body);
    }
}
