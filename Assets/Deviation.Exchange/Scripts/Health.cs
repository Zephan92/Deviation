using Assets.Deviation.Exchange.Scripts;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
	public int Current { get { return _current; } set { _current = Mathf.Clamp(value, _min, _max); } }
	public float CurrentPercentage { get { return _current / (float)_max; } }
	public int Min { get { return _min; } set { _min = value; } }
	public int Max { get { return _max; } set { _max = value; } }
	public float Rate { get { return _rate; } set { _rate = value; } }
	public Splat Splat { get; set; }
	public PlayerStats PlayerStats {get;set;}

	[SyncVar]
	private int _current;
	[SyncVar]
	private int _min;
	[SyncVar]
	private int _max;
	[SyncVar]
	private bool _damageBlock;
	[SyncVar]
	private bool _healBlock;
	[SyncVar]
	private float _health;
	[SyncVar]
	private float _rate;

	public void Init(int min, int max)
	{
		_min = min;
		_max = max;
		_current = max;
		_damageBlock = false;
		_healBlock = false;
		_rate = 0;
		_health = 0;
		Splat = GetComponent<Splat>();
		PlayerStats = GetComponent<PlayerStats>();
	}

	public void UpdateMax(int increase)
	{
		_max += increase;
		_current += increase;
	}

	public int Add(int add)
	{
		int currentMin = _min;
		int currentMax = _max;

		if (_damageBlock)
		{
			currentMin = _current;
		}

		if (_healBlock)
		{
			currentMax = _current;
		}

		int newCurrent = Mathf.Clamp(_current + add, currentMin, currentMax);
		int difference = newCurrent - _current;
		Splat.AddHealth(difference);
		if (add > 0)
		{
			PlayerStats.TotalHealed += difference;
		}
		else
		{
			if (newCurrent == 0)
			{
				PlayerStats.KnockoutsTaken++;
			}

			PlayerStats.DamageTaken -= difference;
		}

		_current = newCurrent;
		return difference;
	}

	public void ReInit()
	{
		_current = _max;
	}

	public void Restore()
	{
		if (!isServer)
		{
			return;
		}

		_health += _max * _rate;
		if (_health > 2 || _health < -2)
		{
			var add = (int)_health;
			Add(add);
			_health -= add;
		}
	}

	public void DamageBlock(bool damageBlockEnabled)
	{
		_damageBlock = damageBlockEnabled;
	}

	public void HealBlock(bool healBlockEnabled)
	{
		_healBlock = healBlockEnabled;
	}
}

