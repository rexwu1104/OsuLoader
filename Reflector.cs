using System;
using System.Reflection;

namespace OsuLoader
{
    public static class Reflector
    {
        internal static T GetField<T>(object source, string name)
        {
            var type = source.GetType();
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return (T)field?.GetValue(source);
        }

        internal static T GetStaticField<T>(Type source, string name)
        {
            var type = source;
            var field = type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return (T)field?.GetValue(null);
        }

        internal static void SetField(object source, string name, object value)
        {
            var type = source.GetType();
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            field?.SetValue(source, value);
        }

        internal static void SetStaticField(Type source, string name, object value)
        {
            var field = source.GetField(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            field?.SetValue(null, value);
        }

        internal static T CallMethod<T>(object source, string methodName, params object[] parameters)
        {
            var type = source.GetType();
            var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return (T)method?.Invoke(source, parameters);
        }

        internal static T CallStaticMethod<T>(Type source, string methodName, params object[] parameters)
        {
            var type = source;
            var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return (T)method?.Invoke(null, parameters);
        }
    }
}