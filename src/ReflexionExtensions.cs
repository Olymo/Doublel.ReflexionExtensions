using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Doublel.ReflexionExtensions
{
    public static class ReflectionExtensions
    {
        public static bool HasAttribute<T>(this PropertyInfo property) where T : Attribute => Attribute.IsDefined(property, typeof(T));

        public static bool InheritsFrom<ParrentType>(this Type t) => t.InheritsFrom(typeof(ParrentType));

        public static bool InheritsFrom(this Type t, Type toInheritFrom) => toInheritFrom.GetTypeInfo().IsAssignableFrom(t.GetTypeInfo());

        public static TValue GetAttributeValue<TAttribute, TValue>(
                this Type type,
                Func<TAttribute, TValue> valueSelector)
                where TAttribute : Attribute
        {
            return type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() is TAttribute att
                ? valueSelector(att)
                : default;
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(
                this PropertyInfo property,
                Func<TAttribute, TValue> valueSelector)
                where TAttribute : Attribute
        {
            return property.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() is TAttribute att
                ? valueSelector(att)
                : default;
        }

        public static IEnumerable<T> GetAttibutes<T>(this object obj) where T : Attribute
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.HasAttribute<T>())
                {
                    yield return property.GetAttribute<T>();
                }
            }
        }

        public static PropertyInfo GetProperty(this object obj, string propertyName)
        {
            if(!PropertyCanBeAccessed(obj.GetType(), propertyName))
            {
                return null;
            }

            if(IsNavigationProperty(propertyName))
            {
                return null;
            }

            return obj.GetType().GetProperty(propertyName);
        }

        public static T GetAttribute<T>(this PropertyInfo property) where T : Attribute => Attribute.GetCustomAttribute(property, typeof(T)) as T;

        public static bool PropertyCanBeAccessed(this Type t, string propertyName)
        {
            if (IsNavigationProperty(propertyName))
            {
                var propertyNames = propertyName.Split(".");

                var typeToCheckPropertyAt = t;

                foreach (var property in propertyNames)
                {
                    if (!typeToCheckPropertyAt.PropertyCanBeAccessed(property))
                    {
                        return false;
                    }

                    typeToCheckPropertyAt = typeToCheckPropertyAt.GetProperty(property).PropertyType;
                    continue;
                }

                return true;
            }

            return t.GetProperties().Any(x => x.Name == propertyName);
        }

        public static IEnumerable<T> GetAllImplementers<T>(this Assembly destinationAssembly) where T : class
        {
            var objects = new List<T>();
            foreach (var type in
                destinationAssembly.GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.GetConstructors().Where(z => z.GetParameters().Count() == 0).Count() == 1 && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type));
            }
            return objects;
        }

        private static bool IsNavigationProperty(string property) => property.Split('.').Count() > 1;
    }
}
