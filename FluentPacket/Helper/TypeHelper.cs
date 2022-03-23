using FluentPacket.Interfaces;
using System;
using System.Reflection;

namespace FluentPacket.Helper
{
    public static class TypeHelper
    {
        public static bool HasConstructorArguments(IConfig config)
        {
            return config.TypeArgs is { Length: > 0 };
        }

        public static bool ConstructorArgsMatchesConfigArgs(ConstructorInfo constructor, IConfig config)
        {
            return constructor.GetParameters().Length == config.TypeArgs.Length;
        }

        public static bool TryGetConstructorParameterListAsConfigTypeArgs(ConstructorInfo constructor, IConfig config, out object[] parameters)
        {
            var constructorParams = constructor.GetParameters();
            parameters = new object[constructorParams.Length];

            for (var i = 0; i < constructorParams.Length; i++)
            {
                try
                {
                    parameters[i] = Convert.ChangeType(config.TypeArgs[i], constructorParams[i].ParameterType);
                }
                catch (InvalidCastException)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool TryGetParameters(IConfig config, Type type, out object[]? parameters)
        {
            parameters = null;

            foreach (ConstructorInfo constructor in type.GetConstructors())
            {
                if (ConstructorArgsMatchesConfigArgs(constructor, config) &&
                    TryGetConstructorParameterListAsConfigTypeArgs(constructor, config, out parameters))
                {
                    break;
                }
            }

            return parameters != null;
        }

        public static Type GetArrayElementType(Type type)
        {
            if (!type.IsArray)
            {
                throw new ArgumentException("Type is not an array");
            }

            // This null override is safe because we've already checked if type.IsArray.
            // If this logic is ever refactored, be sure to reconsider the null override.
            return type.GetElementType()!;
        }

        public static Type GetArrayElementType<T>()
        {
            return GetArrayElementType(typeof(T));
        }

        public static Type GetConcreteArrayElementType(Type type)
        {
            Type t = GetArrayElementType(type);

            if (t.IsGenericType || t.IsAbstract)
            {
                throw new ArgumentException("Type is not concrete");
            }

            return t;
        }

        public static Type GetConcreteArrayElementType<T>()
        {
            return GetConcreteArrayElementType(typeof(T));
        }

        public static bool IsArrayValueType<T>()
        {
            Type type;
            try
            {
                type = GetArrayElementType<T>();
            }
            catch (ArgumentException)
            {
                return false;
            }

            return type.IsValueType;
        }

        public static bool IsValueType<T>()
        {
            return typeof(T).IsValueType;
        }
    }
}
