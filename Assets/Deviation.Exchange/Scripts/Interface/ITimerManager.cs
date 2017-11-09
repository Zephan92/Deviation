namespace Assets.Scripts.Interface
{
	public interface ITimerManager
	{
		void AddAttackTimer(string TimerName, float Cooldown, int playerNumber = -1);
		bool TimerUp(string TimerName, int playerNumber = -1);
		float GetRemainingCooldown(string TimerName, int playerNumber = -1);
		void StartTimer(string TimerName, int playerNumber = -1);
		void StopTimer(string TimerName, int playerNumber = -1);
		void StopTimers();
		void PauseTimer(string TimerName, int playerNumber = -1);
		void PauseTimers();
		void UnpauseTimer(string TimerName, int playerNumber = -1);
		void UnpauseTimers();
		void RestartTimer(string TimerName, int playerNumber = -1);
		void UpdateCountdowns();
		float GetTimerCooldownLength(string TimerName, int playerNumber = -1);
	}
}
