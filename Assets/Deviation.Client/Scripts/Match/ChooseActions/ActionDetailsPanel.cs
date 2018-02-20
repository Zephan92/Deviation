using Assets.Scripts.Interface.DTO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Match
{
	public class ActionDetailsPanel : MonoBehaviour
	{
		public Text Name;
		public Text Description;
		public Image Image;

		public IExchangeAction Action;
		public bool Chosen;
		private bool _enabled;

		public void Awake()
		{
			var actionHeader = transform.Find("ActionHeader");
			var name = actionHeader.Find("Name");
			var description = actionHeader.Find("Description");
			var image = transform.Find("ActionImage");

			if (name != null)
			{
				Name = name.GetComponent<Text>();
				Name.text = "";
			}

			if (description != null)
			{
				Description = description.GetComponent<Text>();
				Description.text = "";
			}

			if (image != null)
			{
				Image = image.GetComponentInChildren<Image>();
			}
		}

		public void UpdateActionDetails(IExchangeAction action)
		{
			Action = action;

			if (Name != null)
			{
				Name.text = action.Name;
			}

			if (Description != null)
			{
				Description.text = action.Description;
			}

			if (Image != null)
			{

			}
		}

		//public void PanelEnabled(bool toggle)
		//{
		//	_enabled = toggle;
		//	Button.enabled = toggle;

		//	if (Chosen)
		//	{
		//		Image.color = Color.yellow;
		//	}
		//	else if (toggle)
		//	{
		//		Image.color = Color.white;
		//	}
		//	else
		//	{
		//		Image.color = Color.gray;
		//	}
		//}

		//public void Choose()
		//{
		//	Chosen = true;
		//	Button.enabled = false;
		//	Image.color = Color.yellow;
		//}

		//public void Unchoose()
		//{
		//	Chosen = false;
		//	Button.enabled = true;
		//	Image.color = Color.white;
		//}
	}
}
