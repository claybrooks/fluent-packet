using System;
using System.Collections.Generic;
using Messaging.Interfaces;

namespace Messaging.Abstractions
{
    public class TypeFactory
    {
        private readonly IDictionary<string, Type> _types = new Dictionary<string, Type>();

        public Type? Get(string name)
        {
            if (_types.TryGetValue(name, out var type))
            {
                return type;
            }

            // Try to load the name normally
            var t = Type.GetType(name);
            if (t == null)
            {
                return null;
            }

            _types.Add(name, t);
            return t;
        }

        public void Register<T>()
        {
            var type = typeof(T);
            if (type.FullName == null)
            {
                throw new Exception("Unable to discern type info");
            }

            Register<T>(type.FullName);
        }

        public void Register<T>(string name)
        {
            _types.Add(name, typeof(T));
        }
    }
}
