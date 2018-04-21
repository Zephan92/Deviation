using Assets.Deviation.Client.Scripts.Match;
using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Deviation.MasterServer.Scripts;
using Assets.Deviation.MasterServer.Scripts.MaterialBank;
using Assets.Deviation.Materials;
using Assets.Deviation.Recipes;
using Assets.Scripts.Library;
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
		public HashSet<Recipe> Recipes;

		public Transform MenuBar;
		public Transform CraftActions;
		public Transform Materials;
		public Transform CraftInteface;

		public VerticalScrollPanel BaseMaterials;
		public VerticalScrollPanel SpecialMaterials;
		public VerticalScrollPanel TypeMaterials;
		public VerticalScrollPanel RecipeScrollPanel;

		public Transform BaseRecipeComponent;
		public Transform SpecialRecipeComponent;
		public Transform TypeRecipeComponent;

		public Button CreateButton;

		public ActionDetailsPanel ActionPanel;

		private UnityAction<int> onBaseListChange;
		private UnityAction<int> onSpecialListChange;
		private UnityAction<int> onTypeListChange;
		private UnityAction<int> onRecipesListChange;

		private int numBaseMaterialRows;
		private int numSpecialMaterialRows;
		private int numTypeMaterialRows;

		private bool baseComponentReady;
		private bool specialComponentReady;
		private bool typeComponentReady;

		private bool _initialized = false;

		public delegate bool onEndDrag(MaterialType type);

		public void Awake()
		{
			Bag = new MaterialBag();
			Recipes = new HashSet<Recipe>();

			var parent = GameObject.Find("CraftingUI");
			MenuBar = parent.transform.Find("Menu Bar");
			CraftActions = parent.transform.Find("CraftActions");
			Materials = CraftActions.Find("Materials");
			CraftInteface = CraftActions.Find("Craft Interface");

			BaseMaterials = Materials.Find("Base").GetComponentInChildren<VerticalScrollPanel>();
			SpecialMaterials = Materials.Find("Special").GetComponentInChildren<VerticalScrollPanel>();
			TypeMaterials = Materials.Find("Type").GetComponentInChildren<VerticalScrollPanel>();

			RecipeScrollPanel = CraftActions.Find("Recipes").GetComponentInChildren<VerticalScrollPanel>();

			BaseRecipeComponent = CraftInteface.Find("Recipe").Find("BaseComponent");
			SpecialRecipeComponent = CraftInteface.Find("Recipe").Find("SpecialComponent");
			TypeRecipeComponent = CraftInteface.Find("Recipe").Find("TypeComponent");

			BaseRecipeComponent.GetComponent<SnapPoint>().onOccupied += () => { ComponentStateChanged(MaterialType.Base, true); };
			SpecialRecipeComponent.GetComponent<SnapPoint>().onOccupied += () => { ComponentStateChanged(MaterialType.Special, true); };
			TypeRecipeComponent.GetComponent<SnapPoint>().onOccupied += () => { ComponentStateChanged(MaterialType.Type, true); };

			BaseRecipeComponent.GetComponent<SnapPoint>().onUnoccupied += () => { ComponentStateChanged(MaterialType.Base, false); };
			SpecialRecipeComponent.GetComponent<SnapPoint>().onUnoccupied += () => { ComponentStateChanged(MaterialType.Special, false); };
			TypeRecipeComponent.GetComponent<SnapPoint>().onUnoccupied += () => { ComponentStateChanged(MaterialType.Type, false); };

			CreateButton = CraftInteface.Find("Output").GetComponentInChildren<Button>();
			ActionPanel = CraftInteface.Find("Output").GetComponentInChildren<ActionDetailsPanel>();

			onBaseListChange += BaseMaterials.OnListChange;
			onSpecialListChange += SpecialMaterials.OnListChange;
			onTypeListChange += TypeMaterials.OnListChange;
			onRecipesListChange += RecipeScrollPanel.OnListChange;

			numBaseMaterialRows = 0;
			numSpecialMaterialRows = 0;
			numTypeMaterialRows = 0;

			CreateButton.onClick.AddListener(Craft);

			GetMaterialBag();
		}

		public void Start()
		{
			_initialized = true;
		}

		public void OnEnable()
		{
			if (_initialized)
			{
				RefreshMaterialLists();
				PopulateRecipesList();
			}
		}

		public void Craft()
		{
			Recipe recipe = GetRecipeFromComponents();
			Recipes.Add(recipe);
			PopulateRecipesList();
			Debug.Log("Crafting!");
		}

		public void RefreshMaterialLists()
		{
			if (BaseMaterials.List == null || SpecialMaterials.List == null || TypeMaterials.List == null)
			{
				return;
			}

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

		private int PopulateMaterialsList(VerticalScrollPanel panel, Dictionary<Materials.Material, int> materials)
		{
			int numRows = 1;

			foreach (Transform child in panel.List.transform)
			{
				//super inefficient..
				Destroy(child.gameObject);
			}

			GameObject materialRow = new GameObject("MaterialRow");
			LayoutGroupFactory.CreateHorizontalLayout(materialRow);
			materialRow.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 75);
			materialRow.transform.SetParent(panel.List.transform);

			int i = 0;
			foreach (var material in materials)
			{
				if (i % 3 == 0 && i > 0)
				{
					numRows++;
					materialRow = new GameObject("MaterialRow");
					LayoutGroupFactory.CreateHorizontalLayout(materialRow);
					materialRow.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 75);
					materialRow.transform.SetParent(panel.List.transform);
				}

				CreateMaterialPanel(material, materialRow);
				i++;
			}

			return numRows;
		}

		private void PopulateRecipesList()
		{
			foreach (Transform child in RecipeScrollPanel.List.transform)
			{
				//super inefficient..
				Destroy(child.gameObject);
			}

			foreach (var recipe in Recipes)
			{
				CreateRecipePanel(recipe);
			}

			onRecipesListChange(Recipes.Count);
		}

		private GameObject CreateMaterialPanel(KeyValuePair<Materials.Material, int> material, GameObject parent)
		{
			var materialPanel = Instantiate(Resources.Load("MaterialPanel"), parent.transform) as GameObject;
			var materialDetailsPanel = materialPanel.GetComponent<MaterialDetailsPanel>();
			materialDetailsPanel.UpdateMaterialDetails(material.Key, material.Value);
			DragableUIFactory.CreateDraggableUI(materialPanel, ValidSnapCheck, material.Key.Type);
			return materialPanel;
		}

		private GameObject CreateRecipePanel(Recipe recipe)
		{
			var recipePanel = Instantiate(Resources.Load("RecipePanel"), RecipeScrollPanel.List.transform) as GameObject;
			var recipeDetailsPanel = recipePanel.GetComponent<RecipeDetailsPanel>();
			recipeDetailsPanel.UpdateRecipeDetails(recipe);
			return recipePanel;
		}

		private bool ValidSnapCheck(SnapPoint snap, MaterialType type)
		{
			RecipeComponent component = snap.GetComponent<RecipeComponent>();
			return component.Type == type;
		}

		private void ComponentStateChanged(MaterialType type, bool state)
		{
			switch (type)
			{
				case MaterialType.Base:
					baseComponentReady = state;
					break;

				case MaterialType.Special:
					specialComponentReady = state;
					break;

				case MaterialType.Type:
					typeComponentReady = state;
					break;
			}

			if (baseComponentReady && specialComponentReady && typeComponentReady)
			{
				UpdateRecipeResult(true);
			}
			else
			{
				UpdateRecipeResult(false);
			}

		}

		private void UpdateRecipeResult(bool recipeReady)
		{
			if (!recipeReady || !RecipeExistFromComponent())
			{
				ActionPanel.ResetActionDetails();
				CreateButton.interactable = false;
				return;
			}

			Recipe recipe = GetRecipeFromComponents();
			ActionPanel.UpdateActionDetails(recipe.Action);
			CreateButton.interactable = true;
		}

		private bool RecipeExistFromComponent()
		{
			Materials.Material baseMaterial = BaseRecipeComponent.GetComponentInChildren<MaterialDetailsPanel>().Material;
			Materials.Material specialMaterial = SpecialRecipeComponent.GetComponentInChildren<MaterialDetailsPanel>().Material;
			Materials.Material typeMaterial = TypeRecipeComponent.GetComponentInChildren<MaterialDetailsPanel>().Material;

			return RecipeLibrary.RecipeExists(baseMaterial, specialMaterial, typeMaterial);
		}

		private Recipe GetRecipeFromComponents()
		{
			Materials.Material baseMaterial = BaseRecipeComponent.GetComponentInChildren<MaterialDetailsPanel>().Material;
			Materials.Material specialMaterial = SpecialRecipeComponent.GetComponentInChildren<MaterialDetailsPanel>().Material;
			Materials.Material typeMaterial = TypeRecipeComponent.GetComponentInChildren<MaterialDetailsPanel>().Material;

			if (RecipeLibrary.RecipeExists(baseMaterial, specialMaterial, typeMaterial))
			{
				return RecipeLibrary.GetRecipe(baseMaterial, specialMaterial, typeMaterial);
			}
			else
			{
				return null;

			}

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
