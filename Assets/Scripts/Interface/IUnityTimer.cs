namespace Assets.Scripts.Interface
{
	public interface IUnityTimer
	{
		bool TimerUp();
		float GetRemainingCountdown();
		void UpdateCountdown(float delta);
		void StartCooldown();
		void StopCooldown();
		void PauseCooldown();
		void UnpauseCooldown();
		void RestartCooldown();
	}
}
