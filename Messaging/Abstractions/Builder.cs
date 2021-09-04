using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Messaging.Interfaces;

namespace Messaging.Abstractions
{
    internal class MethodInfoHelper
    {
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            if (expression.Body is MethodCallExpression member)
                return member.Method;

            throw new ArgumentException("Expression is not a method", "expression");
        }
    }

    public class Builder<TP>
        where TP : Packet, new()
    {
        private TP _packet;
        private readonly MethodInfo _withDataTValue;
        private readonly MethodInfo _withDataTValueTag;

        public Builder()
        {
            _packet = new TP();

            var tbType = typeof(Builder<TP>);

            foreach (MethodInfo mi in tbType.GetMethods())
            {
                if (!mi.Name.Equals("WithData") || !mi.IsGenericMethod)
                {
                    continue;
                }

                var mp = mi.GetParameters();

                switch (mp.Length)
                {
                    case 2:
                        _withDataTValueTag = mi;
                        break;
                    case 1:
                        _withDataTValue = mi;
                        break;
                }
            }

            if (_withDataTValue == null || _withDataTValueTag == null)
            {
                throw new Exception(
                    "Unable to construct ConfigBuilder.  Could not find necessary 'WithData' functions");
            }
        }

        public Builder<TP> Setup()
        {
            _packet = new TP();
            return this;
        }

        public TP FromConfig(IConfigReader reader, string filename)
        {
            // TODO Fix this
            if (!BuildFromFile(reader, filename))
            {
                throw new Exception("Unable to build from file");
            }

            return Build();
        }

        public TP Build()
        {
            return _packet;
        }

        public Builder<TP> WithData<T>(T value)
        {
            _packet.AddData(value);
            return this;
        }

        public Builder<TP> WithData<T>(T value, long tag)
        {
            _packet.AddData(value, tag);
            return this;
        }

        #region Private Methods

        private bool BuildFromFile(IConfigReader reader, string file)
        {
            var configs = reader.Read(file);
            return BuildData(configs);
        }

        private bool BuildData(IEnumerable<IConfig> configs)
        {
            return configs.All(WithData);
        }

        private bool WithData(IConfig config)
        {
            if (!GetType(out var type, config.TypeName) || type == null)
            {
                return false;
            }

            var tag = config.Tag != null ? ResolveTag(config.Tag) : -1;
            var value = config.Value ?? Activator.CreateInstance(type);

            if (value == null)
            {
                throw new Exception("Unable to create default value");
            }

            return tag switch
            {
                -1 => WithData(type, value),
                _ => WithData(type, value, tag)
            };
        }

        private bool WithData(Type type, object value)
        {
            var convertedValue = Convert.ChangeType(value, type);

            if (convertedValue == null)
            {
                throw new Exception("Unable to create default value");
            }

            return WithData(_withDataTValue, type, convertedValue);
        }

        private bool WithData(Type type, object value, long tag)
        {
            var convertedValue = Convert.ChangeType(value, type);
            return WithData(_withDataTValueTag, type, convertedValue, tag);
        }

        private bool WithData(MethodInfo method, Type type, params object[] mParams)
        {
            MethodInfo genericMethod = method.MakeGenericMethod(type);
            genericMethod.Invoke(this, mParams);
            return true;
        }

        private bool GetType(out Type? type, string name)
        {
            type = _packet.TypeFactory.Get(name);
            return type != null;
        }

        private static long ResolveTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return -1;
            }

            return (long)Convert.ChangeType(tag, typeof(long));
        }

        #endregion
    }
}
