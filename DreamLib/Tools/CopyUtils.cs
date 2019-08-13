using DreamLib.Attributes;
using System.Reflection;
using System.Linq;
using System;

namespace DreamLib.Tools
{
    public static class CopyUtils
    {
        public static void Update<T>(this T original, T update) where T : class
        {
            if (original is null)
                throw new NullReferenceException("Could not update the original as it is not set to a reference");
            if (update is null)
                throw new NullReferenceException("Could not update the original as the update is not set to a reference");

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty;
            foreach(PropertyInfo property in typeof(T).GetProperties(bindingFlags).Where(_property => _property.GetCustomAttribute<CopyIgnoreAttribute>() is null))
            {
                if((property.SetMethod?.IsPublic).GetValueOrDefault() && (property.GetMethod?.IsPublic).GetValueOrDefault())
                {
                    property.SetValue(original, property.GetValue(update));
                }
            }
        }

        public static T Copy<T>(this T original) where T : class
        {
            T instance = (T)typeof(T).GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            instance.Update(original);
            return instance;
        }
    }
}
