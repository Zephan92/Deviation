using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Scripts.ModuleEditor;
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
	public class ResourceHotspot : MonoBehaviour
	{
		public string ResourceName;
		public Button ResourceButton;
		public Transform ResourceUI;
		public Text ResourceText;
		public Transform Upgrades;
		public int ResourceCount;
		public int OnClickRate = 1;
		public float OnClickModifier = 1f;
		public int CurrentUpgradeCost = 100;
		public int NumUpgrades = 0;
		public int PotentialUpgrades = 0;

		public void Awake()
		{
			ResourceName = gameObject.name;
			ResourceButton = GetComponentInChildren<Button>();
			ResourceUI = transform.Find("ResourceUI");
			Upgrades = transform.Find("Upgrades");
			ResourceText = ResourceUI.GetComponentInChildren<Text>();
			ResourceButton.onClick.AddListener(ResourceOnClick);
		}

		public void Start()
		{
			ResourceCount = 0;//should retrieve last known count
			ResourceText.text = $"{ResourceName}: {ResourceCount}";

			StartCoroutine(SpawnResourceUpgrade());
			StartCoroutine(GainResources());

		}

		public void ResourceOnClick()
		{
			AddResources((int)(OnClickRate * OnClickModifier));
		}

		public void AddResources(int add)
		{
			ResourceCount += add;
			ResourceText.text = $"{ResourceName}: {ResourceCount}";
		}

		public IEnumerator SpawnResourceUpgrade()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.5f);
				int rand = UnityEngine.Random.Range(0, 2500);
				if (rand < 150)
				{
					int currentUpgradeCost = CurrentUpgradeCost;
					for (int i = 0; i < PotentialUpgrades; i++)
					{
						currentUpgradeCost += CalculateUpgradeCost(currentUpgradeCost);
					}

					if (ResourceCount > currentUpgradeCost)
					{
						SpawnUpgrade();
					}
				}
			}
		}

		public IEnumerator GainResources()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.5f);
				AddResources((int)( OnClickRate * OnClickModifier));
			}
		}

		public void SpawnUpgrade()
		{
			PotentialUpgrades++;
			GameObject upgrade = new GameObject($"{ResourceName} Upgrade");
			upgrade.transform.SetParent(Upgrades);
			upgrade.AddComponent<CanvasRenderer>();
			Image i = upgrade.AddComponent<Image>();
			i.color = Color.red;
			var draggable = DragableUIFactory.CreateDraggableUI(upgrade, ValidSnapCheck, ResourceName);
			RectTransform parentRect = Upgrades.GetComponent<RectTransform>();
			float randX = UnityEngine.Random.Range(parentRect.sizeDelta.x / -2f, parentRect.sizeDelta.x / 2f);
			float randY = UnityEngine.Random.Range(parentRect.sizeDelta.y / -2f, parentRect.sizeDelta.y / 2f);
			RectTransform upgradeRect = upgrade.GetComponent<RectTransform>();
			upgradeRect.localPosition = new Vector2(randX, randY);
			var coroutine = StartCoroutine(DestroyUpgrade(upgrade));
			draggable.OnBeginDragAction += () => StopCoroutine(coroutine);
			draggable.OnEndDragSuccessAction += () => OnUpgradeDragEnd(upgrade, coroutine);
			draggable.OnEndDragFailureAction += () => coroutine = StartCoroutine(DestroyUpgrade(upgrade));
		}

		private bool ValidSnapCheck(SnapPoint snap, string name)
		{
			ResourceHotspot hotspot = snap.GetComponent<ResourceHotspot>();
			return hotspot?.ResourceName == name;
		}

		private void OnUpgradeDragEnd(GameObject upgrade, Coroutine coroutine)
		{
			PotentialUpgrades--;
			NumUpgrades++;
			OnClickRate += CalculateOnclickRate(NumUpgrades);
			AddResources(-CurrentUpgradeCost);
			CurrentUpgradeCost = CalculateUpgradeCost(CurrentUpgradeCost);
			StopCoroutine(coroutine);
			Destroy(upgrade);
		}

		private int CalculateOnclickRate(int numUpgrades)
		{
			return (int) Math.Pow(numUpgrades, 2);
		}

		private int CalculateUpgradeCost(int currentUpgradeCost)
		{
			return (int)Math.Pow(currentUpgradeCost, 1.2);
		}

		public IEnumerator DestroyUpgrade(GameObject go)
		{
			yield return new WaitForSeconds(10f);
			PotentialUpgrades--;
			Destroy(go);
			yield break;
		}
	}
}
