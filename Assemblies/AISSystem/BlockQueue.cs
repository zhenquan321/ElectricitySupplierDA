using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Linq.Expressions;

namespace AISSystem
{
    public sealed class BlockQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        private readonly int maxSize;
        public BlockQueue(int maxSize) { this.maxSize = maxSize; }
        public void Enqueue(T item)
        {
            lock (queue)
            {
                while (queue.Count >= maxSize) { Monitor.Wait(queue); }
                queue.Enqueue(item);
                if (queue.Count == 1)
                {
                    // wake up any blocked dequeue                
                    Monitor.PulseAll(queue);
                }
            }
        }

        public T Dequeue()
        {
            lock (queue)
            {
                while (queue.Count == 0) { Monitor.Wait(queue); }
                T item = queue.Dequeue();
                if (queue.Count == maxSize - 1)
                {
                    // wake up any blocked enqueue                
                    Monitor.PulseAll(queue);
                }
                return item;
            }
        }

        //public void Enqueue(IEnumerable<T> items)
        //{
        //    if (items != null && items.Count() > 0)
        //        foreach (T t in items)
        //            Enqueue(t);
        //}

        public int Count { get { return queue.Count; } }

        public int MaxSize { get { return maxSize; } }
        
        public bool Contains(Func<T, bool> predict)
        {
            return queue.Any(x => predict(x));
        }
    } 
}
