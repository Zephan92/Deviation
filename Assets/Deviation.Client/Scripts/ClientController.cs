using UnityEngine;
using UnityEngine.UI;
using Assets.Deviation.Client.Scripts;

namespace Assets.Deviation.Exchange.Scripts.Client
{
	public enum ClientTab
	{
		Home,
		Play,
		Profile,
		Craft,
		Kits,
		Market,
	}

	public class ClientController : ControllerBase
	{
		public Transform UIs;
		public Transform Header;

		public GameObject MatchmakakingUI;
		public GameObject CraftingUI;
		public GameObject HomeUI;
		public GameObject ProfileUI;
		public GameObject KitsUI;
		public GameObject CurrentUI;
		public GameObject MarketUI;

		public Button PlayButton;
		public Button HomeButton;
		public Button ProfileButton;
		public Button CraftButton;
		public Button KitsButton;
		public Button MarketButton;

		public ClientTab CurrentTab;

		public override void Awake()
		{
			base.Awake();

			var parent = GameObject.Find("ClientUI");
			Header = parent.transform.Find("Header");
			UIs = parent.transform.Find("UIs");

			MatchmakakingUI = UIs.transform.Find("MatchmakingUI").gameObject;
			CraftingUI = UIs.transform.Find("CraftingUI").gameObject;
			HomeUI = UIs.transform.Find("HomeUI").gameObject;
			ProfileUI = UIs.transform.Find("ProfileUI").gameObject;
			KitsUI = UIs.transform.Find("KitsUI").gameObject;
			MarketUI = UIs.transform.Find("MarketUI").gameObject;

			PlayButton = Header.Find("Play").GetComponent<Button>();
			HomeButton = Header.Find("Home").GetComponent<Button>();
			ProfileButton = Header.Find("Profile").GetComponent<Button>();
			CraftButton = Header.Find("Craft").GetComponent<Button>();
			KitsButton = Header.Find("Kits").GetComponent<Button>();
			MarketButton = Header.Find("Market").GetComponent<Button>();

			PlayButton.onClick.AddListener(() => { SwitchTab(ClientTab.Play); });
			HomeButton.onClick.AddListener(() => { SwitchTab(ClientTab.Home); });
			ProfileButton.onClick.AddListener(() => { SwitchTab(ClientTab.Profile); });
			CraftButton.onClick.AddListener(() => { SwitchTab(ClientTab.Craft); });
			KitsButton.onClick.AddListener(() => { SwitchTab(ClientTab.Kits); });
			MarketButton.onClick.AddListener(() => { SwitchTab(ClientTab.Market); });

			SwitchTab(ClientTab.Home);
		}

		public override void Start()
		{
			base.Start();
			ClientDataRepository.Instance.State = ClientState.Client;
		}

		public void SwitchTab(ClientTab tab)
		{
			if (CurrentUI != null)
			{
				CurrentUI.SetActive(false);
			}
			CurrentTab = tab;

			switch (tab)
			{
				case ClientTab.Play:
					MatchmakakingUI.SetActive(true);
					CurrentUI = MatchmakakingUI;
					break;

				case ClientTab.Profile:
					ProfileUI.SetActive(true);
					CurrentUI = ProfileUI;
					break;

				case ClientTab.Craft:
					CraftingUI.SetActive(true);
					CurrentUI = CraftingUI;
					break;

				case ClientTab.Kits:
					KitsUI.SetActive(true);
					CurrentUI = KitsUI;
					break;

				case ClientTab.Market:
					MarketUI.SetActive(true);
					CurrentUI = MarketUI;
					break;

				case ClientTab.Home:
				default:
					HomeUI.SetActive(true);
					CurrentUI = HomeUI;
					break;
			}
		}
	}
}
