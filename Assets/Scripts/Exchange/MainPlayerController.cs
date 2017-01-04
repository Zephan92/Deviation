using Assets.Scripts.Controllers;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Utilities;
using UnityEngine;

namespace Assets.Scripts.Exchange
{
	public class MainPlayerController : MonoBehaviour, IMainPlayerController
	{
		public IInput Input { get; set; }
		public IPlayer MainPlayer { get; set; }
		public IExchangeController ExchangeController { get; set; }

		public void Awake()
		{
			Input = new InputWrapper();
			ExchangeController = FindObjectOfType<ExchangeController>();
		}

		public void Start()
		{
			var mPlayer = GameObject.FindGameObjectWithTag("MainPlayer");
			MainPlayer = mPlayer.GetComponent<Player>();
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

			if (Input.IsUpPressed())
			{
				dir = Direction.Up;
			}
			else if (Input.IsDownPressed())
			{
				dir = Direction.Down;
			}
			else if (Input.IsLeftPressed())
			{
				dir = Direction.Left;
			}
			else if (Input.IsRightPressed())
			{
				dir = Direction.Right;
			}

			if (dir != Direction.None)
			{
				MainPlayer.MoveObject(dir, 1);
			}
		}
		
		//check for user action
		private void CheckForUserAction()
		{
			if (Input.IsCycleActionLeftPressed())
			{
				CycleActionLeft();
			}
			else if (Input.IsActionPressed())
			{
				PrimaryAction();
				ClickPrimaryAction();
			}
			else if (Input.IsCycleActionRightPressed())
			{
				CycleActionRight();
			}
			else if (Input.IsCycleModuleLeftPressed())
			{
				CycleModuleLeft();
			}
			else if (Input.IsModulePressed())
			{
				PrimaryModule();
				ClickPrimaryModule();
			}
			else if (Input.IsCycleModuleRightPressed())
			{
				CycleModuleRight();
			}
			else if (Input.IsPausePressed())
			{
				if (ExchangeController.ExchangeState == ExchangeState.Battle)
					ExchangeController.ChangeStateToPause();
			}
		}

		//click on primary module button
		private void ClickPrimaryModule()
		{
			ExchangeController.ClickOnButton("ExchangeControls", "CurrentModule");
		}

		//click on primary action button
		private void ClickPrimaryAction()
		{
			ExchangeController.ClickOnButton("ExchangeControls", "CurrentAction");
		}

		//cycle battlefield counter clockwise
		private void CycleBattlefieldCC()
		{
			MainPlayer.CycleBattlefieldCC();
			ExchangeController.UpdateExchangeControlsDisplay();
		}

		//cycle battlefield clockwise
		private void CycleBattlefieldCW()
		{
			MainPlayer.CycleBattlefieldCW();
			ExchangeController.UpdateExchangeControlsDisplay();
		}

		//primary action
		private void PrimaryAction()
		{
			MainPlayer.PrimaryAction();
			ExchangeController.UpdateExchangeControlsDisplay();
		}

		//primary module
		private void PrimaryModule()
		{
			MainPlayer.PrimaryModule();
			ExchangeController.UpdateExchangeControlsDisplay();
		}

		//cycle action left
		private void CycleActionLeft()
		{
			MainPlayer.CurrentModule.CycleActionLeft();
			ExchangeController.UpdateExchangeControlsDisplay();
		}

		//cycle action right
		private void CycleActionRight()
		{
			MainPlayer.CurrentModule.CycleActionRight();
			ExchangeController.UpdateExchangeControlsDisplay();
		}

		//cycle module left
		private void CycleModuleLeft()
		{
			MainPlayer.CycleModuleLeft();
			ExchangeController.UpdateExchangeControlsDisplay();
		}

		//cycle module right
		private void CycleModuleRight()
		{
			MainPlayer.CycleModuleRight();
			ExchangeController.UpdateExchangeControlsDisplay();
		}
	}
}
