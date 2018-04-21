using Assets.Deviation.Client.Scripts.Client;
using Assets.Deviation.Client.Scripts.Match;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Recipes
{
	public class RecipeDetailsPanel : MonoBehaviour
	{
		public Text Name;

		public MaterialDetailsPanel BaseMaterial;
		public MaterialDetailsPanel SpecialMaterial;
		public MaterialDetailsPanel TypeMaterial;

		public ActionDetailsPanel ActionDetailsPanel;

		public Recipe Recipe;

		public void Awake()
		{
			var name = transform.Find("Name");
			var count = transform.Find("Count");

			if (name)
			{
				Name = name?.GetComponent<Text>();
				Name.text = "";
			}

			BaseMaterial = transform.Find("Materials")?.Find("BaseMaterial")?.GetComponent<MaterialDetailsPanel>();
			SpecialMaterial = transform.Find("Materials")?.Find("SpecialMaterial")?.GetComponent<MaterialDetailsPanel>();
			TypeMaterial = transform.Find("Materials")?.Find("TypeMaterial")?.GetComponent<MaterialDetailsPanel>();

			ActionDetailsPanel = transform.Find("ActionPanel")?.GetComponent<ActionDetailsPanel>();
		}

		public void UpdateRecipeDetails(Recipe recipe)
		{
			Recipe = recipe;

			if (Name)
			{
				Name.text = Recipe.Action.Name;
			}

			BaseMaterial?.UpdateMaterialDetails(recipe.BaseMaterial);
			SpecialMaterial?.UpdateMaterialDetails(recipe.SpecialMaterial);
			TypeMaterial?.UpdateMaterialDetails(recipe.TypeMaterial);

			ActionDetailsPanel?.UpdateActionDetails(recipe.Action);
		}
	}
}
