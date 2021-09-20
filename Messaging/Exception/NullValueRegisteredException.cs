namespace Messaging.Exception
{
    class NullValueRegisteredException : System.Exception
    {
        public NullValueRegisteredException(string factoryName, string key) : base(GetMessage(factoryName, key))
        {

        }

        #region PrivateHelper

        public static string GetMessage(string factoryName, string key)
        {
            return $"Value registered to key {key} in factory {factoryName} is null";
        }

        #endregion
    }
}