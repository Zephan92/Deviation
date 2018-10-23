using Assets.Deviation.Client.Scripts.UserInterface;
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

		public void Awake()
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
			if (OpenOfferPanel())
			{
				_tradeSelection.Open();
				_tradeSelection.Init(this, trade);
			}
			else
			{
				var go = Instantiate(Resources.Load("DialogPopup"), transform.root) as GameObject;
				var dialogPopup = go.GetComponent<DialogPopup>();
				dialogPopup.Init("No Open Trade Offer Slots!", "You do not have enough room to add another offer, please collect or cancel your current trade offers to make room.", "OK", () => { });
			}
		}

		public bool OpenOfferPanel()
		{
			foreach (var offer in _offerDetailPanels)
			{
				if (!offer.HasOffer)
				{
					return true;
				}
			}

			return false;
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

			Debug.LogError("Failed to find an Open Offer Panel");
		}

		public void CancelOffer(ITradeItem trade)
		{
			foreach (var panel in _offerDetailPanels)
			{
				if (panel.HasOffer && panel.TradeOffer.ID == trade.ID)
				{
					panel.RemoveOffer();
					return;
				}
			}
		}

		public void Bought(ITradeItem trade)
		{
			foreach (var panel in _offerDetailPanels)
			{
				if (panel.HasOffer && panel.TradeOffer.ID == trade.ID)
				{
					panel.OnProgressStatusChange(trade);
					return;
				}
			}
		}

		public void Sold(ITradeItem trade)
		{
			foreach (var panel in _offerDetailPanels)
			{
				if (panel.HasOffer && panel.TradeOffer.ID == trade.ID)
				{
					panel.OnProgressStatusChange(trade);
					return;
				}
			}
		}
	}
}
