using UnityEngine;
using System.Collections;
using Assets.Scripts.Interface;
using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Utilities;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Exchange.NPC;
using System.Collections.Generic;

namespace Assets.Scripts.Controllers
{
	//this is the multiplayer exchange controller
	public class ExchangeController : MonoBehaviour, IExchangeController
	{
		////Public Static Variables
		//public int NumberOfPlayers { get { return 2; } }
		//public BattlefieldZone MainPlayerFieldNumber { get { return BattlefieldZone.One; } }

		////Unity Objects
		//public GameObject MainPlayerObject { get; set; }
		//public IPlayer[] Players { get; set; }
		//public AudioSource music;
		////Current state
		//public ExchangeState ExchangeState { get; set; }

		////Private Variables
		//private IPlayer _mainPlayer;
		//private GameObject[] _displays;
		//private IPlayer _winner;

		////bools
		//public Dictionary<string, bool> DisplayEnabled { get; set; }

		//private bool _awaitingPlayerInput = false;

		////controllers
		//private MainPlayerController mp;
		//private NPCController npc;
		//private ITimerManager tm;
		//private IMultiplayerController mc;

		//void Awake()
		//{
		//	ExchangeState = ExchangeState.Setup;

		//	npc = FindObjectOfType<NPCController>();
		//	mp = FindObjectOfType<MainPlayerController>();
		//	tm = GetComponent<TimerManager>();
		//	mc = FindObjectOfType<MultiplayerController>();
		//	Players = FindObjectsOfType<Player>();

		//	_displays = GameObject.FindGameObjectsWithTag("Display");
		//	DisplayEnabled = new Dictionary<string, bool>();
		//	foreach (GameObject go in _displays)
		//	{
		//		DisplayEnabled.Add(go.name, false);
		//	}

		//	tm.AddAttackTimer("ExchangeTimer", 60);

		//	foreach (IPlayer player in Players)
		//	{
		//		if (player.IsMainPlayer)
		//		{
		//			_mainPlayer = player;
		//		}
		//	}
		//	music = GetComponent<AudioSource>();
		//	ExchangeState = ExchangeState.PreBattle;
		//}

		//void Update()
		//{
		//	switch (ExchangeState)
		//	{
		//		case ExchangeState.Setup:
		//			ExchangeState = ExchangeState.PreBattle;
		//			music.Play();
		//			break;
		//		case ExchangeState.PreBattle:
		//			if (!_awaitingPlayerInput)
		//			{
		//				_awaitingPlayerInput = true;
		//				ToggleDisplay("BattleStart", DisplayEnabled["BattleStart"]);
		//				DisplayEnabled["BattleStart"] = true;
		//				tm.RestartTimer("ExchangeTimer");
		//			}
		//			break;
		//		case ExchangeState.Start:
		//			if (!DisplayEnabled["ExchangeControls"])
		//			{
		//				ToggleDisplay("ExchangeControls", DisplayEnabled["ExchangeControls"]);
		//				DisplayEnabled["ExchangeControls"] = true;
		//				npc.StartDecisionMaker();
		//			}
		//			ExchangeState = ExchangeState.Battle;
		//			break;
		//		case ExchangeState.Battle:
		//			mp.CheckInput();
		//			tm.UpdateCountdowns();

		//			CheckBattleEnd();
		//			break;
		//		case ExchangeState.End:
		//			ExchangeState = ExchangeState.PostBattle;
		//			break;
		//		case ExchangeState.PostBattle:
		//			ExchangeState = ExchangeState.Teardown;
		//			break;
		//		case ExchangeState.Teardown:
		//			EndRound();
		//			break;
		//		case ExchangeState.Paused:
		//			if (!_awaitingPlayerInput)
		//			{
		//				_awaitingPlayerInput = true;
		//			}
		//			break;
		//		default:
		//			break;
		//	}
		//}

		////check to see if the battle is over
		//private void CheckBattleEnd()
		//{
		//	if (tm.GetRemainingCooldown("ExchangeTimer") <= 0)
		//	{
		//		npc.StopDecisionMaker();
		//		ExchangeState = ExchangeState.End;
		//		if (_mainPlayer.Health > Players[0].Health)
		//		{
		//			_winner = _mainPlayer;
		//		}
		//		else if (_mainPlayer.Health < Players[0].Health)
		//		{
		//			_winner = Players[0];
		//		}
		//		else
		//		{
		//			Debug.Log("Yikes its a draw and I have no idea who to choose");
		//		}
		//	}

		//	if (_mainPlayer.Health <= 0)
		//	{
		//		npc.StopDecisionMaker();
		//		ExchangeState = ExchangeState.End;
		//		_winner = Players[0];
		//	}

		//	if (Players[0].Health <= 0)
		//	{
		//		npc.StopDecisionMaker();
		//		ExchangeState = ExchangeState.End;
		//		_winner = _mainPlayer;
		//	}
		//}

		////turn on/off specified display
		//private void ToggleDisplay(string displayName, bool currentDisplayValue)
		//{
		//	toggleVisibility(GetDisplayObject(displayName), !currentDisplayValue);
		//}

		////turn on/off visibility for a display
		//private void toggleVisibility(GameObject display, bool visibility)
		//{
		//	CanvasGroup _canvasGroup = display.GetComponent<CanvasGroup>();
		//	if (1f > _canvasGroup.alpha)
		//		_canvasGroup.alpha = 1f;
		//	else
		//		_canvasGroup.alpha = 0f;

		//	_canvasGroup.blocksRaycasts = _canvasGroup.interactable = visibility;

		//	foreach (Renderer childRenderer in display.GetComponentsInChildren<Renderer>())
		//		childRenderer.enabled = visibility;
		//}

		////load main menu
		//private void EndRound()
		//{
		//	mc.Winners[mc.CurrentRound - 1] = (int)_winner.BattlefieldZone;
		//	ToChangePhase();
		//}

		//private void BackToMultiplayerMenu()
		//{
		//	SceneManager.LoadScene("MultiplayerMenu");
		//}

		//private void ToChangePhase()
		//{
		//	SceneManager.LoadScene("ChangePhase");
		//}

		//private GameObject GetDisplayObject(string displayName)
		//{
		//	foreach (GameObject display in _displays)
		//	{
		//		if (display.name.Equals(displayName))
		//		{
		//			return display;
		//		}
		//	}
		//	return null;
		//}

		////changes the state to start
		//public void ChangeStateToStart()
		//{
		//	ToggleDisplay("BattleStart", DisplayEnabled["BattleStart"]);
		//	ExchangeState = ExchangeState.Start;
		//	_awaitingPlayerInput = false;
		//}

		////changes the state to Pause
		//public void ChangeStateToPause()
		//{
		//	//ExchangeState = ExchangeState.Paused;
		//	//npc.PauseDecisionMaker();
		//	//ToggleDisplay("Unpause", DisplayEnabled["Unpause"]);
		//	//DisplayEnabled["Unpause"] = true;
		//	//_awaitingPlayerInput = true;
		//}

		////changes the state to Pause
		//public void Unpause()
		//{
		//	npc.UnpauseDecisionMaker();
		//	ToggleDisplay("Unpause", DisplayEnabled["Unpause"]);
		//	DisplayEnabled["Unpause"] = false;
		//	ExchangeState = ExchangeState.Battle;
		//}
	}
}