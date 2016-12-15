using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace Praxis.DataProtection
{
    public class NoEncryptionDescriptorDeserializer : IAuthenticatedEncryptorDescriptorDeserializer
    {
        public IAuthenticatedEncryptorDescriptor ImportFromXml(XElement element)
        {
            return new NoEncryptionDescriptor();
        }
    }
}