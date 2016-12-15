using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class TimerManager : MonoBehaviour
	{	 
		private Dictionary<string, UnityTimer> _timerLibary = new Dictionary<string, UnityTimer>();

		public void AddAttackTimer(string TimerName, float Cooldown)
		{
			if (!_timerLibary.ContainsKey(TimerName))
				_timerLibary.Add(TimerName, new AttackTimer(Cooldown));
		}

		public bool IsReady(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				return _timerLibary[TimerName].IsReady();
			else
				throw new Exception("Timer - " + TimerName + ": Does not exist.");
		}

		public float GetRemainingCooldown(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				return _timerLibary[TimerName].GetRemainingCountdown();
			else
				throw new Exception("Timer - " + TimerName + ": Does not exist.");
		}

		public void RestartTimer(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				_timerLibary[TimerName].RestartCooldown();
			else
				throw new Exception("Timer - " + TimerName + ": Does not exist.");
		}

		public void StartTimer(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				_timerLibary[TimerName].StartCooldown();
			else
				throw new Exception("Timer - " + TimerName + ": Does not exist.");
		}

		public void Update()
		{
			foreach (UnityTimer timer in _timerLibary.Values)
			{
				timer.UpdateCountdown(Time.deltaTime);
			}
		}
	}

	public interface UnityTimer
	{
		bool IsReady();
		float GetRemainingCountdown();
		void UpdateCountdown(float delta);
		void StartCooldown();
		void RestartCooldown();
	}

	public class AttackTimer : UnityTimer
	{
		private float _cooldown;
		private float _remainingCooldown;
		private bool _ready;

		public AttackTimer(float Cooldown)
		{
			_ready = true;
			_cooldown = Cooldown;
		}

		public bool IsReady()
		{
			return _ready;
		}

		public float GetRemainingCountdown()
		{
			return _remainingCooldown;
		}

		public void UpdateCountdown(float delta)
		{
			if (!_ready && _remainingCooldown <= 0)
			{
				_ready = true;
				_remainingCooldown -= 0;
			}
			else if(!_ready)
			{
				_remainingCooldown -= delta;
			}
		}

		public void StartCooldown()
		{
			if (_ready)
			{
				_ready = false;
				_remainingCooldown = _cooldown;
			}
		}

		public void RestartCooldown()
		{
			_ready = false;
			_remainingCooldown = _cooldown;
		}
	}
}
