using Assets.Scripts.Controllers;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Utilities;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Exchange.NPC
{
	public class NPCController : MonoBehaviour, INPCController
	{
		public IPlayer[] NPCPlayers { get; set; }
		public NPCDecisionState State { get; set; }
		public ICoroutineManager CoroutineManager {get; set;}
		public IExchangeController ExchangeController { get; set; }

		private IEnumerator _coroutine;

		public void Awake()
		{
			ExchangeController = FindObjectOfType<ExchangeController>();
			State = new NPCDecisionState(100);
			CoroutineManager = FindObjectOfType<CoroutineManager>();
		}

		public void Start()
		{
			FindNPCs();
		}

		public void StartDecisionMaker()
		{
			CoroutineManager.StartCoroutineThread_WhileLoop(MakeDecision, 0.1f, ref _coroutine);
		}

		public void PauseDecisionMaker()
		{
			CoroutineManager.PauseCoroutineThread(ref _coroutine);
		}

		public void UnpauseDecisionMaker()
		{
			CoroutineManager.UnpauseCoroutineThread(ref _coroutine);
		}

		public void StopDecisionMaker()
		{
			CoroutineManager.StopCoroutineThread(ref _coroutine);
		}

		public void MakeDecision()
		{
			ActionDecision();
			MoveDecision();

			if (NPCPlayers[0].MaxHealth / 3 > NPCPlayers[0].Health)
			{
				State.DecisionAdd(Decision.Action, 1);
				State.DecisionAdd(Decision.Move, 2);
			}
		}

		private void ActionDecision()
		{
			Decision decision = Decision.Action;
			if (State.DecisionReady(decision))
			{
				bool success = PrimaryAction(NPCPlayers[0]);
				if (success)
				{
					State.ResetDecision(decision);
					State.DecisionAdd(Decision.CycleAction, Random.Range(15,26));
				}
			}
			else
			{
				State.DecisionAdd(decision, Random.Range(0, 4));
			}
		}

		private void MoveDecision()
		{
			Decision decision = Decision.Move;
			if (State.DecisionReady(decision))
			{
				int range = Random.Range(-550, 550);
				bool success = false;
				if (range >= 300 && range < 500)
				{
					success = NPCPlayers[0].MoveObject(Direction.Down, 1);
				}
				else if (range >= 100 && range < 300)
				{
					success = NPCPlayers[0].MoveObject(Direction.Up, 1);
				}
				else if (range < 100 && range > -200)
				{
					success = NPCPlayers[0].MoveObject(Direction.Left, 1);
				}
				else if (range < -200 && range > -400)
				{
					success = NPCPlayers[0].MoveObject(Direction.Right, 1);
				}
				else
				{
					success = true;
				}

				if (success)
				{
					State.ResetDecision(decision);
				}
			}
			else
			{
				State.DecisionAdd(decision, Random.Range(5, 15));
			}
		}

		//primary action
		private bool PrimaryAction(IPlayer npcPlayer)
		{
			var actionNum = Random.Range(0, 4);
			bool success = npcPlayer.DoAction(actionNum);
			if (success)
			{
			}

			return success;
		}

		private void FindNPCs()
		{
			if (NPCPlayers == null)
			{
				IPlayer[] _players = FindObjectsOfType<Player>();
				NPCPlayers = new IPlayer[_players.Length - 1];
				int counter = 0;
				foreach (IPlayer player in _players)
				{
					if (!player.IsMainPlayer)
					{
						NPCPlayers[counter] = player;
						counter++;
					}
				}
			}
		}
	}
}
