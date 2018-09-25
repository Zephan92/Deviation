using Assets.Deviation.Client.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client.Market
{
	public enum TradeInterfaceType
	{
		Buy,
		Sell
	}

	public class TradeSelection : MonoBehaviour
	{
		public Transform TradeSelectionPanel;
		public Button CloseButton;
		public Transform BuyTransform;
		public Transform SellTransform;
		public QuantityWidget Quantity;
		public PriceWidget Price;
		public Text TotalPriceText;
		public int TotalPrice;

		public TradeInterfaceType Type;

		public void Awake()
		{
			TradeSelectionPanel = transform.Find("TradeSelectionPanel");

			Quantity = TradeSelectionPanel.Find("Quantity").GetComponent<QuantityWidget>();
			Price = TradeSelectionPanel.Find("Price").GetComponent<PriceWidget>();
			TotalPriceText = TradeSelectionPanel.Find("TotalPrice").GetComponentInChildren<Text>();

			CloseButton = TradeSelectionPanel.Find("CloseButton").GetComponent<Button>();
			CloseButton.onClick.AddListener(Close);

			Quantity.OnAmountChange += OnTradeChange;
			Price.OnAmountChange += OnTradeChange;
		}

		public void OnTradeChange(int value)
		{
			long totalPrice = Quantity.Amount * (long) Price.Amount;
			if (totalPrice <= Int32.MaxValue)
			{
				TotalPrice = (int) totalPrice;
				TotalPriceText.text = StringUtilities.ConvertIntToAggregateString(TotalPrice);
			}
			else
			{
				TotalPrice = Int32.MaxValue;
				TotalPriceText.text = "Too much!";
			}
		}

		public void Init(TradeInterfaceType type)
		{
			Type = type;
		}

		public void Open()
		{
			gameObject.SetActive(true);
		}

		public void Close()
		{
			gameObject.SetActive(false);
		}
	}
}
