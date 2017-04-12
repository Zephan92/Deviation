using Deviation.Data.ResourceEnums;

namespace Deviation.Data.Resource
{
	public interface IResource
	{
		string Name { get; set; }
		ResourceType Type { get; set; }
		Rarity Rarity { get; set; }
	}
}
