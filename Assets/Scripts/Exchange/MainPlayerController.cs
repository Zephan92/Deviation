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

		public void CheckInput()
		{
			CheckForMovement();
			CheckForUserAction();
		}

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

		private void ClickPrimaryModule()
		{
			ec.ClickOnButton("ExchangeControls", "CurrentModule");
		}

		private void ClickPrimaryAction()
		{
			ec.ClickOnButton("ExchangeControls", "CurrentAction");
		}


		private void CycleBattlefieldCC()
		{
			_mainPlayer.CycleBattlefieldCC();
			ec.UpdateExchangeControlsDisplay();
		}

		private void CycleBattlefieldCW()
		{
			_mainPlayer.CycleBattlefieldCW();
			ec.UpdateExchangeControlsDisplay();
		}

		private void PrimaryAction()
		{
			_mainPlayer.PrimaryAction();
			ec.UpdateExchangeControlsDisplay();
		}

		private void PrimaryModule()
		{
			_mainPlayer.PrimaryModule();
			ec.UpdateExchangeControlsDisplay();
		}

		private void CycleActionLeft()
		{
			_mainPlayer.EquipedKit.GetCurrentModule().CycleActionLeft();
			ec.UpdateExchangeControlsDisplay();
		}

		private void CycleActionRight()
		{
			_mainPlayer.EquipedKit.GetCurrentModule().CycleActionRight();
			ec.UpdateExchangeControlsDisplay();
		}

		private void CycleModuleLeft()
		{
			_mainPlayer.EquipedKit.CycleModuleLeft();
			ec.UpdateExchangeControlsDisplay();
		}

		private void CycleModuleRight()
		{
			_mainPlayer.EquipedKit.CycleModuleRight();
			ec.UpdateExchangeControlsDisplay();
		}
	}
}
