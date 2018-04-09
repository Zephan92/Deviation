using Assets.Scripts.Interface.DTO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Assets.Deviation.Materials;

namespace Assets.Deviation.Client.Scripts.Client
{
	public class MaterialDetailsPanel : MonoBehaviour
	{
		public Text Name;
		public Image Image;

		public Materials.Material Material;

		public void Awake()
		{
			var name = transform.Find("Name");
			var image = transform.Find("Image");

			if (name != null)
			{
				Name = name.GetComponent<Text>();
				Name.text = "";
			}

			if (image != null)
			{
				Image = image.GetComponentInChildren<Image>();
			}
		}

		public void UpdateMaterialDetails(Materials.Material material)
		{
			Material = material;

			if (Name != null)
			{
				Name.text = material.Name;
			}

			if (Image != null)
			{

			}
		}
	}
}
