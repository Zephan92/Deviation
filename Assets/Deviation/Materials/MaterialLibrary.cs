using System.Collections.Generic;
using System.Linq;

namespace Assets.Deviation.Materials
{
	public class MaterialLibrary
	{
		private static HashSet<Material> _all = new HashSet<Material>();

		public static HashSet<Material> GetMaterials()
		{
			if (_all.Count == 0)
			{
				Common.ToList().ForEach(x => _all.Add(x));
				Uncommon.ToList().ForEach(x => _all.Add(x));
				Rare.ToList().ForEach(x => _all.Add(x));
				Mythic.ToList().ForEach(x => _all.Add(x));
				Legendary.ToList().ForEach(x => _all.Add(x));
			}

			return _all;
		}

		public static HashSet<Material> GetMaterials(Rarity rarity)
		{
			switch (rarity)
			{
				case Rarity.Common:
					return Common;

				case Rarity.Uncommon:
					return Uncommon;

				case Rarity.Rare:
					return Rare;

				case Rarity.Mythic:
					return Mythic;

				case Rarity.Legendary:
					return Legendary;

				default:
					return GetMaterials();
			}
		}

		public static Material GetMaterial(string materialName)
		{
			var materials = GetMaterials();
			if (MaterialExists(materialName))
			{
				return materials.First(material => material.Name == materialName);
			}
			else
			{
				return new Material();
			}
		}

		public static bool MaterialExists(string materialName)
		{
			///hmmmmm
			return GetMaterials().Any(material => material.Name == materialName);
		}

		

		private static HashSet<Material> Common = new HashSet<Material>
		{
			{ new Material(name: "Rock", type: MaterialType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Material(name: "Ice", type: MaterialType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Material(name: "Earth", type: MaterialType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Material(name: "Iron", type: MaterialType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Material(name: "Wood", type: MaterialType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Material(name: "Chalk", type: MaterialType.Type, rarity: Rarity.Common, dropRate: 1500)},
			{ new Material(name: "Sand", type: MaterialType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Material(name: "Herb", type: MaterialType.Type, rarity: Rarity.Common, dropRate: 1500)},
			{ new Material(name: "Coal", type: MaterialType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Material(name: "Dust", type: MaterialType.Base, rarity: Rarity.Common, dropRate: 1500)},
		};

		private static readonly HashSet<Material> Uncommon = new HashSet<Material>
		{
			{ new Material(name: "Proc Wheel", type: MaterialType.Type, rarity: Rarity.Uncommon, dropRate: 120)},
			{ new Material(name: "Snippler Root", type: MaterialType.Base, rarity: Rarity.Uncommon, dropRate: 130)},
			{ new Material(name: "Tin Flake", type: MaterialType.Base, rarity: Rarity.Uncommon, dropRate: 140)},
			{ new Material(name: "Wisp", type: MaterialType.Type, rarity: Rarity.Uncommon, dropRate: 120)},
			{ new Material(name: "Chell Bracelet", type: MaterialType.Type, rarity: Rarity.Uncommon, dropRate: 130)},
			{ new Material(name: "Tea", type: MaterialType.Type, rarity: Rarity.Uncommon, dropRate: 140)},
		};

		private static readonly HashSet<Material> Rare = new HashSet<Material>
		{
			{ new Material(name: "Turk Mech", type: MaterialType.Special, rarity: Rarity.Rare, dropRate: 15)},
			{ new Material(name: "Dark Quaff", type: MaterialType.Special, rarity: Rarity.Rare, dropRate: 12)},
			{ new Material(name: "Shell Elixir", type: MaterialType.Type, rarity: Rarity.Rare, dropRate: 20)},
			{ new Material(name: "Tandem Hollows", type: MaterialType.Type, rarity: Rarity.Rare, dropRate: 17)},
		};

		private static readonly HashSet<Material> Mythic = new HashSet<Material>
		{
			{new Material(name: "Ulmir Fossil", type: MaterialType.Special, rarity: Rarity.Mythic, dropRate: 5)},
			{new Material(name: "Tusker Brand", type: MaterialType.Special, rarity: Rarity.Mythic, dropRate: 8)},

		};

		private static readonly HashSet<Material> Legendary = new HashSet<Material>
		{
			{new Material(name: "Feather Of Ghosh", type: MaterialType.Special, rarity: Rarity.Legendary, dropRate: 2)}
		};
	}
}
