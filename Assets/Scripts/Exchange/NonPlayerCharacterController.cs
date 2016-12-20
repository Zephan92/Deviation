using Assets.Scripts.Controllers;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Exchange
{
	public class NonPlayerCharacterController : MonoBehaviour
	{
		private IEnumerator _coroutine;
		private Player _npcPlayer;
		private ExchangeController ec;

		public void Awake()
		{
			if (ec == null)
			{
				var ecObject = GameObject.FindGameObjectWithTag("ExchangeController");
				ec = ecObject.GetComponent<ExchangeController>();
			}

			if (_npcPlayer == null)
			{
				var _npcObject = GameObject.FindGameObjectsWithTag("Player");
				_npcPlayer = _npcObject[0].GetComponent<Player>();
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
			_npcPlayer.CycleBattlefieldCC();
		}

		//cycle battlefield clockwise
		private void CycleBattlefieldCW()
		{
			_npcPlayer.CycleBattlefieldCW();
		}

		//primary action
		private void PrimaryAction()
		{
			_npcPlayer.PrimaryAction();
			ec.UpdateExchangeControlsDisplay();
		}

		//primary module
		private void PrimaryModule()
		{
			_npcPlayer.PrimaryModule();
			ec.UpdateExchangeControlsDisplay();
		}

		//cycle action left
		private void CycleActionLeft()
		{
			_npcPlayer.EquipedKit.GetCurrentModule().CycleActionLeft();
		}

		//cycle action right
		private void CycleActionRight()
		{
			_npcPlayer.EquipedKit.GetCurrentModule().CycleActionRight();
		}

		//cycle module left
		private void CycleModuleLeft()
		{
			_npcPlayer.EquipedKit.CycleModuleLeft();
		}

		//cycle module right
		private void CycleModuleRight()
		{
			_npcPlayer.EquipedKit.CycleModuleRight();
		}
	}
}
