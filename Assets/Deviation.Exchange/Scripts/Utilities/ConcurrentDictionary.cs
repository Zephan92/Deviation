using System;
using System.Collections.Generic;

public class ConcurrentDictionary<tkey, tvalue>
{
	private Dictionary<tkey, tvalue> dict;

	public tvalue this[tkey key]
	{
		get { lock (this) { return dict[key]; } }
		set { lock (this) { dict[key] = value; } }
	}

	public int Count
	{
		get
		{
			lock (this)
			{
				return dict.Count;
			}
		}
	}

	public bool ContainsKey(tkey item) { lock (this) { return dict.ContainsKey(item); } }

	public ConcurrentDictionary()
	{
		this.dict = new Dictionary<tkey, tvalue>();
	}

	public void Add(tkey key, tvalue val)
	{
		lock (this)
		{
			dict.Add(key, val);
		}
	}

	public void Remove(tkey key)
	{
		lock (this)
		{
			dict.Remove(key);
		}
	}

	public void Clear()
	{
		lock (this)
		{
			dict.Clear();
		}
	}

	public tkey[] GetKeysArray() { lock (this) { tkey[] result = new tkey[dict.Keys.Count]; dict.Keys.CopyTo(result, 0); return result; } }
	public tvalue[] GetValuesArray() { lock (this) { tvalue[] result = new tvalue[dict.Values.Count]; dict.Values.CopyTo(result, 0); return result; } }

}