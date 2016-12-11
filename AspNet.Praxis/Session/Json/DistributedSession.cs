using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Praxis.Session.Json {
    public class DistributedJsonSession : ISession
    {
        private static readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();
        private const int IdByteCount = 16;

        private readonly IDistributedCache _cache;
        private readonly string _sessionKey;
        private readonly TimeSpan _idleTimeout;
        private readonly Func<bool> _tryEstablishSession;
        private readonly ILogger _logger;
        private readonly bool _isNewSessionKey;
        private JObject _store;
        private bool _isModified;
        private bool _isAvailable;
        private string _sessionId;
        private byte[] _sessionIdBytes;

        public DistributedJsonSession(
            IDistributedCache cache,
            string sessionKey,
            TimeSpan idleTimeout,
            Func<bool> tryEstablishSession,
            ILoggerFactory loggerFactory,
            bool isNewSessionKey
        )
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            if (string.IsNullOrEmpty(sessionKey))
            {
                throw new ArgumentException("Argument cannot be null or empty.", nameof(sessionKey));
            }

            if (tryEstablishSession == null)
            {
                throw new ArgumentNullException(nameof(tryEstablishSession));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _cache = cache;
            _sessionKey = sessionKey;
            _idleTimeout = idleTimeout;
            _tryEstablishSession = tryEstablishSession;
            _logger = loggerFactory.CreateLogger<DistributedSession>();
            _isNewSessionKey = isNewSessionKey;
        }

        public bool IsAvailable
        {
            get
            {
                Load();
                return _isAvailable;
            }
        }

        public string Id
        {
            get
            {
                Load();
                return _sessionId ?? (_sessionId = new Guid(IdBytes).ToString());
            }
        }

        private byte[] IdBytes
        {
            get
            {
                if (IsAvailable && _sessionIdBytes == null)
                {
                    _sessionIdBytes = new byte[IdByteCount];
                    CryptoRandom.GetBytes(_sessionIdBytes);
                }
                return _sessionIdBytes;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                Load();
                return _store.Properties().Select(p => p.Name);
            }
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            Load();

            JToken encodedValue;
            if (_store.TryGetValue(key, out encodedValue))
            {
                value = encodedValue.Value<byte[]>();
                return true;
            }

            value = null;
            return false;
        }

        public void Set(string key, byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (IsAvailable)
            {
                if (!_tryEstablishSession())
                {
                    throw new InvalidOperationException("Sessons cannot be started after output has begun.");
                }

                JObject valueJson;

                try
                {
                    valueJson = JObject.Parse(Encoding.UTF8.GetString(value));
                }
                catch
                {
                    var copy = new byte[value.Length];
                    Buffer.BlockCopy(value, 0, copy, 0, value.Length);
                    valueJson = JObject.FromObject(value);
                }
                
                _store.Add(key, valueJson);
                _isModified = true;
            }
        }

        public void Remove(string key)
        {
            Load();
            _isModified |= _store.Remove(key);
        }

        public void Clear()
        {
            Load();
            _isModified |= _store.Count > 0;
            _store = new JObject();
        }

        private void Load()
        {
            if (_store == null)
            {
                try
                {
                    var data = _cache.GetString(_sessionKey);
                    if (data != null)
                    {
                        _store = JObject.Parse(data) ?? new JObject();
                    }
                    else if (!_isNewSessionKey)
                    {
                        _logger.AccessingExpiredSession(_sessionKey);
                    }
                    _store = new JObject();
                    _isAvailable = true;
                }
                catch (Exception exception)
                {
                    _logger.SessionCacheReadException(_sessionKey, exception);
                    _isAvailable = false;
                    _sessionId = string.Empty;
                    _sessionIdBytes = null;
                    _store = new NoOpSessionStore();
                }
            }
        }

        public async Task LoadAsync()
        {
            if (_store == null)
            {
                var data = await _cache.GetStringAsync(_sessionKey);
                if (data != null)
                {
                    _store = JObject.Parse(data) ?? new JObject();
                }
                else if (!_isNewSessionKey)
                {
                    _logger.AccessingExpiredSession(_sessionKey);
                }

                _isAvailable = true;
            }
        }

        public async Task CommitAsync()
        {
            if (_isModified)
            {
                await _cache.SetStringAsync(
                    _sessionKey,
                    _store.ToString(),
                    new DistributedCacheEntryOptions
                    {
                        SlidingExpiration =_idleTimeout
                    });

                _isModified = false;
                _logger.SessionStored(_sessionKey, Id, _store.Count);
            }
            else
            {
                await _cache.RefreshAsync(_sessionKey);
            }
        }
    }
}