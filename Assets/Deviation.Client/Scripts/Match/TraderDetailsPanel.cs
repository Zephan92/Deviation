using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Match
{
	public class TraderDetailsPanel : MonoBehaviour
	{
		public Text Name;
		public Text Title;
		public Text Type;
		public Text Description;
		public Button OnClick;
		public ITrader Trader;
		public Image Image;
		private bool _enabled;
		public bool Chosen;
		public UnityAction<ITrader> OnChosenEvent;

		public void UpdateTraderDetails(ITrader trader, UnityAction action = null)
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
				OnClick.onClick.AddListener(action);
				OnClick.onClick.AddListener(OnChosen);
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

		public void OnChosen()
		{
			Chosen = true;
			OnClick.enabled = false;
			Image.color = Color.yellow;
			OnChosenEvent(Trader);
		}

		public void Unchoose()
		{
			Chosen = false;
			OnClick.enabled = true;
			Image.color = Color.white;
		}
	}
}
