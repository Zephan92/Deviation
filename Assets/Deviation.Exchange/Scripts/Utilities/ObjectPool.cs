using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation.Exchange.Scripts.Utilities
{
	public class ObjectPool<T>
	{
		public delegate T CreateObject();
		public delegate T ResetObject(T poolObject);
		public int PoolSize;
		public int PoolCount;

		private CreateObject _createObjectMethod;

		private ConcurrentQueue<T> _pool;

		public ObjectPool(int poolSize, CreateObject createObjectMethod)
		{
			_createObjectMethod = createObjectMethod;
			_pool = new ConcurrentQueue<T>();
			PoolSize = poolSize;
			PoolCount = poolSize;

			for (int i = 0; i < poolSize; i++)
			{
				_pool.Enqueue(_createObjectMethod());
			}
		}

		public ObjectPool(IEnumerable<T> pool, CreateObject createObjectMethod)
		{
			_createObjectMethod = createObjectMethod;
			_pool = new ConcurrentQueue<T>();
			PoolSize = pool.Count();
			PoolCount = PoolSize;

			_pool.Enqueue(pool);
		}

		public T Get()
		{
			if (_pool.Count == 0)
			{
				PoolSize++;
				return _createObjectMethod();
			}
			else
			{
				PoolCount--;
				return _pool.Dequeue();
			}
		}

		public IEnumerable<T> Get(int numObjects)
		{
			for (int i = 0; i < numObjects - _pool.Count ; i++)
			{
				PoolSize++;
				_pool.Enqueue(_createObjectMethod());
			}

			PoolCount -= numObjects;
			return _pool.Dequeue(numObjects);
		}

		public IEnumerable<T> GetAll()
		{
			PoolCount = 0;
			return _pool.Dequeue(PoolCount);
		}

		public void Release(T poolObject)
		{
			PoolCount++;
			_pool.Enqueue(poolObject);
		}

		public void Release(IEnumerable<T> poolObject)
		{
			PoolCount += poolObject.Count();
			_pool.Enqueue(poolObject);
		}
	}
}
