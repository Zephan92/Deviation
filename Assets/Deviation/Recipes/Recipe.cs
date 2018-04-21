using Assets.Deviation.Materials;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;

namespace Assets.Deviation.Recipes
{
	public class Recipe
	{
		public Material BaseMaterial;
		public Material SpecialMaterial;
		public Material TypeMaterial;

		public IExchangeAction Action;

		public Recipe(Material baseMaterial, Material specialMaterial, Material typeMaterial, IExchangeAction action)
		{
			BaseMaterial = baseMaterial;
			SpecialMaterial = specialMaterial;
			TypeMaterial = typeMaterial;
			Action = action;
		}

		public Recipe(string baseMaterial, string specialMaterial, string typeMaterial, string action)
		{
			BaseMaterial = MaterialLibrary.GetMaterial(baseMaterial);
			SpecialMaterial = MaterialLibrary.GetMaterial(specialMaterial);
			TypeMaterial = MaterialLibrary.GetMaterial(typeMaterial);
			Action = ActionLibrary.GetActionInstance(action);
		}

		public bool Equals(string baseMaterialName, string specialMaterialName, string typeMaterialName)
		{
			var baseMaterial = MaterialLibrary.GetMaterial(baseMaterialName);
			var specialMaterial = MaterialLibrary.GetMaterial(specialMaterialName);
			var typeMaterial = MaterialLibrary.GetMaterial(typeMaterialName);

			return BaseMaterial.Equals(baseMaterial) && SpecialMaterial.Equals(specialMaterial) && TypeMaterial.Equals(typeMaterial);
		}

		public bool Equals(Material baseMaterial, Material specialMaterial, Material typeMaterial)
		{
			return BaseMaterial.Equals(baseMaterial) && SpecialMaterial.Equals(specialMaterial) && TypeMaterial.Equals(typeMaterial);
		}
	}
}
