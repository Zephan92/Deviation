using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
	public int Current { get { return _current; } set { _current = Mathf.Clamp(value, _min, _max); } }
	public float CurrentPercentage { get { return _current / (float)_max; } }
	public int Min { get { return _min; } set { _min = value; } }
	public int Max { get { return _max; } set { _max = value; } }

	[SyncVar]
	private int _current;
	[SyncVar]
	private int _min;
	[SyncVar]
	private int _max;

	public void Init(int min, int max)
	{
		_min = min;
		_max = max;
		_current = max;
	}

	public void UpdateMax(int increase)
	{
		_max += increase;
		_current += increase;
	}

	public void Add(int add)
	{
		_current = Mathf.Clamp(_current + add, _min, _max);
	}

	public void ReInit()
	{
		_current = _max;
	}
}

