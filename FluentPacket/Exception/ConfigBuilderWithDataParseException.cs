using System.Reflection;

namespace FluentPacket.Exception
{
    class ConfigBuilderWithDataParseException : System.Exception
    {
        public ConfigBuilderWithDataParseException(MethodBase? withDataTValue, MethodBase? withDataTValueTag) : base(GetMessage(withDataTValue, withDataTValueTag))
        {

        }

        #region PrivateHelper

        public static string GetMessage(MethodBase? withDataTValue, MethodBase? withDataTValueTag)
        {
            return
                $"Unable to reflect on 'WithData' functions.  This is a programming error and needs to be reported to the developers.  withData={withDataTValue != null}, withDataTag={withDataTValueTag != null}";
        }

        #endregion
    }
}