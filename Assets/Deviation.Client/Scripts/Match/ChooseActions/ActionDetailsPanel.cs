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
	}
}
