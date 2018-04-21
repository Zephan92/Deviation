using Assets.Deviation.Materials;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Deviation.Recipes
{
	public class RecipeLibrary
	{
		private static HashSet<Recipe> _all = new HashSet<Recipe>();

		public static HashSet<Recipe> GetRecipes()
		{
			if (_all.Count == 0)
			{
				Alpha.ToList().ForEach(x => _all.Add(x));
				//Uncommon.ToList().ForEach(x => _all.Add(x));
				//Rare.ToList().ForEach(x => _all.Add(x));
				//Mythic.ToList().ForEach(x => _all.Add(x));
				//Legendary.ToList().ForEach(x => _all.Add(x));
			}

			return _all;
		}

		public static HashSet<Recipe> GetRecipes(TraderType type)
		{
			switch (type)
			{
				case TraderType.Alpha:
					return Alpha;

				default:
					return GetRecipes();
			}
		}

		public static Recipe GetRecipe(string actionName)
		{
			var recipes = GetRecipes();
			return recipes.First(recipe => recipe.Action.Name == actionName);
		}

		public static Recipe GetRecipe(IExchangeAction action)
		{
			var recipes = GetRecipes();
			return recipes.First(recipe => recipe.Action.Equals(action));
		}

		public static Recipe GetRecipe(string baseMaterial, string specialMaterial, string typeMaterial)
		{
			var recipes = GetRecipes();
			if (RecipeExists(baseMaterial, specialMaterial, typeMaterial))
			{
				return recipes.First(material => material.Equals(baseMaterial, specialMaterial, typeMaterial));
			}
			else
			{
				return null;
			}
		}

		public static Recipe GetRecipe(Material baseMaterial, Material specialMaterial, Material typeMaterial)
		{
			var recipes = GetRecipes();
			if (RecipeExists(baseMaterial, specialMaterial, typeMaterial))
			{
				return recipes.First(material => material.Equals(baseMaterial, specialMaterial, typeMaterial));
			}
			else
			{
				return null;
			}
		}

		public static bool RecipeExists(string baseMaterial, string specialMaterial, string typeMaterial)
		{
			return GetRecipes().Any(material => material.Equals(baseMaterial, specialMaterial, typeMaterial));
		}

		public static bool RecipeExists(Material baseMaterial, Material specialMaterial, Material typeMaterial)
		{
			return GetRecipes().Any(material => material.Equals(baseMaterial, specialMaterial, typeMaterial));
		}

		private static HashSet<Recipe> Alpha = new HashSet<Recipe>
		{
			{ new Recipe(action: "Ambush", baseMaterial: "Rock", specialMaterial: "Dark Quaff", typeMaterial: "Herb")},
			{ new Recipe(action: "ShockWave", baseMaterial: "Rock", specialMaterial: "Turk Mech", typeMaterial: "Chalk")},

		};
	}
}
