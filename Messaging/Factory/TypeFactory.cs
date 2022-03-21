using System;
using System.Collections.Generic;

namespace FluentPacket.Factory
{
    public class TypeFactory
    {
        private readonly IDictionary<string, Type> _types = new Dictionary<string, Type>();

        public Type Get(string name)
        {
            return _types.TryGetValue(name, out var type) ? type : DefaultRegisterAndGetValue(name);
        }

        public void Register<T>(string? name = null)
        {
            name ??= typeof(T).Name;
            _types.Add(name, typeof(T));
        }

        private Type DefaultRegisterAndGetValue(string typeName)
        {
            // Try to load the name normally
            var t = Type.GetType(typeName);

            if (t == null)
            {
                throw new ArgumentException($"Unable to get type {typeName}", nameof(typeName));
            }

            _types.Add(typeName, t);
            return t;
        }
    }
}
