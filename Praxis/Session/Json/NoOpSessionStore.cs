using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Praxis.Session.Json
{
    internal class NoOpSessionStore : JObject, IEnumerable
    {
        public new byte[] this[string key]
        {
            get
            {
                return null;
            }

            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
            }
        }

        public new int Count { get; } = 0;

        public bool IsReadOnly { get; } = false;

        public ICollection<string> Keys { get; } = new string[0];

        public ICollection<byte[]> Values { get; } = new byte[0][];

        public void Add(KeyValuePair<string, byte[]> item) { }

        public void Add(string key, byte[] value) { }

        public void Clear() { }

        public bool Contains(KeyValuePair<string, byte[]> item) => false;

        public bool ContainsKey(string key) => false;

        public void CopyTo(KeyValuePair<string, byte[]>[] array, int arrayIndex) { }

        public new IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator() => Enumerable.Empty<KeyValuePair<string, byte[]>>().GetEnumerator();

        public bool Remove(KeyValuePair<string, byte[]> item) => false;

        public new bool Remove(string key) => false;

        public bool TryGetValue(string key, out byte[] value)
        {
            value = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
