using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public class JList<T> : List<T>
    {
        public JList() : base() { }
        public JList(int capacity) : base(capacity) { }
        public JList(IEnumerable<T> collection) : base(collection) { }

        public new T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return base[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                base[index] = value;
            }
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (index + count > Count)
            {
                throw new ArgumentException("Invalid range");
            }

            for (int i = 0; i < count; i++)
            {
                RemoveAt(index);
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            base.Clear();
        }
    }
} 