using Assets.Scripts.Controllers;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.Exchange;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Exchange
{

	public class NonPlayerCharacterController : MonoBehaviour
	{
		private IEnumerator _coroutine;
		private IPlayer[] _npcPlayers;
		private ExchangeController ec;
		public int DecisionSpeed;
		public int Move;
		public int Module;
		public int Action;
		public int CycleAction;
		public int CycleModule;

		public void Awake()
		{
			if (ec == null)
			{
				var ecObject = GameObject.FindGameObjectWithTag("ExchangeController");
				ec = ecObject.GetComponent<ExchangeController>();
			}

			DecisionSpeed = 0;
			Move = 0;
			Module = 0;
			Action = 0;
		}

		public void Start()
		{
			if (_npcPlayers == null)
			{
				IPlayer[] _players = FindObjectsOfType<Player>();
				_npcPlayers = new IPlayer[_players.Length - 1];
				int counter = 0;
				foreach (IPlayer player in _players)
				{
					if (!player.IsMainPlayer)
					{
						_npcPlayers[counter] = player;
						counter++;
					}
				}
			}
		}

		public void StartDecisionMaker()
		{
			if (_coroutine == null)
			{
				_coroutine = DecisionMakerCoroutine();
				StartCoroutine(_coroutine);
			}
		}

		public void PauseDecisionMaker()
		{
			
			StopCoroutine(_coroutine);
			Debug.Log("Paused Decision Maker");
		}

		public void UnpauseDecisionMaker()
		{
			Debug.Log("Unpausing Decision Maker");
			StartCoroutine(_coroutine);
		}

		public void StopDecisionMaker()
		{
			StopCoroutine(_coroutine);
			_coroutine = null;
			Debug.Log("Stopped Decision Maker Thread");
		}

		public IEnumerator DecisionMakerCoroutine()
		{
			Debug.Log("Starting Decision Maker Thread");
			for (int i = 0;;i++)
			{
				//DecisionSpeed++;
				if (Move >= 100)
				{
					int range = Random.Range(-500, 500);
					bool success = false;
					if (range >= 250)
					{
						success = _npcPlayers[0].MoveObject(Direction.Down, 1);
					}
					else if(range >= 0 && range < 250)
					{
						success = _npcPlayers[0].MoveObject(Direction.Up, 1);
					}
					else if(range < 0 && range > -250)
					{
						success = _npcPlayers[0].MoveObject(Direction.Left, 1);
					}
					else if(range < -250)
					{
						success = _npcPlayers[0].MoveObject(Direction.Right, 1);
					}

					if (success)
					{
						Move = 0;
					}
				}
				else
				{
					Move += 10;
				}

				Module++;

				if (CycleAction >= 100)
				{
					int Cycle = Random.Range(-500, 500);
					if (Cycle > 0)
					{
						CycleActionRight(_npcPlayers[0]);
					}
					else
					{
						CycleActionLeft(_npcPlayers[0]);
					}
					CycleAction = 0;
				}
				else
				{
					CycleAction += 10;
				}

				if (CycleModule >= 100)
				{
					int Cycle = Random.Range(-500, 500);
					if (Cycle > 0)
					{
						CycleModuleRight(_npcPlayers[0]);
					}
					else
					{
						CycleModuleLeft(_npcPlayers[0]);
					}
					CycleModule = 0;
				}
				else
				{
					CycleModule += 1;
				}

				if (Action >= 100)
				{
					bool success = PrimaryAction(_npcPlayers[0]);
					if (success)
					{
						Action = 0;
						CycleAction += 25;
					}
				}
				else
				{
					Action += 2;
				}

				yield return new WaitForSeconds(0.1f);
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
				ec.UpdateExchangeControlsDisplay();
			}

			return success;
		}

		//primary module
		private void PrimaryModule(IPlayer npcPlayer)
		{
			npcPlayer.PrimaryModule();
			ec.UpdateExchangeControlsDisplay();
		}

		//cycle action left
		private void CycleActionLeft(IPlayer npcPlayer)
		{
			npcPlayer.GetCurrentModule().CycleActionLeft();
		}

		//cycle action right
		private void CycleActionRight(IPlayer npcPlayer)
		{
			npcPlayer.GetCurrentModule().CycleActionRight();
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
	}
}
