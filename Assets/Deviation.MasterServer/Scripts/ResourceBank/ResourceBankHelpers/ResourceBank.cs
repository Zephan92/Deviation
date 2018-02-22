using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Deviation.MasterServer.Scripts.ResourceBank
{
	public class ResourceBankObject
	{
		private Random Random = new Random();
		private ResourceBankTypeBase _resourceBankType;

		public Dictionary<Resource, int> Resources;
		private Dictionary<Rarity, List<Resource>> _resourcesByRarity;
		private int _resourceCount = 0;

		public ResourceBankObject(Dictionary<Resource, int> resources, ResourceBankTypeBase type)
		{
			Resources = resources;

			_resourcesByRarity = new Dictionary<Rarity, List<Resource>>();
			_resourceBankType = type;

			foreach (Resource resource in resources.Keys)
			{
				if (_resourcesByRarity.ContainsKey(resource.Rarity))
				{
					_resourcesByRarity[resource.Rarity].Add(resource);
				}
				else
				{
					_resourcesByRarity.Add(resource.Rarity, new List<Resource>() { resource });
				}
			}

			foreach (int count in resources.Values)
			{
				_resourceCount += count;
			}
		}

		public int Count()
		{
			return _resourceCount;
		}

		public Resource GetRandomResource(Rarity rarityStart = Rarity.Common, int modifier = 0)
		{
			if (Empty())
			{
				throw new Exception("The Resource Bank was empty.");
			}

			if (rarityStart == Rarity.Default)
			{
				rarityStart = Rarity.Common;
			}

			Resource retval = new Resource();

			int count = 0;
			for (int i = 0; i < (int)rarityStart; i++)
			{
				count += ResourceCount((Rarity)i);
			}

			int min = Math.Min(count + modifier, _resourceCount - 1);

			int ranInt = Random.Next(min, _resourceCount);

			for (int i = (int) rarityStart; i < (int)Rarity.Count; i++)
			{
				Rarity curRarity = (Rarity)i;

				count += ResourceCount(curRarity);

				if (ranInt <= count)
				{
					var resourcesList = _resourcesByRarity[curRarity];
					if (resourcesList.Count > 0)
					{
						retval = ChooseRandomResourceFromList(resourcesList);
					}
					else
					{
						retval = GetRandomResource(rarityStart - 1, modifier);
					}
					break;
				}
			}

			RemoveResource(retval);
			return retval;
		}

		public bool Empty()
		{
			return _resourceCount <= 0;
		}

		public int ResourceCount(Rarity rarity)
		{
			int count = 0;

			if (_resourcesByRarity.ContainsKey(rarity))
			{
				_resourcesByRarity[rarity].ForEach(resource => count += Resources[resource]);
			}
			return count;
		}

		private Resource ChooseRandomResourceFromList(List<Resource> resources)
		{
			int ranInt = Random.Next(0, resources.Count);
			return resources[ranInt];
		}

		private void RemoveResource(Resource resource)
		{
			Resources[resource] -= 1;
			_resourceCount--;

			if (Resources[resource] == 0)
			{
				_resourcesByRarity[resource.Rarity].Remove(resource);
			}
		}

		public override string ToString()
		{
			string retval = "ResourceBank - Count: " + _resourceCount + "\n";

			foreach (Resource resource in Resources.Keys)
			{
				retval += resource.Name + ": " + Resources[resource] + "\n";
			}
			return retval;
		}

		public string ResourceBankType()
		{
			string[] typeName = _resourceBankType.ToString().Split('.');

			return typeName[typeName.Length - 1];
		}
	}
}
