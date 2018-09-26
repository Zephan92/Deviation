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
	public class QuantityWidget : MonoBehaviour
	{
		//constants
		private const int INITIAL_QUANTITY = 0;

		//private variables
		private int _amount;
		private InputField AmountField;
		private Button Plus;
		private Button Minus;
		private Button Reset;

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
				AmountField.text = StringUtilities.ConvertIntToAggregateString(_amount);
				OnAmountChange(_amount);
			}
		}
		public UnityAction<int> OnAmountChange { get; set; }

		public void Awake()
		{
			_amount = 0;
			AmountField = transform.Find("AmountRow").GetComponentInChildren<InputField>();
			Plus = transform.Find("AmountRow").Find("Plus").GetComponent<Button>();
			//Reset = transform.Find("Amount").Find("Reset").GetComponent<Button>();
			Minus = transform.Find("AmountRow").Find("Minus").GetComponent<Button>();

			Plus.onClick.AddListener(() => { Add(1); });
			//Reset.onClick.AddListener(() => { Amount = 1; });
			Minus.onClick.AddListener(() => { Add(-1); });

			AmountField.onEndEdit.AddListener(OnEndEdit);
		}

		public void Add(int value)
		{
			Amount += value;
		}

		public void Reinitialize()
		{
			Amount = 1;
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
