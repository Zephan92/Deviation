using Assets.Scripts.Interface;
using Assets.Scripts.Utilities.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
	//this class holds a dictionary of timers and timer utilities
	public class TimerManager : MonoBehaviour, ITimerManager
	{
		private Dictionary<string, IUnityTimer> _timerLibary = new Dictionary<string, IUnityTimer>();

		//instantiates a new Attack timer and adds it to the timer library
		public void AddAttackTimer(string TimerName, float Cooldown)
		{
			if (!_timerLibary.ContainsKey(TimerName))
				_timerLibary.Add(TimerName, new ActionTimer(Cooldown));
		}

		//checks to see if specified timer is up
		public bool TimerUp(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				return _timerLibary[TimerName].TimerUp();
			else
				throw new Exception("Timer - " + TimerName + ": Does not exist.");
		}

		//returns remaining time on a specified timer
		public float GetRemainingCooldown(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				return _timerLibary[TimerName].GetRemainingCountdown();
			else
				throw new Exception("Timer - " + TimerName + ": Does not exist.");
		}

		public void StopTimer(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				_timerLibary[TimerName].StopCooldown();
			else
				Debug.LogError("Timer - " + TimerName + ": Does not exist.");
		}

		public void StopTimers()
		{
			foreach (IUnityTimer timer in _timerLibary.Values)
			{
				timer.StopCooldown();
			}
		}

		public void PauseTimer(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				_timerLibary[TimerName].PauseCooldown();
			else
				Debug.LogError("Timer - " + TimerName + ": Does not exist.");
		}


		public void PauseTimers()
		{
			foreach (IUnityTimer timer in _timerLibary.Values)
			{
				timer.PauseCooldown();
			}
		}

		public void UnpauseTimer(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				_timerLibary[TimerName].UnpauseCooldown();
			else
				Debug.LogError("Timer - " + TimerName + ": Does not exist.");
		}


		public void UnpauseTimers()
		{
			foreach (IUnityTimer timer in _timerLibary.Values)
			{
				timer.UnpauseCooldown();
			}
		}

		//restarts a specified timer
		public void RestartTimer(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				_timerLibary[TimerName].RestartCooldown();
			else
				Debug.LogError("Timer - " + TimerName + ": Does not exist.");
		}

		public void RestartTimers()
		{
			foreach (IUnityTimer timer in _timerLibary.Values)
			{
				timer.RestartCooldown();
			}
		}

		//starts a specified timer
		public void StartTimer(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				_timerLibary[TimerName].StartCooldown();
			else
				Debug.LogError("Timer - " + TimerName + ": Does not exist.");
		}

		public void UpdateCountdowns()
		{
			//for each timer, update the remaining time
			foreach (IUnityTimer timer in _timerLibary.Values)
			{
				timer.UpdateCountdown(Time.deltaTime);
			}
		}
	}
}
