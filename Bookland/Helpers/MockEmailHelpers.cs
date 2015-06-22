
namespace Bookland.Helpers
{
    public class MockEmailHelpers : Abstract.IMailHelpers
    {
        public bool SendAdminEmail(string toAddress, string subject, string body)
        {
            return true;
        }
    }
}