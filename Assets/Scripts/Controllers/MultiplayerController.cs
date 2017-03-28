using Assets.Scripts.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Controllers
{
    public class MultiplayerController : MonoBehaviour, IMultiplayerController
    {
		private static int NUMBER_OF_PLAYERS;
		private static int CURRENT_ROUND;
		private static int NUMBER_OF_ROUNDS;
		private static int[] WINNERS;

		public int NumberOfPlayers { get { return NUMBER_OF_PLAYERS; } set { NUMBER_OF_PLAYERS = value; } }
		public int CurrentRound { get { return CURRENT_ROUND; } set { CURRENT_ROUND = value; } }
		public int NumberOfRounds { get { return NUMBER_OF_ROUNDS; } set { NUMBER_OF_ROUNDS = value; } }
		public int[] Winners { get { return WINNERS; } set { WINNERS = value; } }

		public void Awake()
		{
			CURRENT_ROUND = 0;
			NUMBER_OF_ROUNDS = 3;
			WINNERS = new int[NUMBER_OF_ROUNDS];
			WINNERS.Initialize();
		}

		//instantiates a new multiplayer instance
        public void StartMultiplayerExchangeInstance()
        {
			if (CURRENT_ROUND < NUMBER_OF_ROUNDS)
			{
				CURRENT_ROUND++;
				SceneManager.LoadScene("MultiplayerExchange");
			}
			else
			{
				for (int i = 1; i <= WINNERS.Length; i++)
				{
					Debug.Log("Round " + i + " Winner: " + WINNERS[i-1]);
				}

				Debug.Log("That's it Folks!");
				SceneManager.LoadScene("MultiplayerMenu");
				Destroy(gameObject);
			}
		}
    }
}
