using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Praxis.Authentication
{
    public class SessionTicketStore : ITicketStore
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IDataSerializer<AuthenticationTicket> _ticketSerializer;

        public SessionTicketStore(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _ticketSerializer = new JsonTicketSerializer();
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Guid.NewGuid().ToString();
            _httpContextAccessor.HttpContext.Session.Set($"identity-{key}", _ticketSerializer.Serialize(ticket));
            return key;
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            // I'm good.
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var serializedTicket = _httpContextAccessor.HttpContext.Session.Get($"identity-{key}");

            if (serializedTicket != null)
            {
                return _ticketSerializer.Deserialize(serializedTicket);
            }

            return null;
        }

        public async Task RemoveAsync(string key)
        {
            _httpContextAccessor.HttpContext.Session.Remove(key);
        }
    }
}
