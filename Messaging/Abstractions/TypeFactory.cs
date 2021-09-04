using System;
using System.Collections.Generic;

namespace Messaging.Abstractions
{
    public class TypeFactory
    {
        private readonly IDictionary<string, Type> _types = new Dictionary<string, Type>();

        public Type Get(string name)
        {
            if (_types.TryGetValue(name, out var type))
            {
                return type;
            }

            // Try to load the name normally
            var t = Type.GetType(name);
            if (t == null)
            {
                throw new Exception("Unable to get type");
            }

            _types.Add(name, t);
            return t;
        }

        public void Register<T>(string name)
        {
            _types.Add(name, typeof(T));
        }
    }
}
