using Assets.Deviation.Client.Scripts.Utilities;
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
	public enum OrderType
	{
		None,
		Buy,
		Sell
	}

	public class TradeSelection : MonoBehaviour
	{
		private MarketController mc;
		private TradeWindow _tradeWindow;

		public Transform TradeSelectionPanel;
		public Text Title;
		public Button CloseButton;
		public Button ConfirmButton;
		public QuantityWidget Quantity;
		public PriceWidget Price;
		public ItemWidget Item;
		public Text TotalPriceText;
		public int TotalPrice;
		public OrderType Type;

		public void Awake()
		{
			mc = FindObjectOfType<MarketController>();

			TradeSelectionPanel = transform.Find("TradeSelectionPanel");

			Title = TradeSelectionPanel.Find("Title").GetComponent<Text>();
			Quantity = TradeSelectionPanel.Find("OrderForm").Find("Quantity").GetComponent<QuantityWidget>();
			Price = TradeSelectionPanel.Find("OrderForm").Find("Price").GetComponent<PriceWidget>();
			Item = TradeSelectionPanel.Find("OrderForm").Find("Item").GetComponent<ItemWidget>();
			TotalPriceText = TradeSelectionPanel.Find("OrderForm").Find("TotalPrice").Find("TotalAmount").GetComponentInChildren<Text>();

			CloseButton = TradeSelectionPanel.Find("CloseButton").GetComponent<Button>();
			CloseButton.onClick.AddListener(Close);

			ConfirmButton = TradeSelectionPanel.Find("ConfirmButton").GetComponent<Button>();
			ConfirmButton.onClick.AddListener(Confirm);

			Quantity.OnAmountChange += OnTradeChange;
			Price.OnAmountChange += OnTradeChange;
			Item.OnItemChange += OnItemChange;
		}

		public void Confirm()
		{
			ITradeItem trade = CreateTrade();

			switch (Type)
			{
				case OrderType.Buy:
					mc.Buy(trade, OnConfirmCallback);
					break;
				case OrderType.Sell:
					mc.Sell(trade, OnConfirmCallback);
					break;
			}

			Close();
		}

		private void OnConfirmCallback(ITradeItem trade)
		{
			_tradeWindow.Fill(trade);
		}

		private ITradeItem CreateTrade()
		{
			switch (Item.TradeItem.ResourceType)
			{
				case ResourceType.Action:
					return new ActionTradeItem(0, Item.TradeItem.Name, Price.Amount, Quantity.Amount, ClientDataRepository.Instance.PlayerAccount.Id, OrderType.None);
				//case TradeType.Resource:
				//return new ResourceTradeItem(ItemToTrade.Name, Price.Amount, Quantity.Amount, ClientDataRepository.Instance.PlayerAccount.Id);
				//case TradeType.Material:
				//return new MaterialTradeItem(ItemToTrade.Name, Price.Amount, Quantity.Amount, ClientDataRepository.Instance.PlayerAccount.Id);
				default:
					return new TradeItem(0, Item.TradeItem.Name, Price.Amount, Quantity.Amount, ClientDataRepository.Instance.PlayerAccount.Id, Item.TradeItem.ResourceType, OrderType.None);
			}
		}

		private void OnTradeChange(int value)
		{
			long totalPrice = Quantity.Amount * (long) Price.Amount;
			if (totalPrice <= Int32.MaxValue)
			{
				TotalPrice = (int) totalPrice;
				TotalPriceText.text = StringUtilities.ConvertIntToLongIntString(TotalPrice);
			}
			else
			{
				TotalPrice = Int32.MaxValue;
				TotalPriceText.text = "Too much!";
			}
			ToggleConfirmButton();
		}

		private void OnItemChange(ITradeItem item)
		{
			if (item != null)
			{
				Price.Amount = item.Price;
			}

			ToggleConfirmButton();
		}

		private void ToggleConfirmButton()
		{
			ConfirmButton.interactable = IsOfferReady();
		}

		private bool IsOfferReady()
		{
			return Item.TradeItem != null &&
				Price.Amount > 0 &&
				Quantity.Amount > 0;
		}

		public void Init(TradeWindow window, OrderType type)
		{
			_tradeWindow = window;
			Type = type;
			Title.text = $"{type} Offer";
		}

		public void Open()
		{
			gameObject.SetActive(true);
			ConfirmButton.interactable = false;
			Item?.Reinitialize();
			Quantity?.Reinitialize();
			Price?.Reinitialize();
		}

		public void Close()
		{
			gameObject.SetActive(false);
		}
	}
}
