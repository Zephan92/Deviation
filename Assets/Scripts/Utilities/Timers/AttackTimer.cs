using System;
using Assets.Scripts.Interface;

namespace Assets.Scripts.Utilities.Timers
{
	//this is the Action implentation of the IUnityTImer interface
	public class ActionTimer : IUnityTimer
	{
		private float _cooldown;
		private float _remainingCooldown;
		private bool _timerUp;
		private bool _timerPaused;

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

		public bool TimerPaused()
		{
			return _timerPaused;
		}

		//returns remaining cooldown for the timer
		public float GetRemainingCountdown()
		{
			return _remainingCooldown;
		}

		//updates the remaininig time and if the timer is done, it sets _timerUp to true
		public void UpdateCountdown(float delta)
		{
			if (_timerPaused)
			{
				return;
			}

			if (!_timerUp && _remainingCooldown <= 0)
			{
				StopCooldown();
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

		public void StopCooldown()
		{
			_timerUp = true;
			_remainingCooldown = 0;
		}

		public void PauseCooldown()
		{
			_timerPaused = true;
		}

		public void UnpauseCooldown()
		{
			_timerPaused = false;
		}
	}
}
