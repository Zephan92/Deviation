using Assets.Scripts.Interface;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Client;
using System.Collections;
using UnityEngine.Networking;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Deviation.Data.Resource;
using System;

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

		private IDeviationClient dc;

		public void Awake()
		{
			CURRENT_ROUND = 0;
			NUMBER_OF_ROUNDS = 3;
			WINNERS = new int[NUMBER_OF_ROUNDS];
			WINNERS.Initialize();
			dc = FindObjectOfType<DeviationClient>();
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

				GetResource();
				Debug.Log("That's it Folks!");
				
				SceneManager.LoadScene("MultiplayerMenu");
				Destroy(gameObject);
			}
		}

		public void GetResource()
		{
			StartCoroutine(AllocateResourcesCoroutine());
		}

		public void GetLootPool()
		{
			StartCoroutine(GetLootPoolFromServer());
		}

		public void OutputResourceBag()
		{
			foreach (string str in dc.currentPlayer.ResourceBag.ToStringArray())
			{
				Debug.Log(str);
			}
		}

		private IEnumerator AllocateResourcesCoroutine()
		{
			UnityWebRequest getreq = UnityWebRequest.Get("http://localhost:50012/api/lootpool/getloot");
			yield return getreq.Send();

			if (getreq.isError)
			{
				Debug.Log("Error: " + getreq.error);
			}
			else
			{
				//Debug.Log("Received " + getreq.downloadHandler.text);
				string lootName = getreq.downloadHandler.text.Replace("\"", "");
				IResource loot = ResourceLibrary.GetResourceInstance(lootName);
				dc.currentPlayer.ResourceBag.AddResource(loot);
				Debug.Log("Adding \"" + lootName + "\" to Resource Bag");
			}
		}

		private IEnumerator GetLootPoolFromServer()
		{
			UnityWebRequest getreq = UnityWebRequest.Get("http://localhost:50012/api/lootpool/getlootpool");
			yield return getreq.Send();

			if (getreq.isError)
			{
				Debug.Log("Error: " + getreq.error);
			}
			else
			{
				//Debug.Log("Received " + getreq.downloadHandler.text);
				string lootpool = getreq.downloadHandler.text;
				Debug.Log(lootpool);
			}
		}
	}
}
