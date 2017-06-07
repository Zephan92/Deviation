using Assets.Scripts.Enum;

namespace Assets.Scripts.Exchange.NPC
{ 
	public class NPCDecisionState
	{
		public int[] DecisionState;
		public int Threshold;

		public NPCDecisionState(int threshold = 100)
		{
			int decisionCount = System.Enum.GetValues(typeof(Decision)).Length;
			DecisionState = new int[decisionCount];
			DecisionState.Initialize();
			Threshold = threshold;
		}

		public bool DecisionReady(Decision decision)
		{
			int dec = (int)decision;
			if (DecisionState[dec] >= Threshold)
			{
				return true;
			}
			return false;
		}

		public void DecisionAdd(Decision decision, int add)
		{
			DecisionState[(int) decision] += add;
		}

		public void ResetDecision(Decision decision)
		{
			DecisionState[(int) decision] = 0;
		}
	}

	
}
