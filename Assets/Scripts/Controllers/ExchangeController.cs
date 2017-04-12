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
		//Public Static Variables
		public int NumberOfPlayers { get { return 2; } }
		public Battlefield MainPlayerFieldNumber { get { return Battlefield.One; } }

		//Unity Objects
		public GameObject MainPlayerObject { get; set; }
		public IPlayer[] Players { get; set; }

		//Current state
		public ExchangeState ExchangeState { get; set; }

		//Private Variables
		private IPlayer _mainPlayer;
		private GameObject[] _displays;
		private IPlayer _winner;

		//bools
		public Dictionary<string, bool> DisplayEnabled { get; set; }

		private bool _awaitingPlayerInput = false;

		//controllers
		private MainPlayerController mp;
		private NPCController npc;
		private ITimerManager tm;
		private IMultiplayerController mc;

		void Awake()
		{
			ExchangeState = ExchangeState.Setup;

			npc = FindObjectOfType<NPCController>();
			mp = FindObjectOfType<MainPlayerController>();
			tm = GetComponent<TimerManager>();
			mc = FindObjectOfType<MultiplayerController>();
			_displays = GameObject.FindGameObjectsWithTag("Display");
			DisplayEnabled = new Dictionary<string, bool>();
			foreach (GameObject go in _displays)
			{
				DisplayEnabled.Add(go.name, false);
			}
			ExchangeState = ExchangeState.PreBattle;
		}

		public void Start()
		{
			Players = FindObjectsOfType<Player>();
			foreach (IPlayer player in Players)
			{
				if (player.IsMainPlayer)
				{
					_mainPlayer = player;
				}
			}
		}

		void Update()
		{
			switch (ExchangeState)
			{
				case ExchangeState.Setup:
					ExchangeState = ExchangeState.PreBattle;
					break;
				case ExchangeState.PreBattle:
					if (!_awaitingPlayerInput)
					{
						_awaitingPlayerInput = true;
						ToggleDisplay("BattleStart", DisplayEnabled["BattleStart"]);
						SelectButton("BattleStart", "Start");
						DisplayEnabled["BattleStart"] = true;
					}
					break;
				case ExchangeState.Start:
					if (!DisplayEnabled["ExchangeControls"])
					{
						ToggleDisplay("ExchangeControls", DisplayEnabled["ExchangeControls"]);
						DisplayEnabled["ExchangeControls"] = true;
						UpdateExchangeControlsDisplay();
						npc.StartDecisionMaker();
					}
					ExchangeState = ExchangeState.Battle;
					break;
				case ExchangeState.Battle:
					mp.CheckInput();
					tm.UpdateCountdowns();

					CheckBattleEnd();
					break;
				case ExchangeState.End:
					ExchangeState = ExchangeState.PostBattle;
					break;
				case ExchangeState.PostBattle:
					ExchangeState = ExchangeState.Teardown;
					break;
				case ExchangeState.Teardown:
					EndRound();
					break;
				case ExchangeState.Paused:
					if (!_awaitingPlayerInput)
					{
						_awaitingPlayerInput = true;
					}
					break;
				default:
					break;
			}
		}

		//check to see if the battle is over
		private void CheckBattleEnd()
		{
			if (_mainPlayer.Health <= 0)
			{
				npc.StopDecisionMaker();
				ExchangeState = ExchangeState.End;
				_winner = Players[0];
			}

			if (Players[0].Health <= 0)
			{
				npc.StopDecisionMaker();
				ExchangeState = ExchangeState.End;
				_winner = _mainPlayer;
			}
		}

		//select a button on the display
		private void SelectButton(string UIGroupName, string buttonName)
		{
			GetButtonFromUIGroup(GetDisplayObject(UIGroupName), buttonName).GetComponent<Selectable>().Select();
		}

		//turn on/off specified display
		private void ToggleDisplay(string displayName, bool currentDisplayValue)
		{
			toggleVisibility(GetDisplayObject(displayName), !currentDisplayValue);
		}

		//turn on/off visibility for a display
		private void toggleVisibility(GameObject display, bool visibility)
		{
			CanvasGroup _canvasGroup = display.GetComponent<CanvasGroup>();
			if (1f > _canvasGroup.alpha)
				_canvasGroup.alpha = 1f;
			else
				_canvasGroup.alpha = 0f;

			_canvasGroup.blocksRaycasts = _canvasGroup.interactable = visibility;

			foreach (Renderer childRenderer in display.GetComponentsInChildren<Renderer>())
				childRenderer.enabled = visibility;
		}

		//load main menu
		private void EndRound()
		{
			mc.Winners[mc.CurrentRound - 1] = (int)_winner.Battlefield;
			mc.StartMultiplayerExchangeInstance();
		}

		private void BackToMultiplayerMenu()
		{
			SceneManager.LoadScene("MultiplayerMenu");
		}

		//returns the specified display
		private GameObject GetDisplayObject(string displayName)
		{
			foreach (GameObject display in _displays)
			{
				if (display.name.Equals(displayName))
				{
					return display;
				}
			}
			return null;
		}

		//returns the specified button
		private Button GetButtonFromUIGroup(GameObject UIGroup, string buttonName)
		{
			foreach (Button button in UIGroup.GetComponentsInChildren<Button>())
			{
				if (button.name.Equals(buttonName))
				{
					return button;
				}
			}
			return null;
		}

		//returns the specified text
		private Text GetTextFromPanelUIGroup(GameObject UIGroup, string panelName)
		{
			foreach (Text text in UIGroup.GetComponentsInChildren<Text>())
			{
				if (text.name.Equals(panelName))
				{
					return text;
				}
			}
			return null;
		}

		//click on the specified button
		public void ClickOnButton(string groupUIName, string buttonName)
		{
			GameObject ExchangeControlsUIGroup = GetDisplayObject(groupUIName);
			Button button = GetButtonFromUIGroup(ExchangeControlsUIGroup, buttonName);
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
			ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
			StartCoroutine(UnClickPointerCoroutine(button, pointer));
		}

		//coroutine to unclick point on button
		private IEnumerator UnClickPointerCoroutine(Button button, PointerEventData pointer)
		{
			yield return new WaitForSeconds(0.1f);
			ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
			ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerExitHandler);
		}

		//changes the state to start
		public void ChangeStateToStart()
		{
			ToggleDisplay("BattleStart", DisplayEnabled["BattleStart"]);
			ExchangeState = ExchangeState.Start;
			_awaitingPlayerInput = false;
		}

		//changes the state to Pause
		public void ChangeStateToPause()
		{
			ExchangeState = ExchangeState.Paused;
			npc.PauseDecisionMaker();
			ToggleDisplay("Unpause", DisplayEnabled["Unpause"]);
			DisplayEnabled["Unpause"] = true;
			SelectButton("Unpause", "Unpause");
			_awaitingPlayerInput = true;
		}

		//changes the state to Pause
		public void Unpause()
		{
			npc.UnpauseDecisionMaker();
			ToggleDisplay("Unpause", DisplayEnabled["Unpause"]);
			DisplayEnabled["Unpause"] = false;
			ExchangeState = ExchangeState.Battle;
		}

		//updates the exchange controls to the latest
		public void UpdateExchangeControlsDisplay()
		{
			var ExchangeControls = GetDisplayObject("ExchangeControls");
			var button = GetButtonFromUIGroup(ExchangeControls, "CurrentModule");
			UpdateButtonColor(button, _mainPlayer.CurrentModule.ModuleTexture);
			UpdateButtonText(button, _mainPlayer.CurrentModule.Name);

			button = GetButtonFromUIGroup(ExchangeControls, "NextModule");
			UpdateButtonColor(button, _mainPlayer.CurrentModule.GetRightModule().ModuleTexture);
			UpdateButtonText(button, _mainPlayer.CurrentModule.GetRightModule().Name);

			button = GetButtonFromUIGroup(ExchangeControls, "PreviousModule");
			UpdateButtonColor(button, _mainPlayer.CurrentModule.GetLeftModule().ModuleTexture);
			UpdateButtonText(button, "");

			button = GetButtonFromUIGroup(ExchangeControls, "CurrentAction");
			UpdateButtonColor(button, _mainPlayer.CurrentModule.GetCurrentAction().ActionTexture);
			UpdateButtonText(button, _mainPlayer.CurrentModule.GetCurrentAction().Name + ": " +
				(int) (-1 * _mainPlayer.CurrentModule.GetCurrentAction().Attack.EnergyRecoilModifier 
				* _mainPlayer.CurrentModule.GetCurrentAction().Attack.BaseDamage));

			button = GetButtonFromUIGroup(ExchangeControls, "NextAction");
			UpdateButtonColor(button, _mainPlayer.CurrentModule.GetRightAction().ActionTexture);
			UpdateButtonText(button, _mainPlayer.CurrentModule.GetRightAction().Name);

			button = GetButtonFromUIGroup(ExchangeControls, "PreviousAction");
			UpdateButtonColor(button, _mainPlayer.CurrentModule.GetLeftAction().ActionTexture);
			UpdateButtonText(button, "");

			var text = GetTextFromPanelUIGroup(ExchangeControls, "HealthText");
			text.text = _mainPlayer.Health.ToString();

			text = GetTextFromPanelUIGroup(ExchangeControls, "EnergyText");
			text.text = _mainPlayer.Energy.ToString();

			text = GetTextFromPanelUIGroup(ExchangeControls, "Enemy1EnergyText");
			text.text = Players[0].Energy.ToString();

			text = GetTextFromPanelUIGroup(ExchangeControls, "Enemy1HealthText");
			text.text = Players[0].Health.ToString();
		}

		//updates the button color
		private void UpdateButtonColor(Button button, Color color)
		{
			
			if(button != null && color != null)
			{
				ColorBlock cb = button.colors;
				cb.normalColor = color;
				button.colors = cb;
			}
			else
			{
				if (button == null)
				{
					Debug.Log("Failed to update button color: Missing button");
				}
				else
				{
					Debug.Log("Failed to update button color: Missing color for Button: " +  button.name);
				}

			}
		}

		//updates the button text
		private void UpdateButtonText(Button button, String name)
		{

			if (button != null && ( name != null || name.Equals("")) )
			{
				var text = button.gameObject.GetComponentInChildren<Text>();
				text.text = name;
			}
			else
			{
				if (button == null)
				{
					Debug.Log("Failed to update button text: Missing button");
				}
				else
				{
					Debug.Log("Failed to update button text: Missing name for Button: " + button.name);
				}

			}
		}
	}
}