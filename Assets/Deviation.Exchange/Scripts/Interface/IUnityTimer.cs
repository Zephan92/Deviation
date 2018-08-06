using UnityEngine;

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
		float GetCooldown();
	}

	public interface IUnityTimer2
	{
		float Cooldown { get; set; }
		bool TimerUp { get; }
		float Remaining { get; }
		float Elapsed { get; }
		void Restart(bool resetCooldown = false);
	}

	public class UnityTimer : MonoBehaviour, IUnityTimer2
	{
		public static UnityTimer Get { get { return new GameObject("UnityTimer").AddComponent<UnityTimer>(); } }

		public float Cooldown { get; set; }

		public bool TimerUp
		{
			get { return Elapsed >= Cooldown; }
		}

		public float Remaining => Cooldown - Elapsed;
		public float Elapsed { get; private set; }

		public void FixedUpdate()
		{
			Elapsed += Time.deltaTime;
		}

		public void Restart(bool resetCooldown = false)
		{
			Elapsed = 0f;

			if (resetCooldown)
			{
				Cooldown = 0;
			}
		}
	}
}
