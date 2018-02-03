using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Match
{
	public class TraderDetailsPanel : MonoBehaviour
	{
		//get these dynamicly
		public Text Name;
		public Text Title;
		public Text Type;
		public Text Description;
		public Button OnClick;
		public Image Image;

		public ITrader Trader;
		public bool Chosen;
		private bool _enabled;

		public void Awake()
		{
			
		}

		public void UpdateTraderDetails(ITrader trader, UnityAction onClickAction = null)
		{
			Trader = trader;

			if (Name != null)
			{
				Name.text = trader.Name;
			}

			if (Title != null)
			{
				Title.text = trader.Title;
			}

			if (Type != null)
			{
				Type.text = trader.Type.ToString();
			}

			if (Description != null)
			{
				Description.text = trader.Description;
			}

			if (OnClick != null)
			{
				OnClick.onClick.AddListener(onClickAction);
				OnClick.onClick.AddListener(Choose);
			}

			if (Image != null)
			{

			}
		}

		public void PanelEnabled(bool toggle)
		{
			_enabled = toggle;
			OnClick.enabled = toggle;

			if (Chosen)
			{
				Image.color = Color.yellow;
			}
			else if (toggle)
			{
				Image.color = Color.white;
			}
			else
			{
				Image.color = Color.gray;
			}
		}

		public void Choose()
		{
			Chosen = true;
			OnClick.enabled = false;
			Image.color = Color.yellow;
		}

		public void Unchoose()
		{
			Chosen = false;
			OnClick.enabled = true;
			Image.color = Color.white;
		}
	}
}
