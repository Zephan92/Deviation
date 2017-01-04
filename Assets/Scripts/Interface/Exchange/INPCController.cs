using Assets.Scripts.Exchange.NPC;

namespace Assets.Scripts.Interface.Exchange
{
	public interface INPCController
	{
		IPlayer[] NPCPlayers { get; set; }
		NPCDecisionState State { get; set; }
		void StartDecisionMaker();
		void PauseDecisionMaker();
		void UnpauseDecisionMaker();
		void StopDecisionMaker();
	}
}
