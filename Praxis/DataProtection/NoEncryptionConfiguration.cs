using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace Praxis.DataProtection
{
    public class NoEncryptionConfiguration : IAuthenticatedEncryptorConfiguration
    {
        public IAuthenticatedEncryptorDescriptor CreateNewDescriptor()
        {
            return new NoEncryptionDescriptor();
        }
    }
}
