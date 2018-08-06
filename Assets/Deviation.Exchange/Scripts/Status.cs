using Assets.Scripts.Enum;
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Status : NetworkBehaviour
{
	private Health _health;
	private Mover _mover;
	private IExchangePlayer _player;
	private CoroutineManager cm;

	//private Dictionary<StatusEffect, IEnumerator> statusEffectsDict;
	private IEnumerator _coroutine = null;

	public void Start()
	{
		//statusEffectsDict = new Dictionary<StatusEffect, IEnumerator>();//should use this lol
		cm = FindObjectOfType<CoroutineManager>();
		_health = GetComponent<Health>();
		_mover = GetComponent<Mover>();
		_player = GetComponent<ExchangePlayer>();
	}

	public void ApplyEffect(StatusEffect effect, float timeout, float rate = 0f, int actionNumber = -1)
	{
		switch (effect)
		{
			//health
			case StatusEffect.HealthRate:
				HealthRate(timeout, rate);
				break;
			case StatusEffect.HealBlock:
				HealBlock(timeout);
				break;
			case StatusEffect.DamageBlock:
				DamageBlock(timeout);
				break;
			//case StatusEffect.TODOEndurance:
			//	break;
			//case StatusEffect.TODOShield:
			//	break;
			//case StatusEffect.Invisible:
			//	Invisible(timeout);
			//	break;
			case StatusEffect.Disable:
				Disable(timeout, actionNumber);
				break;
			case StatusEffect.Silence:
				Silence(timeout);
				break;
			
			case StatusEffect.Root:
				Root(timeout);
				break;
			default:
				Debug.LogError("Effect is not implemented yet. Effect: " + effect);
				break;
		}
	}

	private void HealthRate(float timeout, float rate)
	{
		_health.Rate += rate;
		cm.StartCoroutineThread_AfterTimout(HealthRateMethod, new object [] { rate }, timeout, ref _coroutine);
	}

	private void HealthRateMethod(object[] objectArray)
	{
		_health.Rate -= (float) objectArray[0];
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

	private void HealBlock(float timeout)
	{
		_health.HealBlock(true);
		cm.StartCoroutineThread_AfterTimout(HealBlockMethod, timeout, ref _coroutine);
	}

	private void HealBlockMethod()
	{
		_health.HealBlock(false);
	}

	private void DamageBlock(float timeout)
	{
		_health.DamageBlock(true);
		cm.StartCoroutineThread_AfterTimout(DamageBlockMethod, timeout, ref _coroutine);
	}

	private void DamageBlockMethod()
	{
		_health.DamageBlock(false);
	}
	
	private void Disable(float timeout, int actionNumber)
	{
		_player.DisableAction(true, actionNumber);
		cm.StartCoroutineThread_AfterTimout(DisableMethod, new object[] { actionNumber }, timeout, ref _coroutine);
	}

	private void DisableMethod(object [] objectParameters)
	{
		_player.DisableAction(false, (int) objectParameters[0]);
	}

	private void Silence(float timeout)
	{
		_player.DisableAction(true);
		cm.StartCoroutineThread_AfterTimout(SilenceMethod, timeout, ref _coroutine);
	}

	private void SilenceMethod()
	{
		_player.DisableAction(false);
	}
}
