using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HpaUtility.Extensions
{   public static class LinqExtension
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(
            this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return YieldBatchElements(enumerator, batchSize - 1);
        }

        private static IEnumerable<T> YieldBatchElements<T>(
            IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (int i = 0; i < batchSize && source.MoveNext(); i++)
                yield return source.Current;
        }


        public static async Task<IList<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> selector, int maxDegreesOfParallelism = 4)
        {
            var results = new List<TResult>();

            var activeTasks = new HashSet<Task<TResult>>();
            foreach (var item in source)
            {
                activeTasks.Add(selector(item));
                if (activeTasks.Count >= maxDegreesOfParallelism)
                {
                    var completed = await Task.WhenAny(activeTasks);
                    activeTasks.Remove(completed);
                    results.Add(completed.Result);
                }
            }

            results.AddRange(await Task.WhenAll(activeTasks));
            return results;
        }
    }
}
