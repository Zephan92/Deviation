using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Interface.DTO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Match
{
	public class KitDetailsPanel : MonoBehaviour
	{
		public Text Name;
		public Image Image;

		public IKit Kit;

		public void Awake()
		{
			var kitHeader = transform.Find("KitHeader");
			var name = kitHeader.Find("Name");
			var image = transform.Find("KitImage");

			Name = name?.GetComponent<Text>();
			Image = image?.GetComponentInChildren<Image>();

			ResetActionDetails();
		}

		public void UpdateKitDetails(IKit kit)
		{
			Kit = kit;

			if (Name != null)
			{
				Name.text = kit.Name;
			}

			if (Image != null)
			{
				Image.enabled = true;
			}
		}

		public void ResetActionDetails()
		{
			Kit = null;

			if (Name != null)
			{
				Name.text = "";
			}

			if (Image != null)
			{
				Image.enabled = false;
			}
		}
	}
}
