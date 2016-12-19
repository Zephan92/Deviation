using UnityEngine;
using System.Collections;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Controllers
{
	public class ExchangeController : MonoBehaviour, IExchangeController
	{
		//Unity Objects
		public GameObject MainPlayerObject;
		public GameObject[] PlayerObjects;

		public static int NumberOfPlayers = 2;

		//Public Static Variables
		public ExchangeState ExchangeState;


		//Private Static Variables
		private Player _mainPlayer;
		private GameObject[] _displays;
		private bool _battleStartDisplayIsEnabled = false;
		private bool _ExchangeControlsDisplayIsEnabled = false;
		private bool _awaitingPlayerInput = false;
		private MainPlayerController mp;

		void Awake()
		{
			ExchangeState = ExchangeState.Setup;

			if (mp == null)
			{
				var mpObject = GameObject.FindGameObjectWithTag("MainPlayerController");
				mp = mpObject.GetComponent<MainPlayerController>();
			}

			if (MainPlayerObject == null)
			{
				MainPlayerObject = GameObject.FindGameObjectWithTag("MainPlayer");
				_mainPlayer = MainPlayerObject.GetComponent<Player>();
			}

			PlayerObjects = GameObject.FindGameObjectsWithTag("Player");

			_displays = GameObject.FindGameObjectsWithTag("Display");

			ExchangeState = ExchangeState.PreBattle;
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
                        ToggleDisplay("BattleStart", _battleStartDisplayIsEnabled);
						SelectButton("BattleStart", "Start");
						_battleStartDisplayIsEnabled = true;
					}
                    break;
                case ExchangeState.Start:
					if (!_ExchangeControlsDisplayIsEnabled)
					{
						ToggleDisplay("ExchangeControls", _ExchangeControlsDisplayIsEnabled);
						_ExchangeControlsDisplayIsEnabled = true;
						UpdateExchangeControlsDisplay();
					}
					ExchangeState = ExchangeState.Battle;
                    break;
                case ExchangeState.Battle:
                    mp.CheckInput();
					CheckBattleEnd();
                    break;
                case ExchangeState.End:
                    ExchangeState = ExchangeState.PostBattle;
                    break;
                case ExchangeState.PostBattle:
                    ExchangeState = ExchangeState.Teardown;
                    break;
                case ExchangeState.Teardown:
                    BackToMainMenu();
                    break;
                case ExchangeState.Paused:
                    break;
                default:
                    break;
            }
        }

		private void CheckBattleEnd()
		{
			if (_mainPlayer.GetHealth() <= 0 || PlayerObjects[0].GetComponent<Player>().GetHealth() <= 0)
			{
				ExchangeState = ExchangeState.End;
			}
		}

		private void SelectButton(string UIGroupName, string buttonName)
		{
			GetButtonFromUIGroup(GetDisplayObject(UIGroupName), buttonName).GetComponent<Selectable>().Select();
		}

        private void ToggleDisplay(string displayName, bool currentDisplayValue)
        {
			toggleVisibility(GetDisplayObject(displayName), !currentDisplayValue);
        }


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

        private void BackToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

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

		public void ClickOnButton(string groupUIName, string buttonName)
		{
			GameObject ExchangeControlsUIGroup = GetDisplayObject(groupUIName);
			Button button = GetButtonFromUIGroup(ExchangeControlsUIGroup, buttonName);
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
			ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
			StartCoroutine(UnClickPointer(button, pointer));
		}

		private IEnumerator UnClickPointer(Button button, PointerEventData pointer)
		{
			yield return new WaitForSeconds(0.1f);
			ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
			ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerExitHandler);
		}

		public void ChangeStateToStart()
        {
            ToggleDisplay("BattleStart", _battleStartDisplayIsEnabled);
            ExchangeState = ExchangeState.Start;
            _awaitingPlayerInput = false;
        }

		public void UpdateExchangeControlsDisplay()
		{
			var ExchangeControls = GetDisplayObject("ExchangeControls");
			var button = GetButtonFromUIGroup(ExchangeControls, "CurrentModule");
			UpdateButtonColor(button, _mainPlayer.EquipedKit.GetCurrentModule().ModuleTexture);
			UpdateButtonText(button, _mainPlayer.EquipedKit.GetCurrentModule().Name);

			button = GetButtonFromUIGroup(ExchangeControls, "NextModule");
			UpdateButtonColor(button, _mainPlayer.EquipedKit.GetRightModule().ModuleTexture);
			UpdateButtonText(button, _mainPlayer.EquipedKit.GetRightModule().Name);

			button = GetButtonFromUIGroup(ExchangeControls, "PreviousModule");
			UpdateButtonColor(button, _mainPlayer.EquipedKit.GetLeftModule().ModuleTexture);
			UpdateButtonText(button, "");

			button = GetButtonFromUIGroup(ExchangeControls, "CurrentAction");
			UpdateButtonColor(button, _mainPlayer.EquipedKit.GetCurrentModule().GetCurrentAction().ActionTexture);
			UpdateButtonText(button, _mainPlayer.EquipedKit.GetCurrentModule().GetCurrentAction().Name);

			button = GetButtonFromUIGroup(ExchangeControls, "NextAction");
			UpdateButtonColor(button, _mainPlayer.EquipedKit.GetCurrentModule().GetRightAction().ActionTexture);
			UpdateButtonText(button, _mainPlayer.EquipedKit.GetCurrentModule().GetRightAction().Name);

			button = GetButtonFromUIGroup(ExchangeControls, "PreviousAction");
			UpdateButtonColor(button, _mainPlayer.EquipedKit.GetCurrentModule().GetLeftAction().ActionTexture);
			UpdateButtonText(button, "");

			var text = GetTextFromPanelUIGroup(ExchangeControls, "HealthText");
			text.text = _mainPlayer.GetHealth().ToString();

			text = GetTextFromPanelUIGroup(ExchangeControls, "EnergyText");
			text.text = _mainPlayer.GetEnergy().ToString();

			text = GetTextFromPanelUIGroup(ExchangeControls, "Enemy1EnergyText");
			text.text = PlayerObjects[0].GetComponent<Player>().GetEnergy().ToString();

			text = GetTextFromPanelUIGroup(ExchangeControls, "Enemy1HealthText");
			text.text = PlayerObjects[0].GetComponent<Player>().GetHealth().ToString();
		}

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