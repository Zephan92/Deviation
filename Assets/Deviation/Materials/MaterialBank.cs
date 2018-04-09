using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Deviation.Materials
{
	public class MaterialBankObject
	{
		private Random Random = new Random();
		private MaterialBankTypeBase _materialBankType;

		public Dictionary<Material, int> Materials;
		private Dictionary<Rarity, List<Material>> _materialsByRarity;
		private int _materialCount = 0;

		public MaterialBankObject(Dictionary<Material, int> materials, MaterialBankTypeBase type)
		{
			Materials = materials;

			_materialsByRarity = new Dictionary<Rarity, List<Material>>();
			_materialBankType = type;

			foreach (Material material in materials.Keys)
			{
				if (_materialsByRarity.ContainsKey(material.Rarity))
				{
					_materialsByRarity[material.Rarity].Add(material);
				}
				else
				{
					_materialsByRarity.Add(material.Rarity, new List<Material>() { material });
				}
			}

			foreach (int count in materials.Values)
			{
				_materialCount += count;
			}
		}

		public int Count()
		{
			return _materialCount;
		}

		public Material GetRandomMaterial(Rarity rarityStart = Rarity.Common, int modifier = 0)
		{
			if (Empty())
			{
				throw new Exception("The Materal Bank was empty.");
			}

			if (rarityStart == Rarity.Default)
			{
				rarityStart = Rarity.Common;
			}

			Material retval = new Material();

			int count = 0;
			for (int i = 0; i < (int)rarityStart; i++)
			{
				count += MateralCount((Rarity)i);
			}

			int min = Math.Min(count + modifier, _materialCount - 1);

			int ranInt = Random.Next(min, _materialCount);

			for (int i = (int) rarityStart; i < (int)Rarity.Count; i++)
			{
				Rarity curRarity = (Rarity)i;

				count += MateralCount(curRarity);

				if (ranInt <= count)
				{
					var materialsList = _materialsByRarity[curRarity];
					if (materialsList.Count > 0)
					{
						retval = ChooseRandomMateralFromList(materialsList);
					}
					else
					{
						retval = GetRandomMaterial(rarityStart - 1, modifier);
					}
					break;
				}
			}

			RemoveMateral(retval);
			return retval;
		}

		public bool Empty()
		{
			return _materialCount <= 0;
		}

		public int MateralCount(Rarity rarity)
		{
			int count = 0;

			if (_materialsByRarity.ContainsKey(rarity))
			{
				_materialsByRarity[rarity].ForEach(material => count += Materials[material]);
			}
			return count;
		}

		private Material ChooseRandomMateralFromList(List<Material> materials)
		{
			int ranInt = Random.Next(0, materials.Count);
			return materials[ranInt];
		}

		private void RemoveMateral(Material material)
		{
			Materials[material] -= 1;
			_materialCount--;

			if (Materials[material] == 0)
			{
				_materialsByRarity[material.Rarity].Remove(material);
			}
		}

		public override string ToString()
		{
			string retval = "MateralBank - Count: " + _materialCount + "\n";

			foreach (Material material in Materials.Keys)
			{
				retval += material.Name + ": " + Materials[material] + "\n";
			}
			return retval;
		}

		public string MaterialBankType()
		{
			string[] typeName = _materialBankType.ToString().Split('.');

			return typeName[typeName.Length - 1];
		}
	}
}
