namespace Assets.Scripts.Exchange.NPC
{
	public enum Decision
	{
		DecisionSpeed = 0,
		Move = 1,
		Module = 2,
		Action = 3,
		CycleAction = 4,
		CycleModule = 5,
	}

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
