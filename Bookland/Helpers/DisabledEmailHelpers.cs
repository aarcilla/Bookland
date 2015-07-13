
namespace Bookland.Helpers
{
    public class DisabledEmailHelpers : Abstract.IMailHelpers
    {
        public bool SendAdminEmail(string toAddress, string subject, string body)
        {
            return true;
        }
    }
}