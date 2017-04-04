using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Enum;

namespace Assets.Scripts.DTO
{
	public class Resource : IResource
	{
		public string Name { get; set; }
		public ResourceType Type { get; set; }
		public Rarity Rarity { get; set; }

		public Resource(string name, ResourceType type, Rarity rarity)
		{
			Name = name;
			Type = type;
			Rarity = rarity;
		}
	}
}
