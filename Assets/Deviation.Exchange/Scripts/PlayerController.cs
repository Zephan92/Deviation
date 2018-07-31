using UnityEngine.Networking;
using Assets.Scripts.Utilities;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using UnityEngine;
using Barebones.MasterServer;
using UnityEngine.UI;
using Assets.Deviation.Exchange.Scripts.Display;

public class PlayerController : NetworkBehaviour
{
	public Transform ExchangeCanvas;
	public Sprite WinScreen;
	public Sprite LoseScreen;

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
			ec.OnExchangeStateChange += ExchangeStateChange;
		}

		WinScreen = Resources.Load<Sprite>("Splash/Win");
		LoseScreen = Resources.Load<Sprite>("Splash/Defeat");
		ExchangeCanvas = GameObject.Find("ExchangeCanvas").transform;
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

	private void ExchangeStateChange(ExchangeState value)
	{
		switch (value)
		{
			case ExchangeState.End:
				DisplaySplash();
				break;
		}
	}

	private void DisplaySplash()
	{
		if (isLocalPlayer)
		{
			Sprite splashScreen = LoseScreen;
			IExchangePlayer winner = ec.GetRoundWinner();

			if (winner == null)
			{
				//draw screen
				splashScreen = LoseScreen;
			}
			else if (winner.PlayerId == Player.PlayerId)
			{
				splashScreen = WinScreen;
			}

			StartCoroutine(FadeImage.Fade(ExchangeCanvas, splashScreen, 1.0f, 5.0f));
		}
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
		if (InputWrapper.GetKeyDown(KeyCode.Q))
		{
			Player.Action(0);
		}
		else if (InputWrapper.GetKeyDown(KeyCode.W))
		{
			Player.Action(1);
		}
		else if (InputWrapper.GetKeyDown(KeyCode.E))
		{
			Player.Action(2);
		}
		else if (InputWrapper.GetKeyDown(KeyCode.R))
		{
			Player.Action(3);
		}
		else if (InputWrapper.GetKeyDown(KeyCode.Escape))
		{
			CmdReset();
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

		Debug.LogErrorFormat("Recieved Request from Server");

		CmdServerResponse(Player.PeerId);
	}

	[Command]
	private void CmdServerResponse(int peerId)
	{
		//Debug.LogErrorFormat("Recieved Response from {0}", peerId);
		ec.ServerResponse(peerId);
	}
}

