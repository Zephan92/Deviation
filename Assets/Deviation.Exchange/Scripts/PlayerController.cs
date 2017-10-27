using UnityEngine.Networking;
using Assets.Scripts.Utilities;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using UnityEngine;
using Barebones.MasterServer;

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

		if (InputWrapper.GetKeyDown(KeyCode.UpArrow))
		{
			dir = Direction.Up;
		}
		else if (InputWrapper.GetKeyDown(KeyCode.DownArrow))
		{
			dir = Direction.Down;
		}
		else if (InputWrapper.GetKeyDown(KeyCode.LeftArrow))
		{
			dir = Direction.Left;
		}
		else if (InputWrapper.GetKeyDown(KeyCode.RightArrow))
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

		if (InputWrapper.GetKeyDown(KeyCode.Q))
		{
			success = Player.Action(0);
		}
		else if (InputWrapper.GetKeyDown(KeyCode.W))
		{
			success = Player.Action(1);
		}
		else if (InputWrapper.GetKeyDown(KeyCode.E))
		{
			success = Player.Action(2);
		}
		else if (InputWrapper.GetKeyDown(KeyCode.R))
		{
			success = Player.Action(3);
		}
		else if (InputWrapper.GetKeyDown(KeyCode.Escape))
		{
			CmdReset();
		}

		if (success)
		{
		}
	}

	[Command]
	private void CmdReset()
	{
		ec.ResetExchange();
	}

	[ClientRpc]
	public void RpcClientRequest()
	{
		if (!isLocalPlayer)
		{
			return;
		}
		CmdServerResponse(Player.PeerId);
	}

	[Command]
	private void CmdServerResponse(int peerId)
	{
		Debug.LogErrorFormat("Recieved Response from {0}", peerId);
		ec.ServerResponse(peerId);
	}
}

