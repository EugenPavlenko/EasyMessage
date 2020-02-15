using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyMessage.Core.Utils
{
    public static class ReflectionUtils
    {
        /// <summary>
        /// Recursively search a type to match generic interface base
        /// </summary>
        /// <param name="type">Type to search</param>
        /// <param name="genericType">Match interfaces</param>
        /// <returns></returns>
        public static List<Type> FilterInterfacesForGenericType(Type type, Type genericType) => FilterInterfacesForGenericType(type.GetInterfaces(), genericType);

        /// <summary>
        /// Recursively search interfaces to match a generic interface base
        /// </summary>
        /// <param name="interfaces"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static List<Type> FilterInterfacesForGenericType(IEnumerable<Type> interfaces, Type genericType)
        {
            var results = new List<Type>();
            foreach (var iFaceType in interfaces)
            {
                if (iFaceType.IsGenericType && iFaceType.GetGenericTypeDefinition() == genericType)
                {
                    results.Add(iFaceType);
                }
                if (iFaceType.BaseType == null) continue;
                var baseInterfaces = FilterInterfacesForGenericType(iFaceType.BaseType.GetInterfaces(), genericType);
                results.AddRange(baseInterfaces);
            }
            return results;
        }

        /// <summary>
        /// Gets all interfaces for type and its base types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllInterfaces(Type type)
        {
            var results = new List<Type>();
            do
            {
                results.AddRange(type.GetInterfaces());
                type = type.BaseType;
            } while (type != null);
            return results.Distinct();
        }

        /// <summary>
        /// Determines whether the current type has the specified interface
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool IsContainInterface(this Type type, Type interfaceType)
        {
            var types = type.GetInterfaces();

            return interfaceType.IsGenericType ?
                types.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == interfaceType)
                : types.Any(x => x == interfaceType);
        }
    }
}