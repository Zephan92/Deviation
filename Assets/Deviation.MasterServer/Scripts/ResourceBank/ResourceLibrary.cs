using System.Collections.Generic;
using System.Linq;

namespace Assets.Deviation.MasterServer.Scripts.ResourceBank
{
	public class ResourceLibrary
	{
		private static HashSet<Resource> _all = new HashSet<Resource>();

		public static HashSet<Resource> GetResources(Rarity rarity = Rarity.Default)
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
					return GetAllResources();
			}
		}

		public static Resource GetResource(string resourceName)
		{
			var resources = GetAllResources();
			if (ResourceExists(resourceName))
			{
				return resources.First(resource => resource.Name == resourceName);
			}
			else
			{
				return new Resource();
			}
		}

		public static bool ResourceExists(string resourceName)
		{
			///hmmmmm
			return GetAllResources().Any(resource => resource.Name == resourceName);
		}

		private static HashSet<Resource> GetAllResources()
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

		private static HashSet<Resource> Common = new HashSet<Resource>
		{
			{ new Resource(name: "Rock", type: ResourceType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Resource(name: "Ice", type: ResourceType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Resource(name: "Earth", type: ResourceType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Resource(name: "Iron", type: ResourceType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Resource(name: "Wood", type: ResourceType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Resource(name: "Chalk", type: ResourceType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Resource(name: "Sand", type: ResourceType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Resource(name: "Herb", type: ResourceType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Resource(name: "Coal", type: ResourceType.Base, rarity: Rarity.Common, dropRate: 1500)},
			{ new Resource(name: "Dust", type: ResourceType.Base, rarity: Rarity.Common, dropRate: 1500)},
		};

		private static readonly HashSet<Resource> Uncommon = new HashSet<Resource>
		{
			{ new Resource(name: "Proc Wheel", type: ResourceType.Base, rarity: Rarity.Uncommon, dropRate: 120)},
			{ new Resource(name: "Snippler Root", type: ResourceType.Base, rarity: Rarity.Uncommon, dropRate: 130)},
			{ new Resource(name: "Tin Flake", type: ResourceType.Base, rarity: Rarity.Uncommon, dropRate: 140)},
			{ new Resource(name: "Wisp", type: ResourceType.Base, rarity: Rarity.Uncommon, dropRate: 120)},
			{ new Resource(name: "Chell Bracelet", type: ResourceType.Base, rarity: Rarity.Uncommon, dropRate: 130)},
			{ new Resource(name: "Tea", type: ResourceType.Base, rarity: Rarity.Uncommon, dropRate: 140)},
		};

		private static readonly HashSet<Resource> Rare = new HashSet<Resource>
		{
			{ new Resource(name: "Turk Mech", type: ResourceType.Base, rarity: Rarity.Rare, dropRate: 15)},
			{ new Resource(name: "Dark Quaff", type: ResourceType.Base, rarity: Rarity.Rare, dropRate: 12)},
			{ new Resource(name: "Shell Elixir", type: ResourceType.Base, rarity: Rarity.Rare, dropRate: 20)},
			{ new Resource(name: "Tandem Hollows", type: ResourceType.Base, rarity: Rarity.Rare, dropRate: 17)},
		};

		private static readonly HashSet<Resource> Mythic = new HashSet<Resource>
		{
			{new Resource(name: "Ulmir Fossil", type: ResourceType.Base, rarity: Rarity.Mythic, dropRate: 5)},
			{new Resource(name: "Tusker Brand", type: ResourceType.Base, rarity: Rarity.Mythic, dropRate: 8)},

		};

		private static readonly HashSet<Resource> Legendary = new HashSet<Resource>
		{
			{new Resource(name: "Feather Of Ghosh", type: ResourceType.Base, rarity: Rarity.Legendary, dropRate: 2)}
		};
	}
}
