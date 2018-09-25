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
		private Button Add1;
		private Button Add10;
		private Button Add100;
		private Button Add1k;

		public void Awake()
		{
			_amount = 0;
			AmountField = transform.Find("Amount").GetComponent<InputField>();
			Add1 = transform.Find("QuickAddButtons").Find("Add1").GetComponent<Button>();
			Add10 = transform.Find("QuickAddButtons").Find("Add10").GetComponent<Button>();
			Add100 = transform.Find("QuickAddButtons").Find("Add100").GetComponent<Button>();
			Add1k = transform.Find("QuickAddButtons").Find("Add1k").GetComponent<Button>();

			Add1.onClick.AddListener(() => { Add(1); });
			Add10.onClick.AddListener(() => { Add(10); });
			Add100.onClick.AddListener(() => { Add(100); });
			Add1k.onClick.AddListener(() => { Add(1000); });

			AmountField.onEndEdit.AddListener(OnEndEdit);
		}

		public void Add(int value)
		{
			Amount += value;
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
