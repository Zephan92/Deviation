using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Match
{
	public enum ChooseTraderUIState
	{
		Start = 0,
		TraderSelected = 1,
		TraderConfirmed = 2,
	}

	public class ChooseTraderUIController : UIController
	{
		public ITrader CurrentTrader;
		public UnityAction<ITrader> OnConfirmTrader;
		public ChooseTraderUIState UIState
		{
			get
			{
				return _uiState;
			}
			set
			{
				_uiState = value;

				if (OnUIStateChange != null)
				{
					OnUIStateChange(value);
				}
			}
		}
		public InputField SearchTradersList;
		public TraderSelectScrollPanel TraderList;
		public Button[] TraderListFilters = new Button[5];
		public TraderDetailsPanel TraderDetails;

		private UnityAction<ChooseTraderUIState> OnUIStateChange;
		private UnityAction<int> onListChange;
		private ChooseTraderUIState _uiState;
		private ITrader _selectedTrader;
		private bool[] _traderListFiltersEnabled = new bool[5];
		private List<GameObject> _traderListPanels;

		private const int Max_Item_Before_Scroll = 5;
		private const float Item_Width = 248;

		public override void Awake()
		{
			base.Awake();

			TraderList = GetComponentInChildren<TraderSelectScrollPanel>();
			TraderDetails = GetComponentInChildren<TraderDetailsPanel>();
			SearchTradersList = GetComponentInChildren<InputField>();
			var footer = transform.Find("Footer");
			TraderListFilters = footer.GetComponentsInChildren<Button>();
			OnUIStateChange += OnUIStateChangeMethod;
			onListChange += TraderList.OnListChange;
			SearchTradersList.onValueChanged.AddListener(FilterOnSearch);

			TraderList.MaxListSize = Max_Item_Before_Scroll;
			TraderList.ItemWidth = Item_Width;
			for(int i = 0; i < _traderListFiltersEnabled.Length; i++)
			{
				_traderListFiltersEnabled[i] = true;
			}
		}

		public override void Start()
		{
			var traders = new List<ITrader>()
			{
				new Trader("Dude", "The king of the world", TraderType.Alpha, "", Guid.NewGuid()),
				new Trader("Yolo", "The Land before time", TraderType.Gamma, "", Guid.NewGuid()),
				new Trader("Swakalot", "The Stealthy Knight", TraderType.Beta, "", Guid.NewGuid()),
				new Trader("Chain", "The liverator", TraderType.Alpha, "A long, long time ago. In a galaxy far away. Naboo was under an attack And I thought me and Qui-Gon Jinn Could talk the federation into Maybe cutting them a little slack But their response, it didn't thrill us They locked the doors and tried to kill us . We escaped from that gas Then met Jar Jar and Boss Nass We took a bongo from the scene And we went to Theed to see the Queen We all wound up on Tatooine That's where we found this boy...", Guid.NewGuid()),
				new Trader("Chris", "He Man", TraderType.Gamma, "", Guid.NewGuid()),
				new Trader("Merlot", "The drink of Time", TraderType.Epsilon, "", Guid.NewGuid()),
				new Trader("Johnnnny", "Banned before the hand", TraderType.Epsilon, "", Guid.NewGuid()),
				new Trader("Sevannah", "leaked trader", TraderType.Beta, "", Guid.NewGuid()),
				new Trader("jannet", "The Terrible", TraderType.Delta, "", Guid.NewGuid()),
				new Trader("carl", "The warrior", TraderType.Alpha, "", Guid.NewGuid()),
				new Trader("cooked", "The wisdom seeker", TraderType.Delta, "", Guid.NewGuid()),
				new Trader("lololol", "The Last of His Kind", TraderType.Beta, "", Guid.NewGuid()),
				new Trader("yeahitkeepsgoing", "The Winds of Winter", TraderType.Epsilon, "", Guid.NewGuid()),
				new Trader("Short", "The Shadow of doubt", TraderType.Gamma, "", Guid.NewGuid()),
			};
			traders = traders.OrderBy(t => t.Name).ToList();
			_traderListPanels = InitializeTraderPanels(traders);
			UIState = ChooseTraderUIState.Start;
		}

		public override void Update()
		{
			//ChooseTraderTimer.text = ((int)tm.GetRemainingCooldown(ClientMatchState.ChooseTrader.ToString())).ToString();
		}

		private void OnUIStateChangeMethod(ChooseTraderUIState state)
		{
			switch (state)
			{
				case ChooseTraderUIState.Start:
					RebuildTraderList();
					break;
				case ChooseTraderUIState.TraderSelected:
					break;
				case ChooseTraderUIState.TraderConfirmed:
					cmc.State = ClientMatchState.ChooseActions;
					break;
			}
		}

		public void ConfirmTrader()
		{
			if (OnConfirmTrader != null)
			{
				OnConfirmTrader(_selectedTrader);
			}

			foreach (var panel in _traderListPanels)
			{
				var traderDetail = panel.GetComponent<TraderDetailsPanel>();

				if (traderDetail.Chosen)
				{
					traderDetail.transform.SetAsFirstSibling();
					panel.SetActive(true);
				}
				else
				{
					traderDetail.PanelEnabled(false);
					panel.SetActive(false);
				}
			}

			TraderList.PanelEnabled(false);
			TraderList.ResetPanel();
			SearchTradersList.transform.parent.gameObject.SetActive(false);
			UIState = ChooseTraderUIState.TraderConfirmed;
		}

		public void FilterOnSearch(string searchField)
		{
			RebuildTraderList();
		}

		public void FilterOnclick(int type)
		{
			bool check = true;
			//check if all filters are enabled
			for (int i = 0; i < _traderListFiltersEnabled.Count(); i++)
			{
				check = check && _traderListFiltersEnabled[i];
			}

			if (check)
			{//if all filters are enabled, disable all filters but the specified
				for (int i = 0; i < _traderListFiltersEnabled.Count(); i++)
				{
					_traderListFiltersEnabled[i] = false;
				}

				_traderListFiltersEnabled[type] = true;
			}
			else
			{//else check if only the specified filter is enabled
				check = _traderListFiltersEnabled[type];
				for (int i = 0; i < _traderListFiltersEnabled.Count(); i++)
				{
					check = check && !_traderListFiltersEnabled[i];
				}

				if (check)
				{//if only the enabled filter is the one specified, turn all filters on
					for (int i = 0; i < _traderListFiltersEnabled.Count(); i++)
					{
						_traderListFiltersEnabled[i] = true;
					}
				}
				else if (!_traderListFiltersEnabled[type])
				{//else if the specified filter is disabled, enable it
					_traderListFiltersEnabled[type] = true;
				}
				else
				{//else the specified filter is disabled
					_traderListFiltersEnabled[type] = false;

					//if everything is off after this filter is disabled
					check = true;
					for (int i = 0; i < _traderListFiltersEnabled.Count(); i++)
					{
						check = check && !_traderListFiltersEnabled[i];
					}

					//turn all filters on
					if (check)
					{
						for (int i = 0; i < _traderListFiltersEnabled.Count(); i++)
						{
							_traderListFiltersEnabled[i] = true;
						}
					}
				}
			}

			RebuildTraderList();
		}

		public List<GameObject> InitializeTraderPanels(List<ITrader> traders)
		{
			List<GameObject> traderPanels = new List<GameObject>();

			foreach (ITrader trader in traders)
			{
				UnityAction onTraderListPanelClick = () =>
				{
					TraderDetails.UpdateTraderDetails(trader, ConfirmTrader);
					UnChooseTraderListPanels(trader);
					_selectedTrader = trader;
					UIState = ChooseTraderUIState.TraderSelected;
				};
				var panel = TraderPanelFactory.CreateTraderPanel(trader, TraderList.List, onTraderListPanelClick);
				traderPanels.Add(panel);
			}

			return traderPanels;
		}

		private void RebuildTraderList()
		{
			int traderCount = 0;
			foreach (GameObject panel in _traderListPanels)
			{
				ITrader trader = panel.GetComponent<TraderDetailsPanel>().Trader;
				panel.SetActive(_traderListFiltersEnabled[(int)trader.Type]);

				if (SearchTradersList.text.Length > 0 && panel.activeInHierarchy)
				{
					panel.SetActive(trader.Name.StartsWith(SearchTradersList.text, System.StringComparison.CurrentCultureIgnoreCase));
				}

				if (panel.activeInHierarchy)
				{
					traderCount++;
				}
			}

			for(int i = 0; i < _traderListFiltersEnabled.Count(); i++)
			{
				ToggleButtonColor(_traderListFiltersEnabled[i], (TraderType) i);
			}

			onListChange(traderCount);
		}

		private void ToggleButtonColor(bool filterEnabled, TraderType type)
		{
			ColorBlock colors = TraderListFilters[(int)type].colors;
			if (filterEnabled)
			{
				colors.normalColor = Color.white;
				colors.pressedColor = Color.white;
				colors.highlightedColor = Color.white;
			}
			else
			{
				colors.normalColor = Color.gray;
				colors.pressedColor = Color.gray;
				colors.highlightedColor = Color.gray;
			}

			TraderListFilters[(int)type].colors = colors;
		}


		private void UnChooseTraderListPanels(ITrader trader)
		{
			foreach (var panel in _traderListPanels)
			{
				var traderDetail = panel.GetComponent<TraderDetailsPanel>();

				if (traderDetail.Trader != trader)
				{
					traderDetail.Unchoose();
				}
			}
		}
	}
}
