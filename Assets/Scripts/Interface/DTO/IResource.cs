using Assets.Scripts.Enum;

namespace Assets.Scripts.Interface.DTO
{
	public interface IResource
	{
		string Name { get; set; }
		ResourceType Type { get; set; }
		Rarity Rarity { get; set; }
	}
}
