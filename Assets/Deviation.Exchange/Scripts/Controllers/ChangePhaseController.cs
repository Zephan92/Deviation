using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
	public interface IChangePhaseController
	{
		bool PhaseStarted { get; set; }
	}

	public class ChangePhaseController : MonoBehaviour, IChangePhaseController
	{
		private ITimerManager tm;
		private IMultiplayerController mc;
		public bool PhaseStarted { get; set; }

		public void Awake()
		{
			PhaseStarted = false;
			tm = GetComponent<TimerManager>();
			mc = FindObjectOfType<MultiplayerController>();
		}

		public void Start()
		{
			tm.AddAttackTimer("ChangePhase", 45.0f);
			tm.StartTimer("ChangePhase");
			PhaseStarted = true;
		}

		public void Update()
		{
			tm.UpdateCountdowns();

			if (tm.GetRemainingCooldown("ChangePhase") <= 0)
			{
				ToNextRound();
			}
		}

		private void ToNextRound()
		{
			if (mc != null)
			{
				mc.StartMultiplayerExchangeInstance();
			}
		}
	}
}
