using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SAH2.Core.Extensions
{
    public static class EnumerableListExtensions
    {
        /// <summary>
        ///    Runs the action for every item in the list and  returns the list.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            var array = source as TSource[] ?? source.ToArray();
            foreach (var item in array)
                action(item);

            return array;
        }

        /// <summary>
        ///    Runs the action for every item that meets the declared condition in the list and  returns the list.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="action"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool> selector, Action<TSource> action)
        {
            var array = source as TSource[] ?? source.ToArray();
            array.Where(selector).ForEach(action);
            return array;
        }

        /// <summary>
        /// Converts the list to an <see cref="ObservableCollection{T}"/> and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source is null)
                return null;

            var collection = new ObservableCollection<T>();

            foreach (var item in source)
                collection.Add(item);

            return collection;
        }
    }
}