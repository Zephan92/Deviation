using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Deviation.Exchange.Scripts.Utilities
{
	public class ConcurrentQueue<T>
	{
		private Queue<T> queue;

		public ConcurrentQueue()
		{
			this.queue = new Queue<T>();
		}

		public ConcurrentQueue(IEnumerable<T> collection)
		{
			this.queue = new Queue<T>(collection);
		}

		public ConcurrentQueue(int count)
		{
			this.queue = new Queue<T>(count);
		}

		public int Count
		{
			get
			{
				lock (this)
				{
					return queue.Count;
				}
			}
		}

		public T Dequeue()
		{
			lock (this)
			{
				return queue.Dequeue();
			}
		}

		public IEnumerable<T> Dequeue(int batch)
		{
			lock (this)
			{
				int count = queue.Count;
				List<T> retVal = new List<T>();
				for (int i = 0; i < batch && i < count; i++)
				{
					retVal.Add(queue.Dequeue());
				}
				return retVal.AsEnumerable();
			}
		}

		public void Enqueue(T item)
		{
			lock (this)
			{
				queue.Enqueue(item);
			}
		}

		public void Enqueue(IEnumerable<T> items)
		{
			lock (this)
			{
				foreach (T item in items)
				{
					queue.Enqueue(item);
				}
			}
		}

		public void Clear()
		{
			lock (this)
			{
				queue.Clear();
			}
		}
	}
}
