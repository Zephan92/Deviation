using Assets.Scripts.Interface.DTO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Assets.Deviation.Materials;
using System.Collections.Generic;

namespace Assets.Deviation.Client.Scripts.Client
{
	public class MaterialDetailsPanel : MonoBehaviour
	{
		public Text Name;
		public Image Image;
		public Text Count;

		public Materials.Material Material;

		public void Awake()
		{
			var name = transform.Find("Name");
			var image = transform.Find("Image");
			var count = transform.Find("Count");

			if (name)
			{
				Name = name?.GetComponent<Text>();
				Name.text = "";
			}

			if (count)
			{
				Count = count?.GetComponent<Text>();
				Count.text = "";
			}

			Image = image?.GetComponentInChildren<Image>();
		}

		public void UpdateMaterialDetails(Materials.Material material, int count)
		{
			Material = material;

			if (Name)
			{
				Name.text = material.Name;
			}

			if (Count)
			{
				Count.text = $"x{count}";
			}
		}
	}
}
