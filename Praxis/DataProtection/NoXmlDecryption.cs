using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;

namespace Praxis.DataProtection
{
    public class NoXmlDecryption : IXmlDecryptor
    {
        public XElement Decrypt(XElement encryptedElement)
        {
            return encryptedElement;
        }
    }
}
