using Assets.Scripts.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Utilities.Timers
{
	//this is the Action implentation of the IUnityTImer interface
	public class ActionTimer : IUnityTimer
	{
		private float _cooldown;
		private float _remainingCooldown;
		private bool _timerUp;

		public ActionTimer(float Cooldown)
		{
			_timerUp = true;
			_cooldown = Cooldown;
		}

		//returns true if the timer's cooldown is done.
		public bool TimerUp()
		{
			return _timerUp;
		}

		//returns remaining cooldown for the timer
		public float GetRemainingCountdown()
		{
			return _remainingCooldown;
		}

		//updates the remaininig time and if the timer is done, it sets _timerUp to true
		public void UpdateCountdown(float delta)
		{
			if (!_timerUp && _remainingCooldown <= 0)
			{
				_timerUp = true;
				_remainingCooldown = 0;
			}
			else if (!_timerUp)
			{
				_remainingCooldown -= delta;
			}
		}

		//starts a timer cooldown
		public void StartCooldown()
		{
			if (_timerUp)
			{
				_timerUp = false;
				_remainingCooldown = _cooldown;
			}
		}

		//restarts a timer cooldown
		public void RestartCooldown()
		{
			_timerUp = false;
			_remainingCooldown = _cooldown;
		}
	}
}
