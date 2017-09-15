using UnityEngine.Networking;
using Assets.Scripts.Utilities;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
	public IInput InputWrapper { get; set; }
	public IExchangePlayer Player { get; set; }
	public IExchangeController1v1 ec { get; set; }

	public void Awake()
	{
		InputWrapper = new InputWrapper();
		if (Player == null)
		{
			Player = GetComponent<ExchangePlayer>();
		}

		if (ec == null)
		{
			ec = FindObjectOfType<ExchangeController1v1>();
		}
	}

	public void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		CheckInput();
	}

	//check user input
	public void CheckInput()
	{
		switch (ec.ExchangeState)
		{
			case ExchangeState.Battle:
				CheckForMovement();
				break;
		}
		CheckForUserAction();
	}

	//check for user movement
	private void CheckForMovement()
	{
		Direction dir = Direction.None;

		if (InputWrapper.IsKeyPressed(KeyCode.UpArrow))
		{
			dir = Direction.Up;
		}
		else if (InputWrapper.IsKeyPressed(KeyCode.DownArrow))
		{
			dir = Direction.Down;
		}
		else if (InputWrapper.IsKeyPressed(KeyCode.LeftArrow))
		{
			dir = Direction.Left;
		}
		else if (InputWrapper.IsKeyPressed(KeyCode.RightArrow))
		{
			dir = Direction.Right;
		}

		if (dir != Direction.None)
		{
			Player.Mover.Move(dir, 1);
		}
	}

	private void CheckForUserAction()
	{
		bool success = false;

		if (InputWrapper.IsKeyPressed(KeyCode.Q))
		{
			success = Player.Action(0);
		}
		else if (InputWrapper.IsKeyPressed(KeyCode.W))
		{
			success = Player.Action(1);
		}
		else if (InputWrapper.IsKeyPressed(KeyCode.E))
		{
			success = Player.Action(2);
		}
		else if (InputWrapper.IsKeyPressed(KeyCode.R))
		{
			success = Player.Action(3);
		}

		if (success)
		{
		}
	}
}
