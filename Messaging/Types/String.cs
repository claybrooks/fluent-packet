using Messaging.Interfaces;
using Messaging.Serializer;

namespace Messaging.Types
{
    public class String : ReferenceType<string>
    {
        /*public String() : this("")
        {

        }*/

        public String(string value) : base(value, new StringSerializer(value.Length))
        {

        }

        /*
        public override void Clear()
        {
            Value = "";
        }
        */
    }
}
