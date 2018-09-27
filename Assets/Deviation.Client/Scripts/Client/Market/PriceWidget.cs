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
		//constants
		private const int INITIAL_PRICE = 0;

		//private variables
		private int _amount;
		private InputField AmountField;
		private Button Over;
		private Button Market;
		private Button Under;

		//public properties
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
				AmountField.text = StringUtilities.ConvertIntToLongIntString(_amount);
				OnAmountChange(_amount);
			}
		}
		public UnityAction<int> OnAmountChange { get; set; }

		public void Awake()
		{
			_amount = 0;
			AmountField = transform.Find("AmountRow").GetComponentInChildren<InputField>();
			Over = transform.Find("AmountRow").Find("Over").GetComponent<Button>();
			//Market = transform.Find("QuickPriceButtons").Find("Market").GetComponent<Button>();
			Under = transform.Find("AmountRow").Find("Under").GetComponent<Button>();

			Over.onClick.AddListener(Overprice);
			//Market.onClick.AddListener(Market);
			Under.onClick.AddListener(Underprice);

			AmountField.onEndEdit.AddListener(OnEndEdit);
		}

		public void Reinitialize()
		{
			Amount = INITIAL_PRICE;
		}

		public void SetToMarketPrice()
		{
			//get market price
			Amount = 3125;
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
			int amount = StringUtilities.ConvertShortIntStringToInt(value);

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
