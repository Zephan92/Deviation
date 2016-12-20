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
	public class TimerManager : MonoBehaviour
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

		//restarts a specified timer
		public void RestartTimer(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				_timerLibary[TimerName].RestartCooldown();
			else
				throw new Exception("Timer - " + TimerName + ": Does not exist.");
		}

		//starts a specified timer
		public void StartTimer(string TimerName)
		{
			if (_timerLibary.ContainsKey(TimerName))
				_timerLibary[TimerName].StartCooldown();
			else
				throw new Exception("Timer - " + TimerName + ": Does not exist.");
		}

		public void Update()
		{
			//for each timer, update the remaining time
			foreach (IUnityTimer timer in _timerLibary.Values)
			{
				timer.UpdateCountdown(Time.deltaTime);
			}
		}
	}
}
