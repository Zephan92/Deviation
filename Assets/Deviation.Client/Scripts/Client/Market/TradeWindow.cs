using Assets.Deviation.Exchange.Scripts.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client.Market
{
	public class TradeWindow : MonoBehaviour
	{
		public Transform TradeSelectionPanel;
		public TradeSelection TradeSelection;

		public Transform EmptyTransform;
		public Transform FilledTransform;

		public Transform BuyTransform;
		public Transform SellTransform;

		public Button BuyButton;
		public Button SellButton;


		public void Start()
		{
			var parent = GameObject.Find("MarketUI");
			TradeSelectionPanel = parent.transform.Find("TradingUI").Find("TradeSelection");
			TradeSelection = TradeSelectionPanel.GetComponent<TradeSelection>();
			EmptyTransform = transform.Find("Empty");
			BuyTransform = EmptyTransform.Find("Item").Find("Buy");
			SellTransform = EmptyTransform.Find("Item").Find("Sell");
			BuyButton = BuyTransform.GetComponent<Button>();
			SellButton = SellTransform.GetComponent<Button>();

			FilledTransform = transform.Find("Filled");

			BuyButton.onClick.AddListener(() => OpenTradeSelection(TradeInterfaceType.Buy));
			SellButton.onClick.AddListener(() => OpenTradeSelection(TradeInterfaceType.Sell));
		}

		public void OpenTradeSelection(TradeInterfaceType trade)
		{
			TradeSelection.Open();
			TradeSelection.Init(trade);
		}
	}
}
