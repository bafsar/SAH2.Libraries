using System;
using System.Collections.Generic;
using System.Dynamic;

namespace SAH2.Core.Extensions
{
    public static class ExpandoObjectExtensions
    {
        /// <summary>
        ///    Gets wanted item from the <see cref="ExpandoObject"/>. If it doesn't exist, returns null.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key">The key for item to search</param>
        /// <param name="throwExceptionIsNull">Throw an exception if the value that looking for is none?</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">
        ///     Throws an exception if <see cref="throwExceptionIsNull" /> value setted <see langword="true"/> and value that looking for doesn't exist.
        /// </exception>
        public static object GetValue(this ExpandoObject obj, string key, bool throwExceptionIsNull = false)
        {
            ((IDictionary<string, object>)obj).TryGetValue(key, out var v);

            if (throwExceptionIsNull && v == null)
                throw new NullReferenceException();

            return v;
        }

        /// <summary>
        ///    Checks the searched item whether exist or not in the <see cref="ExpandoObject"/> and returns the result
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key">The key for item to search</param>
        /// <returns></returns>
        public static bool IsValueExist(this ExpandoObject obj, string key)
        {
            return obj.GetValue(key) != null;
        }

        /// <summary>
        ///   Adds the sent object to the <see cref="ExpandoObject"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add(this ExpandoObject obj, string key, object value)
        {
            ((IDictionary<string, object>)obj).Add(key, value);
        }

        /// <summary>
        ///   Removes the searhed item from the <see cref="ExpandoObject"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        public static void Remove(this ExpandoObject obj, string key)
        {
            ((IDictionary<string, object>)obj).Remove(key);
        }
    }
}