using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Deviation.MasterServer.Scripts;
using Assets.Deviation.MasterServer.Scripts.MaterialBank;
using Assets.Deviation.Materials;
using Assets.Scripts.ModuleEditor;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client
{
	public class CraftingController : MonoBehaviour
	{
		public MaterialBag Bag;

		public Transform MenuBar;
		public Transform CraftActions;
		public Transform Materials;
		public Transform CraftInteface;

		public VerticalScrollPanel BaseMaterials;
		public VerticalScrollPanel SpecialMaterials;
		public VerticalScrollPanel TypeMaterials;

		public Transform BaseRecipeComponent;
		public Transform SpecialRecipeComponent;
		public Transform TypeRecipeComponent;

		private UnityAction<int> onBaseListChange;
		private UnityAction<int> onSpecialListChange;
		private UnityAction<int> onTypeListChange;

		private int numBaseMaterialRows;
		private int numSpecialMaterialRows;
		private int numTypeMaterialRows;

		public delegate bool onEndDrag(MaterialType type);

		public void Awake()
		{
			Bag = new MaterialBag();

			var parent = GameObject.Find("CraftingUI");
			MenuBar = parent.transform.Find("Menu Bar");
			CraftActions = parent.transform.Find("CraftActions");
			Materials = CraftActions.Find("Materials");
			CraftInteface = CraftActions.Find("Craft Interface");

			BaseMaterials = Materials.Find("Base").GetComponentInChildren<VerticalScrollPanel>();
			SpecialMaterials = Materials.Find("Special").GetComponentInChildren<VerticalScrollPanel>();
			TypeMaterials = Materials.Find("Type").GetComponentInChildren<VerticalScrollPanel>();

			BaseRecipeComponent = CraftInteface.Find("Recipe").Find("BaseComponent");
			SpecialRecipeComponent = CraftInteface.Find("Recipe").Find("SpecialComponent");
			TypeRecipeComponent = CraftInteface.Find("Recipe").Find("TypeComponent");

			onBaseListChange += BaseMaterials.OnListChange;
			onSpecialListChange += SpecialMaterials.OnListChange;
			onTypeListChange += TypeMaterials.OnListChange;

			numBaseMaterialRows = 0;
			numSpecialMaterialRows = 0;
			numTypeMaterialRows = 0;

			GetMaterialBag();
		}

		public int PopulateMaterialsList(VerticalScrollPanel panel, Dictionary<Materials.Material, int> materials)
		{
			int numRows = 1;

			foreach (Transform child in panel.List.transform)
			{
				//super inefficient..
				Destroy(child.gameObject);
			}

			GameObject materialRow = new GameObject("MaterialRow");
			CreateHorizontalLayout(materialRow);
			materialRow.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 75);
			materialRow.transform.SetParent(panel.List.transform);

			int i = 0;
			foreach (var material in materials.Keys)
			{
				if (i % 3 == 0 && i > 0)
				{
					numRows++;
					materialRow = new GameObject("MaterialRow");
					CreateHorizontalLayout(materialRow);
					materialRow.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 75);
					materialRow.transform.SetParent(panel.List.transform);
				}

				CreateMaterialPanel(material, materialRow);
				i++;
			}

			return numRows;
		}

		public GameObject CreateMaterialPanel(Materials.Material material, GameObject parent)
		{
			var materialPanel = Instantiate(Resources.Load("MaterialPanel"), parent.transform) as GameObject;
			var materialDetailsPanel = materialPanel.GetComponent<MaterialDetailsPanel>();
			materialDetailsPanel.UpdateMaterialDetails(material);
			CreateEventTriggers(materialPanel, ValidSnapCheck, material.Type);
			return materialPanel;
		}

		private bool ValidSnapCheck(SnapPoint snap, MaterialType type)
		{
			RecipeComponent component = snap.GetComponent<RecipeComponent>();
			return component.Type == type;
		}

		//need to move this to shared class
		private void CreateEventTriggers<T>(GameObject actionPanel, DragableUI.onEndDrag<T> onEndDrag, T type)
		{
			DragableUI dragable = actionPanel.AddComponent<DragableUI>();
			EventTrigger et = actionPanel.AddComponent<EventTrigger>();
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.BeginDrag, dragable, onEndDrag, type));
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.Drag, dragable, onEndDrag, type));
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.EndDrag, dragable, onEndDrag, type));
		}

		private EventTrigger.Entry CreateEventTriggerEntry<T>(EventTriggerType eventTriggerType, DragableUI dragable, DragableUI.onEndDrag<T> onEndDrag, T type)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = eventTriggerType;

			switch (eventTriggerType)
			{
				case EventTriggerType.BeginDrag:
					entry.callback.AddListener((eventData) => { dragable.BeginDrag(); });
					break;

				case EventTriggerType.Drag:
					entry.callback.AddListener((eventData) => { dragable.OnDrag(); });
					break;

				case EventTriggerType.EndDrag:
					entry.callback.AddListener((eventData) => { dragable.EndDrag(onEndDrag, type); });
					break;
			}

			return entry;
		}

		//move this toooooo
		public HorizontalLayoutGroup CreateHorizontalLayout(GameObject go)
		{
			var layout = go.AddComponent<HorizontalLayoutGroup>();
			layout.childControlWidth = false;
			layout.childControlHeight = true;
			layout.childForceExpandWidth = false;
			layout.childForceExpandHeight = true;
			return layout;
		}

		public VerticalLayoutGroup CreateVerticalLayout(GameObject go)
		{
			var layout = go.AddComponent<VerticalLayoutGroup>();
			layout.childControlWidth = true;
			layout.childControlHeight = false;
			layout.childForceExpandWidth = true;
			layout.childForceExpandHeight = false;
			return layout;
		}

		public void GetMaterial()
		{
			Msf.Client.Connection.SendMessage((short)MaterialBankOpCodes.GetMaterials, (status, response) =>
			{
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogError($"GetMaterial failed. Error {response}");
				}
				else if (status == ResponseStatus.Success)
				{
					MaterialsPacket packet = response.Deserialize(new MaterialsPacket());
					var material = packet.Materials;
					Bag.AddMaterial(material);
					RefreshMaterialLists();
				}
			});
		}

		public void GetMaterialBag()
		{
			//change this to actually get users material bag from DB
			int counter = 0;
			while (counter < 6)
			{
				GetMaterial();
				counter++;
			}
		}

		public void RefreshMaterialLists()
		{
			BaseRecipeComponent.GetComponentInChildren<DragableUI>()?.ReturnToOrignalParent();
			SpecialRecipeComponent.GetComponentInChildren<DragableUI>()?.ReturnToOrignalParent();
			TypeRecipeComponent.GetComponentInChildren<DragableUI>()?.ReturnToOrignalParent();

			numBaseMaterialRows = PopulateMaterialsList(BaseMaterials, Bag.MaterialsByType(MaterialType.Base));
			numSpecialMaterialRows = PopulateMaterialsList(SpecialMaterials, Bag.MaterialsByType(MaterialType.Special));
			numTypeMaterialRows = PopulateMaterialsList(TypeMaterials, Bag.MaterialsByType(MaterialType.Type));

			onBaseListChange(numBaseMaterialRows);
			onSpecialListChange(numSpecialMaterialRows);
			onTypeListChange(numTypeMaterialRows);
		}

		public void GetMaterialBank()
		{
			Msf.Client.Connection.SendMessage((short)MaterialBankOpCodes.GetMaterialBank, (status, response) =>
			{
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogErrorFormat("GetMaterialBank failed. Error {1}", response);
				}
				else if (status == ResponseStatus.Success)
				{
					MaterialBankPacket packet = response.Deserialize(new MaterialBankPacket());
					var materialBank = packet.MaterialBank;
					UnityEngine.Debug.Log(materialBank.ToString());
				}
			});
		}
	}
}
