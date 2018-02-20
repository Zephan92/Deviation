using Assets.Scripts.Interface;
using Assets.Scripts.Utilities.Timers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
	//this class holds a dictionary of timers and timer utilities
	public class TimerManager : MonoBehaviour, ITimerManager
	{
		private Dictionary<string, IUnityTimer> _timerLibary = new Dictionary<string, IUnityTimer>();
		
		//instantiates a new Attack timer and adds it to the timer library
		public void AddTimer(string TimerName, float Cooldown, int playerNumber = -1)
		{
			if (!_timerLibary.ContainsKey(TimerName + playerNumber))
				_timerLibary.Add(TimerName + playerNumber, new ActionTimer(Cooldown));
		}

		//checks to see if specified timer is up
		public bool TimerUp(string TimerName, int playerNumber = -1)
		{
			if (_timerLibary.ContainsKey(TimerName + playerNumber))
				return _timerLibary[TimerName + playerNumber].TimerUp();
			else
				throw new Exception("Timer - " + TimerName + playerNumber + ": Does not exist.");
		}

		//returns remaining time on a specified timer
		public float GetRemainingCooldown(string TimerName, int playerNumber = -1)
		{
			if (_timerLibary.ContainsKey(TimerName + playerNumber))
				return _timerLibary[TimerName + playerNumber].GetRemainingCountdown();
			else
				throw new Exception("Timer - " + TimerName + playerNumber + ": Does not exist.");
		}

		public void StopTimer(string TimerName, int playerNumber = -1)
		{
			if (_timerLibary.ContainsKey(TimerName + playerNumber))
				_timerLibary[TimerName + playerNumber].StopCooldown();
			else
				Debug.LogError("Timer - " + TimerName + playerNumber + ": Does not exist.");
		}

		public void StopTimers()
		{
			foreach (IUnityTimer timer in _timerLibary.Values)
			{
				timer.StopCooldown();
			}
		}

		public void PauseTimer(string TimerName, int playerNumber = -1)
		{
			if (_timerLibary.ContainsKey(TimerName + playerNumber))
				_timerLibary[TimerName + playerNumber].PauseCooldown();
			else
				Debug.LogError("Timer - " + TimerName + playerNumber + ": Does not exist.");
		}


		public void PauseTimers()
		{
			foreach (IUnityTimer timer in _timerLibary.Values)
			{
				timer.PauseCooldown();
			}
		}

		public void UnpauseTimer(string TimerName, int playerNumber = -1)
		{
			if (_timerLibary.ContainsKey(TimerName + playerNumber))
				_timerLibary[TimerName + playerNumber].UnpauseCooldown();
			else
				Debug.LogError("Timer - " + TimerName + playerNumber + ": Does not exist.");
		}


		public void UnpauseTimers()
		{
			foreach (IUnityTimer timer in _timerLibary.Values)
			{
				timer.UnpauseCooldown();
			}
		}

		//restarts a specified timer
		public void RestartTimer(string TimerName, int playerNumber = -1)
		{
			if (_timerLibary.ContainsKey(TimerName + playerNumber))
				_timerLibary[TimerName + playerNumber].RestartCooldown();
			else
				Debug.LogError("Timer - " + TimerName + playerNumber + ": Does not exist.");
		}

		public void RestartTimers()
		{
			foreach (IUnityTimer timer in _timerLibary.Values)
			{
				timer.RestartCooldown();
			}
		}

		//starts a specified timer
		public void StartTimer(string TimerName, int playerNumber = -1)
		{
			if (_timerLibary.ContainsKey(TimerName + playerNumber))
				_timerLibary[TimerName + playerNumber].StartCooldown();
			else
				Debug.LogError("Timer - " + TimerName + playerNumber + ": Does not exist.");
		}

		public void UpdateCountdowns()
		{
			//for each timer, update the remaining time
			foreach (IUnityTimer timer in _timerLibary.Values)
			{
				timer.UpdateCountdown(Time.deltaTime);
			}
		}

		public float GetTimerCooldownLength(string TimerName, int playerNumber = -1)
		{
			if (_timerLibary.ContainsKey(TimerName + playerNumber))
				return _timerLibary[TimerName + playerNumber].GetCooldown();
			else
				Debug.LogError("Timer - " + TimerName + playerNumber + ": Does not exist.");

			return 0;
		}

	}
}
