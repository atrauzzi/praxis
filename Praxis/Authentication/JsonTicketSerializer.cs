using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var ticketJson = new JObject();

            ticketJson.Add("scheme", ticket.AuthenticationScheme);

            ticketJson.Add("properties", SerializeProperties(ticket.Properties));

            var identitiesJson = new JArray();

            foreach (var identity in ticket.Principal.Identities)
            {
                identitiesJson.Add(SerializeIdentity(identity));
            }

            ticketJson.Add("identities", identitiesJson);

            return Encoding.UTF8.GetBytes(ticketJson.ToString());
        }

        private JObject SerializeProperties(AuthenticationProperties properties)
        {
            var propertiesJson = new JObject();

            foreach (var property in properties.Items)
            {
                propertiesJson.Add(property.Key, property.Value);
            }

            return propertiesJson;
        }

        private JObject SerializeIdentity(ClaimsIdentity identity)
        {
            var identityJson = new JObject();

            identityJson.Add("authentication_type", identity.AuthenticationType);

            var claimsJson = new JObject();

            foreach (var claim in identity.Claims)
            {
                claimsJson.Add(claim.Type, new JValue(claim.Value));
            }

            identityJson.Add("claims", claimsJson);

            return identityJson;
        }

        //
        //

        public AuthenticationTicket Deserialize(byte[] data)
        {
            var dataJson = JObject.Parse(Encoding.UTF8.GetString(data));

            return new AuthenticationTicket(
                DeserializePrincipal(dataJson),
                DeserializeProperties(dataJson),
                dataJson.SelectToken("scheme").Value<string>()
            );
        }

        private ClaimsPrincipal DeserializePrincipal(JToken data)
        {
            return new ClaimsPrincipal(
                data.SelectToken("identities")?.Select(identityJson =>
                {

                    return new ClaimsIdentity(
                        identityJson.SelectToken("claims")?.Value<JObject>()?.Properties()?.Select(propertyJson =>
                            new Claim(
                                propertyJson.Name,
                                propertyJson.Value.Value<string>()
                            )),
                        identityJson.SelectToken("authentication_type")?.Value<string>());

                }));
        }

        private AuthenticationProperties DeserializeProperties(JToken data)
        {
            return new AuthenticationProperties(data
                .SelectToken("properties")?
                .Value<JObject>()?
                .Properties()?
                .ToDictionary(
                    propertyJson => propertyJson.Name,
                    propertyJson => propertyJson.Value.Value<string>()));
        }
    }
}