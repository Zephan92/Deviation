using Assets.Deviation.Client.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client.Market
{
	public class PriceWidget : MonoBehaviour
	{
		public int Amount
		{
			get { return _amount; }
			set
			{
				if (value < 0)
				{
					value = 0;
				}

				_amount = value;
				AmountField.text = StringUtilities.ConvertIntToAggregateString(_amount);
				OnAmountChange(_amount);
			}
		}
		public UnityAction<int> OnAmountChange;

		private int _amount;
		private InputField AmountField;
		private Button OverpriceButton;
		private Button MarketButton;
		private Button UnderpriceButton;

		public void Awake()
		{
			_amount = 0;
			AmountField = transform.Find("Amount").GetComponent<InputField>();
			OverpriceButton = transform.Find("QuickPriceButtons").Find("OverMarket").GetComponent<Button>();
			MarketButton = transform.Find("QuickPriceButtons").Find("Market").GetComponent<Button>();
			UnderpriceButton = transform.Find("QuickPriceButtons").Find("UnderMarket").GetComponent<Button>();

			OverpriceButton.onClick.AddListener(Overprice);
			MarketButton.onClick.AddListener(Market);
			UnderpriceButton.onClick.AddListener(Underprice);

			AmountField.onEndEdit.AddListener(OnEndEdit);
		}

		public void Market()
		{
			Amount = 3523;
		}

		public void Overprice()
		{
			int valueToAdd = (int)(Amount * 0.1f);

			if ((float)(valueToAdd + (float)Amount) > Int32.MaxValue)
			{
				Amount = Int32.MaxValue;
				return;
			}

			Amount += valueToAdd;
		}

		public void Underprice()
		{
			int valueToAdd = (int)(Amount * -0.1f);
			Amount += valueToAdd;
		}

		private void OnEndEdit(string value)
		{
			int amount = StringUtilities.ConvertAggregateStringToInt(value);

			if (amount >= 0)
			{
				Amount = amount;
			}
			else
			{
				Amount = _amount;
			}
		}
	}
}
