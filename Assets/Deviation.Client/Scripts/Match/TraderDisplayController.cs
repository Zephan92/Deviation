using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Deviation.Client.Scripts;
using UnityEngine.UI;
using UnityEngine.Events;
using Assets.Scripts.Utilities;
using Assets.Scripts.Interface;
using Assets.Deviation.Client.Scripts.Match;
using Assets.Deviation.Client.Scripts.UserInterface;

public class TraderDisplayController : MonoBehaviour
{
	public TraderDetailsPanel TraderDetailsPanel;
	public VerticalScrollPanel CharacterList;
	public Button[] Filters = new Button[5];
	public InputField Search;

	public bool Alpha = true;
	public bool Beta = true;
	public bool Gamma = true;
	public bool Delta = true;
	public bool Epsilon = true;

	private List<GameObject> _traderPanels;
	private UnityAction<int> onListChange;
	private const int Max_Item_Before_Scroll = 7;
	private const float Item_Height = 80;
	private int _traderCount;

	public void Start() {
		Search.onValueChanged.AddListener(FilterOnSearch);
		CharacterList.MaxListSize = Max_Item_Before_Scroll;
		CharacterList.ItemHeight = Item_Height;
		onListChange += CharacterList.OnListChange;

		var traders = new List<ITrader>()
		{
			new Trader("Dude", "The king of the world", TraderType.Alpha, ""),
			new Trader("Yolo", "The Land before time", TraderType.Gamma, ""),
			new Trader("Swakalot", "The Stealthy Knight", TraderType.Beta, ""),
			new Trader("Chain", "The liverator", TraderType.Alpha, "A long, long time ago. In a galaxy far away. Naboo was under an attack And I thought me and Qui-Gon Jinn Could talk the federation into Maybe cutting them a little slack But their response, it didn't thrill us They locked the doors and tried to kill us . We escaped from that gas Then met Jar Jar and Boss Nass We took a bongo from the scene And we went to Theed to see the Queen We all wound up on Tatooine That's where we found this boy..."),
			new Trader("Chris", "He Man", TraderType.Gamma, ""),
			new Trader("Merlot", "The drink of Time", TraderType.Epsilon, ""),
			new Trader("Johnnnny", "Banned before the hand", TraderType.Epsilon, ""),
			new Trader("Sevannah", "leaked trader", TraderType.Beta, ""),
			new Trader("jannet", "The Terrible", TraderType.Delta, ""),
			new Trader("carl", "The warrior", TraderType.Alpha, ""),
			new Trader("cooked", "The wisdom seeker", TraderType.Delta, ""),
			new Trader("lololol", "The Last of His Kind", TraderType.Beta, ""),
			new Trader("yeahitkeepsgoing", "The Winds of Winter", TraderType.Epsilon, ""),
			new Trader("Short", "The Shadow of doubt", TraderType.Gamma, ""),
		};

		_traderPanels = InitializeTraderPanels(traders);
		RebuildTraderList();
	}

	public List<GameObject> InitializeTraderPanels(List<ITrader> traders)
	{
		List<GameObject> traderPanels = new List<GameObject>();

		foreach (ITrader trader in traders)
		{
			UnityAction unityAction = () =>
			{
				TraderDetailsPanel.UpdateTraderDetails(trader);
			};
			var panel = TraderPanelFactory.CreateTraderPanel(TraderDisplaySide.LeftSide, trader, CharacterList.List, unityAction, OnDifferentTraderPanelChosen);
			traderPanels.Add(panel);
		}

		return traderPanels;
	}

	public void OnDifferentTraderPanelChosen(ITrader trader)
	{
		foreach (var panel in _traderPanels)
		{
			var traderDetail = panel.GetComponent<TraderDetailsPanel>();

			if (traderDetail.Trader != trader)
			{
				traderDetail.Unchoose();
			}
		}
	}

	public void OnConfirmTrader()
	{
		foreach (var panel in _traderPanels)
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
		CharacterList.PanelEnabled(false);
		CharacterList.ResetPanel();
		Search.transform.parent.gameObject.SetActive(false);
	}

	private void ToggleButtonColor(bool filterEnabled, TraderType type)
	{
		ColorBlock colors = Filters[(int)type].colors;
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

		Filters[(int)type].colors = colors;
	}

	private void RebuildTraderList()
	{
		int traderCount = 0;
		foreach (GameObject panel in _traderPanels)
		{
			ITrader trader = panel.GetComponent<TraderDetailsPanel>().Trader;

			switch (trader.Type)
			{
				case TraderType.Alpha:
					panel.SetActive(Alpha);
					break;
				case TraderType.Beta:
					panel.SetActive(Beta);
					break;
				case TraderType.Gamma:
					panel.SetActive(Gamma);
					break;
				case TraderType.Delta:
					panel.SetActive(Delta);
					break;
				case TraderType.Epsilon:
					panel.SetActive(Epsilon);
					break;
			}

			if (Search.text.Length > 0 && panel.activeInHierarchy)
			{
				panel.SetActive(trader.Name.StartsWith(Search.text, System.StringComparison.CurrentCultureIgnoreCase));
			}

			if (panel.activeInHierarchy)
			{
				traderCount++;
			}
		}

		ToggleButtonColor(Alpha, TraderType.Alpha);
		ToggleButtonColor(Beta, TraderType.Beta);
		ToggleButtonColor(Gamma, TraderType.Gamma);
		ToggleButtonColor(Delta, TraderType.Delta);
		ToggleButtonColor(Epsilon, TraderType.Epsilon);

		onListChange(traderCount);
	}

	public void FilterOnSearch(string searchField)
	{
		RebuildTraderList();
	}

	public void FilterOnClickAlpha()
	{
		if (Alpha && Beta && Gamma && Delta && Epsilon)
		{
			Alpha = true;
			Beta = false;
			Gamma = false;
			Delta = false;
			Epsilon = false;
		}
		else if (Alpha && !Beta && !Gamma && !Delta && !Epsilon)
		{
			Alpha = true;
			Beta = true;
			Gamma = true;
			Delta = true;
			Epsilon = true;
		}
		else if (!Alpha)
		{
			Alpha = true;
		}
		else
		{
			Alpha = false;
		}

		RebuildTraderList();
	}

	public void FilterOnClickBeta()
	{
		if (Alpha && Beta && Gamma && Delta && Epsilon)
		{
			Alpha = false;
			Beta = true;
			Gamma = false;
			Delta = false;
			Epsilon = false;
		}
		else if (!Alpha && Beta && !Gamma && !Delta && !Epsilon)
		{
			Alpha = true;
			Beta = true;
			Gamma = true;
			Delta = true;
			Epsilon = true;
		}
		else if (!Beta)
		{
			Beta = true;
		}
		else
		{
			Beta = false;
		}

		RebuildTraderList();
	}

	public void FilterOnClickGamma()
	{
		if (Alpha && Beta && Gamma && Delta && Epsilon)
		{
			Alpha = false;
			Beta = false;
			Gamma = true;
			Delta = false;
			Epsilon = false;
		}
		else if (!Alpha && !Beta && Gamma && !Delta && !Epsilon)
		{
			Alpha = true;
			Beta = true;
			Gamma = true;
			Delta = true;
			Epsilon = true;
		}
		else if (!Gamma)
		{
			Gamma = true;
		}
		else
		{
			Gamma = false;
		}

		RebuildTraderList();
	}

	public void FilterOnClickDelta()
	{
		if (Alpha && Beta && Gamma && Delta && Epsilon)
		{
			Alpha = false;
			Beta = false;
			Gamma = false;
			Delta = true;
			Epsilon = false;
		}
		else if (!Alpha && !Beta && !Gamma && Delta && !Epsilon)
		{
			Alpha = true;
			Beta = true;
			Gamma = true;
			Delta = true;
			Epsilon = true;
		}
		else if (!Delta)
		{
			Delta = true;
		}
		else
		{
			Delta = false;
		}

		RebuildTraderList();
	}

	public void FilterOnClickEpsilon()
	{
		if (Alpha && Beta && Gamma && Delta && Epsilon)
		{
			Alpha = false;
			Beta = false;
			Gamma = false;
			Delta = false;
			Epsilon = true;
		}
		else if (!Alpha && !Beta && !Gamma && !Delta && Epsilon)
		{
			Alpha = true;
			Beta = true;
			Gamma = true;
			Delta = true;
			Epsilon = true;
		}
		else if (!Epsilon)
		{
			Epsilon = true;
		}
		else
		{
			Epsilon = false;
		}

		RebuildTraderList();
	}
}