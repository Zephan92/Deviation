﻿using Assets.Scripts.Controllers;
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
			ModuleDecision();
			CycleModuleDecision();
			CycleActionDecision();

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
				int range = Random.Range(-500, 500);
				bool success = false;
				if (range >= 300)
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
				else if (range < -200)
				{
					success = NPCPlayers[0].MoveObject(Direction.Right, 1);
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

		private void ModuleDecision()
		{
			Decision decision = Decision.Module;
			if (State.DecisionReady(decision))
			{
				bool success = PrimaryModule(NPCPlayers[0]);
				if (success)
				{
					State.ResetDecision(decision);
					State.DecisionAdd(Decision.CycleModule, Random.Range(15, 26));
				}
			}
			else
			{
				State.DecisionAdd(decision, Random.Range(0, 3));
			}
		}

		private void CycleModuleDecision()
		{
			Decision decision = Decision.CycleModule;
			if (State.DecisionReady(decision))
			{
				int Cycle = Random.Range(-500, 500);
				if (Cycle > 0)
				{
					CycleModuleRight(NPCPlayers[0]);
				}
				else
				{
					CycleModuleLeft(NPCPlayers[0]);
				}
				State.ResetDecision(decision);
			}
			else
			{
				State.DecisionAdd(decision, Random.Range(0, 3));
			}
		}

		private void CycleActionDecision()
		{
			Decision decision = Decision.CycleAction;

			if (State.DecisionReady(decision))
			{
				int Cycle = Random.Range(-500, 500);
				if (Cycle > 0)
				{
					CycleActionRight(NPCPlayers[0]);
				}
				else
				{
					CycleActionLeft(NPCPlayers[0]);
				}

				State.ResetDecision(decision);
			}
			else
			{
				State.DecisionAdd(decision, Random.Range(8, 12));
			}
		}

		//cycle battlefield counter clockwise
		private void CycleBattlefieldCC(IPlayer npcPlayer)
		{
			npcPlayer.CycleBattlefieldCC();
		}

		//cycle battlefield clockwise
		private void CycleBattlefieldCW(IPlayer npcPlayer)
		{
			npcPlayer.CycleBattlefieldCW();
		}

		//primary action
		private bool PrimaryAction(IPlayer npcPlayer)
		{
			bool success = npcPlayer.PrimaryAction();
			if (success)
			{
				ExchangeController.UpdateExchangeControlsDisplay();
			}

			return success;
		}

		//primary module
		private bool PrimaryModule(IPlayer npcPlayer)
		{
			bool success = npcPlayer.PrimaryModule();

			if (success)
			{
				ExchangeController.UpdateExchangeControlsDisplay();
			}

			return success;
		}

		//cycle action left
		private void CycleActionLeft(IPlayer npcPlayer)
		{
			npcPlayer.CurrentModule.CycleActionLeft();
		}

		//cycle action right
		private void CycleActionRight(IPlayer npcPlayer)
		{
			npcPlayer.CurrentModule.CycleActionRight();
		}

		//cycle module left
		private void CycleModuleLeft(IPlayer npcPlayer)
		{
			npcPlayer.EquipedKit.CycleModuleLeft();
		}

		//cycle module right
		private void CycleModuleRight(IPlayer npcPlayer)
		{
			npcPlayer.EquipedKit.CycleModuleRight();
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
