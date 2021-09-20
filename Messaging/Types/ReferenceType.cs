using System;
using Messaging.Interfaces;

namespace Messaging.Types
{
    public class ReferenceType<T> : Data<T>
        where T : class
    {
        public Action ClearStrategy { get; set; }

        public ReferenceType(T value, ISerializer<T> serializer) : base(value, serializer)
        {
            ClearStrategy = ClearWithNew_ExpectDefaultConstructable;
        }

        public override void Clear()
        {
            ClearStrategy.Invoke();
        }

        private void ClearWithNew_ExpectDefaultConstructable()
        {
            if (Activator.CreateInstance(typeof(T)) is T t)
            {
                Value = t;
            }

        }
    }
}
