using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace SAH2.Core.Extensions
{
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        ///    Creates clone of the <see cref="newObject"/> and changes with every item that meets the declared condition. Then returns the collection.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="newObject"></param>
        /// <returns></returns>
        public static ObservableCollection<TSource> Change<TSource>(this ObservableCollection<TSource> source,
            Func<TSource, bool> selector, TSource newObject)
        {
            if (!source.Any(selector))
                return source;

            var correspondRecords = source.Where(selector).ToList();
            foreach (var t in correspondRecords)
            {
                var rIndex = source.IndexOf(t);
                source[rIndex] = newObject.Clone();
            }
            return source;
        }

        /// <summary>
        ///    Runs the action for every item that meets the declared condition in the collection and returns it.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ObservableCollection<TSource> ForEach<TSource>(this ObservableCollection<TSource> source,
            Func<TSource, bool> selector, Action<TSource> action)
        {
            if (source.Any(selector))
            {
                foreach (var r in source.Where(selector))
                    action(r);
            }
            return source;
        }

        /// <summary>
        ///    Removes every item that meets the declared condition from collection and then returns the collection.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static ObservableCollection<TSource> Remove<TSource>(this ObservableCollection<TSource> source,
            Func<TSource, bool> selector)
        {
            while (source.Any(selector))
            {
                var r = source.FirstOrDefault(selector);
                source.Remove(r);
            }
            return source;
        }


        /// <summary>
        /// Adds items to the collection and returns it.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static ObservableCollection<TModel> AddRange<TModel>(this ObservableCollection<TModel> source, params TModel[] items)
        {
            if (!items.Any())
                return source;

            if (source == null)
                source = new ObservableCollection<TModel>();

            foreach (var item in items)
            {
                source.Add(item);
            }

            return source;
        }

        /// <summary>
        /// Adds items to the collection and returns it.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static ObservableCollection<TModel> AddRange<TModel>(this ObservableCollection<TModel> source, IEnumerable<TModel> items)
        {
            var array = items as TModel[] ?? items.ToArray();
            return source.AddRange(array);
        }



        /// <summary>
        /// Clones the source and returns it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}