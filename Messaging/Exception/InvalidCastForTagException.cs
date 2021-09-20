

using System;

namespace Messaging.Exception
{
    class InvalidCastForTagException<T> : System.Exception
    {
        public InvalidCastForTagException(int tag) : base(GetMessage(tag))
        {

        }
        public InvalidCastForTagException(int tag, Type? type) : base(GetMessage(tag, type))
        {

        }

        #region PrivateHelper

        public static string GetMessage(int tag, Type? type = null)
        {
            var msg = $"Unable to cast {nameof(Data<T>)} for tag {tag}";
            if (type != null)
            {
                msg += $".  Generic registered is {type.Name}";
            }

            return msg;
        }

        #endregion
    }
}
