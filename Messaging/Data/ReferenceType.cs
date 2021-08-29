using Messaging.Interfaces;
using Messaging.Abstractions;

namespace Messaging.Data
{
    public abstract class ReferenceType<T, TS> : Data<T, TS> where T : class where TS : ISerializer<T>
    {
        protected ReferenceType(ISerializerFactory factory) : base(factory)
        {

        }

        protected ReferenceType(ISerializerFactory factory, T value) : base(factory, value)
        {

        }
    }
}
