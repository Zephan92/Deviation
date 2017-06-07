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
			
			if (Input.IsAction_Q_Pressed())
			{
				success = MainPlayer.DoAction(0);
			}
			else if (Input.IsAction_W_Pressed())
			{
				success = MainPlayer.DoAction(1);
			}
			else if (Input.IsAction_E_Pressed())
			{
				success = MainPlayer.DoAction(2);
			}
			else if (Input.IsAction_R_Pressed())
			{
				success = MainPlayer.DoAction(3);
			}
			else if (Input.IsPausePressed())
			{
				//if (ExchangeController.ExchangeState == ExchangeState.Battle)
				//	ExchangeController.ChangeStateToPause();
			}

			if (success)
			{
			}
		}
	}
}
