using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Deviation.Exchange.Scripts.Utilities;
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
	public class ItemWidget : MonoBehaviour
	{
		//constants
		private const string INITIAL_ITEM = "";

		//private variables
		private ITradeItem _tradeItem;
		private InputField SearchField;
		private MarketController mc;
		private Transform itemDetailTransform;
		private VerticalScrollPanel itemListScroll;
		private ObjectPool<GameObject> itemPanelPool;
		private List<GameObject> itemListChildren;
		private string _lastSearch;

		//public properties
		public ITradeItem TradeItem
		{
			get { return _tradeItem; }
			set
			{
				if (_tradeItem != value)
				{
					_tradeItem = value;
					OnItemChange(value);
				}
			}
		}
		public UnityAction<ITradeItem> OnItemChange { get; set; }

		public void Awake()
		{
			mc = FindObjectOfType<MarketController>();
			itemListChildren = new List<GameObject>();

			SearchField = transform.Find("Search").GetComponentInChildren<InputField>();
			SearchField.onValueChanged.AddListener(OnValueChanged);

			itemDetailTransform = transform.parent.parent.Find("ItemDetail");
			itemListScroll = itemDetailTransform.Find("ItemListScroll").GetComponent<VerticalScrollPanel>();

			itemPanelPool = new ObjectPool<GameObject>(20, () => {
				var item = Instantiate(Resources.Load("ItemPanel"), itemListScroll.List.transform) as GameObject;
				item.SetActive(false);
				return item;
			});
		}

		public void Reinitialize()
		{
			SearchField.text = INITIAL_ITEM;
			TradeItem = null;
		}

		private void OnValueChanged(string searchTerm)
		{
			if (_lastSearch == searchTerm)
			{
				return;
			}

			_lastSearch = searchTerm;
			TradeItem = null;
			itemListChildren.ForEach(child => { child.SetActive(false); itemPanelPool.Release(child); });
			itemListChildren = new List<GameObject>();
			itemListScroll.gameObject.SetActive(searchTerm.Length > 0);
			
			List<ITradeItem> items = mc.SearchForItems(searchTerm);

			items.ForEach(i => itemListChildren.Add(Create_ItemPanel(i, itemListScroll.List)));
		}

		private GameObject Create_ItemPanel(ITradeItem item, GameObject parent)
		{
			var itemPanel = itemPanelPool.Get();
			itemPanel.SetActive(true);
			itemPanel.transform.SetAsLastSibling();
			var button = itemPanel.transform.Find("Button");
			button.GetComponentInChildren<Text>().text = item.Name;
			button.GetComponent<Button>().onClick.AddListener(() => OnItemClick(item));
			return itemPanel;
		}

		private void OnItemClick(ITradeItem item)
		{
			_lastSearch = item.Name;
			TradeItem = item;
			SearchField.text = item.Name;
			itemListScroll.gameObject.SetActive(false);
		}
	}
}
