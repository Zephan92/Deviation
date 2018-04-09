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

			Name = name?.GetComponent<Text>();
			Description = description?.GetComponent<Text>();
			Image = image?.GetComponentInChildren<Image>();

			ResetActionDetails();
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
				Image.enabled = true;
			}
		}

		public void ResetActionDetails()
		{
			Action = null;

			if (Name != null)
			{
				Name.text = "";
			}

			if (Description != null)
			{
				Description.text = "";
			}

			if (Image != null)
			{
				Image.enabled = false;
			}
		}
	}
}
