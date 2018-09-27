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
		private Transform _tradeSelectionPanel;
		private Transform _trades;
		private Transform _buyTransform;
		private Transform _sellTransform;

		private TradeSelection _tradeSelection;
		private Button _buyButton;
		private Button _sellButton;

		private List<OfferDetailsPanel> _offerDetailPanels;

		public void Start()
		{
			//children
			_trades = transform.Find("Trades");
			_buyTransform = transform.Find("Buy");
			_sellTransform = transform.Find("Sell");
			_tradeSelectionPanel = transform.Find("TradeSelectionPopup");

			_tradeSelection = _tradeSelectionPanel.GetComponent<TradeSelection>();
			_buyButton = _buyTransform.GetComponent<Button>();
			_sellButton = _sellTransform.GetComponent<Button>();

			_buyButton.onClick.AddListener(() => OpenTradeSelection(TradeInterfaceType.Buy));
			_sellButton.onClick.AddListener(() => OpenTradeSelection(TradeInterfaceType.Sell));

			_offerDetailPanels = _trades.GetComponentsInChildren<OfferDetailsPanel>().ToList();
		}

		public void OpenTradeSelection(TradeInterfaceType trade)
		{
			_tradeSelection.Open();
			_tradeSelection.Init(this, trade);
		}

		public void Fill(TradeInterfaceType type, ITradeItem trade)
		{
			foreach (var offer in _offerDetailPanels)
			{
				if (!offer.HasOffer)
				{
					offer.Init(type, trade);
					return;
				}
			}
		}
	}
}
