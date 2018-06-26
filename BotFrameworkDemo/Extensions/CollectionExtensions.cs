using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotFrameworkDemo.Extensions
{
    public static class CollectionExtensions
    {
        public static T PickOne<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return default(T);
            }

            var ran = new Random(Guid.NewGuid().GetHashCode());
            var index = ran.Next(0, collection.Count() - 1);
            return collection.ElementAt(index);
        }

        public static string PickOneWithParams<T>(this IEnumerable<T> collection, params string[] args)
        {
            string format = collection.PickOne() as string;
            if (string.IsNullOrEmpty(format))
            {
                return string.Empty;
            }

            return string.Format(format, args);
        }

        public static bool PartialContains(this IEnumerable<string> collection, string text)
        {
            return collection.Any(x => text.IndexOf(x, StringComparison.OrdinalIgnoreCase) > -1);
        }

        public static bool ContainsIgnoreCase(this IEnumerable<string> collection, string text)
        {
            return collection.Any(x => text.Equals(x, StringComparison.OrdinalIgnoreCase));
        }
    }
}