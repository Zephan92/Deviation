using Deviation.Data.Resource;
using System.Collections.Generic;

namespace Deviation.Data.LootPool
{
	public interface ILootPool
	{
		IResource GetLoot();
		Dictionary<IResource, int> GetPool();
	}
}
