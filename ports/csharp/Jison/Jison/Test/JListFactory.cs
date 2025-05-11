using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public static class JListFactory
    {
        public static JList<T> CreateList<T>() where T : class
        {
            return new JList<T>();
        }

        public static JList<T> CreateList<T>(int capacity) where T : class
        {
            if (capacity < 0)
            {
                throw new ArgumentException("Capacity cannot be negative", nameof(capacity));
            }
            return new JList<T>(capacity);
        }

        public static JList<T> CreateList<T>(IEnumerable<T> collection) where T : class
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            return new JList<T>(collection);
        }

        public static JList<T> CreateList<T>(params T[] items) where T : class
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            return new JList<T>(items);
        }

        public static JList<T> CreateListWithCapacity<T>(int capacity) where T : class
        {
            if (capacity < 0)
            {
                throw new ArgumentException("Capacity cannot be negative", nameof(capacity));
            }
            return new JList<T>(capacity);
        }

        public static JList<T> CreateListFromRange<T>(IEnumerable<T> collection, int startIndex, int count) where T : class
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (startIndex < 0)
            {
                throw new ArgumentException("Start index cannot be negative", nameof(startIndex));
            }
            if (count < 0)
            {
                throw new ArgumentException("Count cannot be negative", nameof(count));
            }

            var list = new JList<T>();
            int currentIndex = 0;
            foreach (var item in collection)
            {
                if (currentIndex >= startIndex && currentIndex < startIndex + count)
                {
                    list.Add(item);
                }
                currentIndex++;
            }
            return list;
        }
    }
} 