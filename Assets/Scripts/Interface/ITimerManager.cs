namespace Assets.Scripts.Interface
{
	public interface ITimerManager
	{
		void AddAttackTimer(string TimerName, float Cooldown);
		bool TimerUp(string TimerName);
		float GetRemainingCooldown(string TimerName);
		void StartTimer(string TimerName);
		void StopTimer(string TimerName);
		void StopTimers();
		void PauseTimer(string TimerName);
		void PauseTimers();
		void UnpauseTimer(string TimerName);
		void UnpauseTimers();
		void RestartTimer(string TimerName);
		void UpdateCountdowns();
	}
}
