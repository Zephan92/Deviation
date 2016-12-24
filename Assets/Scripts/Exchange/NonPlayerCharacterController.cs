using Assets.Scripts.Controllers;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Exchange
{
	public class NonPlayerCharacterController : MonoBehaviour
	{
		private IEnumerator _coroutine;
		private Player[] _npcPlayers;
		private ExchangeController ec;

		public void Awake()
		{
			if (ec == null)
			{
				var ecObject = GameObject.FindGameObjectWithTag("ExchangeController");
				ec = ecObject.GetComponent<ExchangeController>();
			}
		}

		public void Start()
		{
			if (_npcPlayers == null)
			{
				_npcPlayers = FindObjectsOfType<Player>();
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


				yield return new WaitForSeconds(.1f);
			}
		}

		//cycle battlefield counter clockwise
		private void CycleBattlefieldCC()
		{
			_npcPlayers[0].CycleBattlefieldCC();
		}

		//cycle battlefield clockwise
		private void CycleBattlefieldCW()
		{
			_npcPlayers[0].CycleBattlefieldCW();
		}

		//primary action
		private void PrimaryAction()
		{
			_npcPlayers[0].PrimaryAction();
			ec.UpdateExchangeControlsDisplay();
		}

		//primary module
		private void PrimaryModule()
		{
			_npcPlayers[0].PrimaryModule();
			ec.UpdateExchangeControlsDisplay();
		}

		//cycle action left
		private void CycleActionLeft()
		{
			_npcPlayers[0].GetCurrentModule().CycleActionLeft();
		}

		//cycle action right
		private void CycleActionRight()
		{
			_npcPlayers[0].GetCurrentModule().CycleActionRight();
		}

		//cycle module left
		private void CycleModuleLeft()
		{
			_npcPlayers[0].EquipedKit.CycleModuleLeft();
		}

		//cycle module right
		private void CycleModuleRight()
		{
			_npcPlayers[0].EquipedKit.CycleModuleRight();
		}
	}
}
