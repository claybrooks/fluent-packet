using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Messaging.Interfaces;

namespace Messaging.Abstractions
{

    public class Builder<TP>
        where TP : Packet, new()
    {
        private TP _packet;
        
        private readonly MethodInfo _voidWithData;
        private readonly MethodInfo _paramWithData;
        private readonly MethodInfo _tagWithData;
        private readonly MethodInfo _paramTagWithData;

        public Builder()
        {
            _packet = new TP();

            var tbType = typeof(Builder<TP>);

            foreach (MethodInfo mi in tbType.GetMethods())
            {
                if (!mi.Name.Equals("WithData"))
                {
                    continue;
                }

                var mp = mi.GetParameters();

                switch (mp.Length)
                {
                    case 0:
                        _voidWithData = mi;
                        break;
                    case 2:
                        _paramTagWithData = mi;
                        break;
                    default:
                    {
                        if (mp[0].ParameterType.IsGenericParameter)
                        {
                            _paramWithData = mi;
                        }
                        else
                        {
                            _tagWithData = mi;
                        }

                        break;
                    }
                }
            }

            if (_voidWithData == null || _paramWithData == null || _tagWithData == null || _paramTagWithData == null)
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
            if (BuildFromFile(reader, filename))
            {
                return Build();
            }

            return Build();
        }

        public TP Build()
        {
            return _packet;
        }

        public Builder<TP> WithData<T>()
        {
            return WithData(default(T));
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
            if (!GetType(out Type? dataType, config.TypeName) || dataType == null)
            {
                return false;
            }

            var tag = ResolveTag(config.Tag);

            switch (tag)
            {
                case -1 when string.IsNullOrEmpty(config.Value):
                    return WithData(dataType);
                case -1:
                    return WithData(dataType, config.Value);
                default:
                    return string.IsNullOrEmpty(config.Value) ? WithData(dataType, tag) : WithData(dataType, config.Value, tag);
            }
        }

        private bool WithData(Type value)
        {
            MethodInfo genericMethod = _voidWithData.MakeGenericMethod(value);
            genericMethod.Invoke(this, null);
            return true;
        }

        private bool WithData(Type dataType, string value)
        {
            var convertedValue = Convert.ChangeType(value, dataType);
            
            MethodInfo genericMethod = _paramWithData.MakeGenericMethod(dataType);
            genericMethod.Invoke(this, new[] { convertedValue });
            return true;
        }

        private bool WithData(Type dataType, long tag)
        {
            MethodInfo genericMethod = _tagWithData.MakeGenericMethod(dataType);
            genericMethod.Invoke(this, new object[] { tag });
            return true;
        }

        private bool WithData(Type dataType, string value, long tag)
        {
            var convertedValue = Convert.ChangeType(value, dataType);
            
            MethodInfo genericMethod = _paramTagWithData.MakeGenericMethod(dataType);
            genericMethod.Invoke(this, new[] { convertedValue, tag });
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
