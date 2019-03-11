using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared.Extension
{
    public static class EnumrableExtesion
    {
        public static IEnumerable<TSource> SortLike<TSource, TKey>(this ICollection<TSource> source,
                                            IEnumerable<TKey> sortOrder)
        {
            var cloned = sortOrder.ToArray();
            var sourceArr = source.ToArray();
            Array.Sort(cloned, sourceArr);
            return sourceArr;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int takeCount)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (takeCount < 0) { throw new ArgumentOutOfRangeException("takeCount", "must not be negative"); }
            if (takeCount == 0) { yield break; }

            T[] result = new T[takeCount];
            int i = 0;

            int sourceCount = 0;
            foreach (T element in source)
            {
                result[i] = element;
                i = (i + 1) % takeCount;
                sourceCount++;
            }

            if (sourceCount < takeCount)
            {
                takeCount = sourceCount;
                i = 0;
            }

            for (int j = 0; j < takeCount; ++j)
            {
                yield return result[(i + j) % takeCount];
            }
        }

        public static bool IsNullorEmpty(this IEnumerable enumerator)
        {
            var erator = enumerator.GetEnumerator();
            erator.MoveNext();
            return (enumerator == null || erator.Current == null);
        }

        public static bool IsNullorEmpty<T>(this IEnumerable<T> enumerator)
        {
            return (enumerator == null || enumerator.Count() == 0);
        }

        public static string SelectRandomString(this string[] s)
        {
            if (!s.IsNullorEmpty())
            {
                var index = Random.Next(s.Length);
                return s[index];
            }
            return null;
        }

        public static Random Random { get; } = new Random(Guid.NewGuid().GetHashCode());

    }
}
