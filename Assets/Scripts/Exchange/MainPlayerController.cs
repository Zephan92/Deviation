using Assets.Scripts.Controllers;
using Assets.Scripts.Enum;

using UnityEngine;

namespace Assets.Scripts.Exchange
{
	class MainPlayerController : MonoBehaviour
	{
		private Player _mainPlayer;
		private static ExchangeController ec;

		public void Awake()
		{
			if (_mainPlayer == null)
			{
				var mPlayer = GameObject.FindGameObjectWithTag("MainPlayer");
				_mainPlayer = mPlayer.GetComponent<Player>();
			}

			if (ec  == null)
			{
				var ecObject = GameObject.FindGameObjectWithTag("ExchangeController");
				ec = ecObject.GetComponent<ExchangeController>();
			}
		}

		//check user input
		public void CheckInput()
		{
			CheckForMovement();
			CheckForUserAction();
		}

		//check for user movement
		private void CheckForMovement()
		{
			Direction dir = Direction.None;

			if (Input.GetKeyDown(KeyCode.W))
			{
				dir = Direction.Up;
			}
			else if (Input.GetKeyDown(KeyCode.S))
			{
				dir = Direction.Down;
			}
			else if (Input.GetKeyDown(KeyCode.A))
			{
				dir = Direction.Left;
			}
			else if (Input.GetKeyDown(KeyCode.D))
			{
				dir = Direction.Right;
			}

			if (dir != Direction.None)
			{
				_mainPlayer.MoveObject(dir, 1);
			}
		}
		
		//check for user action
		private void CheckForUserAction()
		{
			if (Input.GetKeyDown(KeyCode.U))
			{
				CycleActionLeft();
			}
			else if (Input.GetKeyDown(KeyCode.I))
			{
				PrimaryAction();
				ClickPrimaryAction();
			}
			else if (Input.GetKeyDown(KeyCode.O))
			{
				CycleActionRight();
			}
			else if (Input.GetKeyDown(KeyCode.J))
			{
				CycleModuleLeft();
			}
			else if (Input.GetKeyDown(KeyCode.K))
			{
				PrimaryModule();
				ClickPrimaryModule();
			}
			else if (Input.GetKeyDown(KeyCode.L))
			{
				CycleModuleRight();
			}
		}

		//click on primary module button
		private void ClickPrimaryModule()
		{
			ec.ClickOnButton("ExchangeControls", "CurrentModule");
		}

		//click on primary action button
		private void ClickPrimaryAction()
		{
			ec.ClickOnButton("ExchangeControls", "CurrentAction");
		}

		//cycle battlefield counter clockwise
		private void CycleBattlefieldCC()
		{
			_mainPlayer.CycleBattlefieldCC();
			ec.UpdateExchangeControlsDisplay();
		}

		//cycle battlefield clockwise
		private void CycleBattlefieldCW()
		{
			_mainPlayer.CycleBattlefieldCW();
			ec.UpdateExchangeControlsDisplay();
		}

		//primary action
		private void PrimaryAction()
		{
			_mainPlayer.PrimaryAction();
			ec.UpdateExchangeControlsDisplay();
		}

		//primary module
		private void PrimaryModule()
		{
			_mainPlayer.PrimaryModule();
			ec.UpdateExchangeControlsDisplay();
		}

		//cycle action left
		private void CycleActionLeft()
		{
			_mainPlayer.EquipedKit.GetCurrentModule().CycleActionLeft();
			ec.UpdateExchangeControlsDisplay();
		}

		//cycle action right
		private void CycleActionRight()
		{
			_mainPlayer.EquipedKit.GetCurrentModule().CycleActionRight();
			ec.UpdateExchangeControlsDisplay();
		}

		//cycle module left
		private void CycleModuleLeft()
		{
			_mainPlayer.EquipedKit.CycleModuleLeft();
			ec.UpdateExchangeControlsDisplay();
		}

		//cycle module right
		private void CycleModuleRight()
		{
			_mainPlayer.EquipedKit.CycleModuleRight();
			ec.UpdateExchangeControlsDisplay();
		}
	}
}
