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
		public Button Button;
		public Image Image;

		public ITrader Trader;
		public bool Chosen;

		public void Awake()
		{
			var name = transform.Find("Name");
			var title = transform.Find("Title");
			var type = transform.Find("TraderTypeLogo");
			var description = transform.Find("Description");
			var button = transform.Find("Button");
			var image = transform.Find("Image");

			if (name != null)
			{
				Name = name.GetComponent<Text>();
				Name.text = "";
			}

			if (title != null)
			{
				Title = title.GetComponent<Text>();
				Title.text = "";
			}

			if (type != null)
			{
				Type = type.GetComponentInChildren<Text>();
				Type.text = "";
			}

			if (description != null)
			{
				Description = description.GetComponent<Text>();
				Description.text = "";
			}

			if (button != null)
			{
				Button = button.GetComponent<Button>();
				Button.interactable = false;
			}
			else
			{
				Button = GetComponent<Button>();
				Button.interactable = false;
			}

			if (image != null)
			{
				Image = image.GetComponentInChildren<Image>();
			}
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

			if (Button != null)
			{
				Button.onClick.RemoveAllListeners();
				if (onClickAction != null)
				{
					Button.onClick.AddListener(onClickAction);
				}
				Button.onClick.AddListener(Choose);
				Button.interactable = true;
			}

			if (Image != null)
			{

			}
		}

		public void PanelEnabled(bool toggle)
		{
			Button.enabled = toggle;

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
			Button.enabled = false;
			Image.color = Color.yellow;
		}

		public void Unchoose()
		{
			Chosen = false;
			Button.enabled = true;
			Image.color = Color.white;
		}
	}
}
