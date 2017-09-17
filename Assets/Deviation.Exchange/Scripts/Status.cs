using Assets.Scripts.Enum;
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Status : NetworkBehaviour
{
	private Energy _energy;
	private Health _health;
	private Mover _mover;

	private CoroutineManager cm;

	private Dictionary<StatusEffect, IEnumerator> statusEffectsDict;
	private IEnumerator _coroutine = null;

	public void Start()
	{
		statusEffectsDict = new Dictionary<StatusEffect, IEnumerator>();
		cm = FindObjectOfType<CoroutineManager>();
		_energy = GetComponent<Energy>();
		_health = GetComponent<Health>();
		_mover = GetComponent<Mover>();
	}

	public void ApplyEffect(StatusEffect effect, float timeout, float rate = 0f)
	{
		switch (effect)
		{
			case StatusEffect.Burn:
				Burn(timeout, rate);
				break;
			case StatusEffect.Root:
				Root(timeout);
				break;
			default:
				Debug.LogError("Effect is not implemented yet. Effect: " + effect);
				break;
		}
	}

	private void Burn(float timeout, float rate)
	{
		_health.Rate = rate;
		cm.StartCoroutineThread_AfterTimout(BurnMethod, timeout, ref _coroutine);
	}

	private void BurnMethod()
	{
		_health.Rate = 0f;
	}

	private void Root(float timeout)
	{
		_mover.SetRoot(true);
		cm.StartCoroutineThread_AfterTimout(RootMethod, timeout, ref _coroutine);
	}

	private void RootMethod()
	{
		_mover.SetRoot(false);
	}
}
