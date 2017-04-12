using Deviation.Data.ResourceEnums;
using System.Collections.Generic;

namespace Deviation.Data.Resource
{
	public class ResourceLibrary
	{
		public static IResource GetResourceInstance(string resourceName)
		{
			IResource resource = ResourceLibraryTable[resourceName];
			IResource resourceInstance = new Resource(resource.Name, resource.Type, resource.Rarity);
			return resourceInstance;
		}

		public static readonly Dictionary<string, IResource> ResourceLibraryTable = new Dictionary<string, IResource>
		{
			{ "Rock",new Resource(name: "Rock", type: ResourceType.Base, rarity: Rarity.Common)},
			{ "Ice",new Resource(name: "Ice", type: ResourceType.Base, rarity: Rarity.Common)},
			{ "Earth",new Resource(name: "Earth", type: ResourceType.Base, rarity: Rarity.Common)},
			{ "Iron",new Resource(name: "Iron", type: ResourceType.Base, rarity: Rarity.Common)},
			{ "Wood",new Resource(name: "Wood", type: ResourceType.Base, rarity: Rarity.Common)},

			{ "Proc Wheel",new Resource(name: "Proc Wheel", type: ResourceType.Base, rarity: Rarity.Uncommon)},
			{ "Snippler Root",new Resource(name: "Snippler Root", type: ResourceType.Base, rarity: Rarity.Uncommon)},
			{ "Tin Flake",new Resource(name: "Tin Flake", type: ResourceType.Base, rarity: Rarity.Uncommon)},

			{ "Turk Mech",new Resource(name: "Turk Mech", type: ResourceType.Base, rarity: Rarity.Rare)},
			{ "Dark Quaff",new Resource(name: "Dark Quaff", type: ResourceType.Base, rarity: Rarity.Rare)},

			{ "Ulmir Fossil",new Resource(name: "Ulmir Fossil", type: ResourceType.Base, rarity: Rarity.Mythic)},

			{ "Feather Of Ghosh",new Resource(name: "Feather Of Ghosh", type: ResourceType.Base, rarity: Rarity.Legendary)}
		};
	}
}
