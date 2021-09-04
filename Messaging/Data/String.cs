namespace Messaging.Data
{
    public class String : ReferenceType<string>
    {
        public String() : this("")
        {
        }
        public String(string value) : base(value)
        {
        }
    }
}
