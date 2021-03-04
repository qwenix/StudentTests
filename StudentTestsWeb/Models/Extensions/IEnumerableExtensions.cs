using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentTestsWeb {
    public static class IEnumerableExtensions {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            if (source == null || action == null)
                throw new ArgumentNullException();

            foreach (T element in source) {
                action(element);
            }
        }
    }
}
