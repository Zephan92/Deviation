using Assets.Deviation.Client.Scripts.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Odysseys
{
	public class HotspotController : MonoBehaviour
	{
		public ResourceHotspot[] Hotspots;
		public Transform Modifiers;
		public bool ModifierActive = false;

		public void Awake()
		{
			Hotspots = FindObjectsOfType<ResourceHotspot>();
			Modifiers = transform.Find("Modifiers");
			StartCoroutine(SpawnClickModifier());
		}

		public IEnumerator SpawnClickModifier()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.5f);
				int rand = UnityEngine.Random.Range(0, 20000);
				if (rand < 150)
				{
					SpawnModifier();
				}
			}
		}

		public void SpawnModifier()
		{
			GameObject modifier = new GameObject($"Click Modifier");
			modifier.transform.SetParent(Modifiers);
			modifier.AddComponent<CanvasRenderer>();
			Button button = modifier.AddComponent<Button>();
			button.onClick.AddListener(() => ModifierClicked(modifier));
			Image i = modifier.AddComponent<Image>();
			i.color = Color.blue;
			RectTransform parentRect = Modifiers.GetComponent<RectTransform>();
			float randX = UnityEngine.Random.Range(parentRect.sizeDelta.x / -2f, parentRect.sizeDelta.x / 2f);
			float randY = UnityEngine.Random.Range(parentRect.sizeDelta.y / -2f, parentRect.sizeDelta.y / 2f);
			RectTransform modifierRect = modifier.GetComponent<RectTransform>();
			modifierRect.localPosition = new Vector2(randX, randY);
			Destroy(modifier, 10f);
		}

		public void ModifierClicked(GameObject go)
		{
			foreach (ResourceHotspot hotspot in Hotspots)
			{
				hotspot.OnClickModifier = 13f;
			}
			ModifierActive = true;
			StartCoroutine(ResetModifiers());
			Destroy(go);
		}

		public IEnumerator ResetModifiers()
		{
			yield return new WaitForSeconds(5f);
			foreach (ResourceHotspot hotspot in Hotspots)
			{
				//this resets all modifiers, I should fix it to only remove a single modifier
				hotspot.OnClickModifier = 1f;
			}
			ModifierActive = false;
		}
	}
}
