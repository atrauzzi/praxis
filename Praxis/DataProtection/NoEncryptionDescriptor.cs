using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace Praxis.DataProtection
{
    public class NoEncryptionDescriptor : IAuthenticatedEncryptorDescriptor
    {
        public IAuthenticatedEncryptor CreateEncryptorInstance()
        {
            return new NoEncryption();
        }

        public XmlSerializedDescriptorInfo ExportToXml()
        {
            return new XmlSerializedDescriptorInfo(new XElement("None"), typeof(NoEncryptionDescriptorDeserializer));
        }
    }
}
