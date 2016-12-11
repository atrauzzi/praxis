using System.Text;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace Praxis.Authentication
{
    public class JsonTicketSerializer : IDataSerializer<AuthenticationTicket>
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public byte[] Serialize(AuthenticationTicket ticket)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ticket, _jsonSerializerSettings));
        }

        public AuthenticationTicket Deserialize(byte[] data)
        {
            return JsonConvert.DeserializeObject<AuthenticationTicket>(Encoding.UTF8.GetString(data));
        }
    }
}