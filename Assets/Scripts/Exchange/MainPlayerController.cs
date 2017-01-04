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
			bool success = false;
			if (Input.IsCycleActionLeftPressed())
			{
				success = MainPlayer.CycleActionLeft();
			}
			else if (Input.IsActionPressed())
			{
				success = MainPlayer.PrimaryAction();
				ExchangeController.ClickOnButton("ExchangeControls", "CurrentAction");
			}
			else if (Input.IsCycleActionRightPressed())
			{
				success = MainPlayer.CycleActionRight();
			}
			else if (Input.IsCycleModuleLeftPressed())
			{
				success = MainPlayer.CycleModuleLeft();
			}
			else if (Input.IsModulePressed())
			{
				success = MainPlayer.PrimaryModule();
				ExchangeController.ClickOnButton("ExchangeControls", "CurrentModule");
			}
			else if (Input.IsCycleModuleRightPressed())
			{
				success = MainPlayer.CycleModuleRight();
			}
			else if (Input.IsPausePressed())
			{
				if (ExchangeController.ExchangeState == ExchangeState.Battle)
					ExchangeController.ChangeStateToPause();
			}

			if (success)
			{
				ExchangeController.UpdateExchangeControlsDisplay();
			}
		}

		////cycle battlefield counter clockwise
		//private void CycleBattlefieldCC()
		//{
		//	MainPlayer.CycleBattlefieldCC();
		//	ExchangeController.UpdateExchangeControlsDisplay();
		//}

		////cycle battlefield clockwise
		//private void CycleBattlefieldCW()
		//{
		//	MainPlayer.CycleBattlefieldCW();
		//	ExchangeController.UpdateExchangeControlsDisplay();
		//}

	}
}
