using UnityEngine;
using UnityEngine.Networking;

public class Energy : NetworkBehaviour
{
	public int Current { get { return _current; } set { _current = Mathf.Clamp(value, _min, _max); } }
	public float CurrentPercentage { get { return _current / (float)_max; } }
	public int Min { get { return _min; } set { _min = value; } }
	public int Max { get { return _max; } set { _max = value; } }
	public float Rate { get { return _rate; } set { _rate = value; } }

	[SyncVar]
	private int _current;
	[SyncVar]
	private int _min;
	[SyncVar]
	private int _max;
	[SyncVar]
	private float _rate;
	[SyncVar]
	private float _energy;
	[SyncVar]
	private bool _recoilBlock;
	[SyncVar]
	private bool _regenBlock;

	public void Init(int min, int max, float rate)
	{
		_min = min;
		_max = max;
		_current = max;
		_rate = rate;
		_energy = 0f;
		_recoilBlock = false;
		_regenBlock = false;
	}

	public void UpdateMax(int increase)
	{
		_max += increase;
		_current += increase;
	}

	public void Add(int add)
	{
		int currentMin = _min;
		int currentMax = _max;

		if (_recoilBlock)
		{
			currentMin = _current;
		}

		if (_regenBlock)
		{
			currentMax = _current;
		}

		_current = Mathf.Clamp(_current + add, _min, _max);
		
	}

	public void ReInit()
	{
		_current = _max;
	}

	public void Restore()
	{
		_energy += _max * _rate;
		if (_energy > 1 || _energy < -1)
		{
			var add = (int)_energy;
			Add(add);
			_energy -= add;
		}
	}

	public void RecoilBlock(bool recoilBlockEnabled)
	{
		_recoilBlock = recoilBlockEnabled;
	}

	public void RegenBlock(bool regenBlockEnabled)
	{
		_regenBlock = regenBlockEnabled;
	}
}
