using Microsoft.AspNetCore.DataProtection;

namespace Praxis.Security
{
    public class PassthroughDataProtectionProvider : IDataProtectionProvider
    {
        public IDataProtector CreateProtector(string purpose)
        {
            return new PassthroughDataProtector();
        }
    }
}
