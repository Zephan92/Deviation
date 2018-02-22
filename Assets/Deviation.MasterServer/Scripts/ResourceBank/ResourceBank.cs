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

		private Dictionary<Resource, int> _resources;
		private Dictionary<Rarity, List<Resource>> _resourcesByRarity;
		private int _resourceCount = 0;

		public ResourceBankObject(Dictionary<Resource, int> resources, ResourceBankTypeBase type)
		{
			_resourcesByRarity = new Dictionary<Rarity, List<Resource>>();
			_resources = resources;
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

		public Dictionary<Resource, int> GetResources()
		{
			return _resources;
		}

		public Resource GetResource()
		{
			if (Empty())
			{
				throw new Exception("The Resource Bank was empty.");
			}

			return GetRandomResource();
		}

		private Resource GetRandomResource()
		{
			Resource retval = new Resource();

			int ranInt = Random.Next(0, _resourceCount);
			int count = 0;

			for (int i = 0; i < (int)Rarity.Count; i++)
			{
				Rarity curRarity = (Rarity)i;

				count += ResourceCount(curRarity);

				if (ranInt < count)
				{
					retval = ChooseRandomResourceFromList(_resourcesByRarity[curRarity]);
					RemoveResource(retval);
					break;
				}
			}

			return retval;
		}

		public bool Empty()
		{
			return _resourceCount == 0;
		}

		public int ResourceCount(Rarity rarity)
		{
			int count = 0;

			if (_resourcesByRarity.ContainsKey(rarity))
			{
				_resourcesByRarity[rarity].ForEach(resource => count += _resources[resource]);
			}
			return count;
		}

		public Resource ChooseRandomResourceFromList(List<Resource> resources)
		{
			int ranInt = Random.Next(0, resources.Count);
			return resources[ranInt];
		}

		private void RemoveResource(Resource resource)
		{
			_resources[resource] -= 1;
			_resourceCount--;

			if (_resources[resource] == 0)
			{
				_resourcesByRarity[resource.Rarity].Remove(resource);
			}
		}

		public override string ToString()
		{
			string retval = "ResourceBank - Count: " + _resourceCount + "\n";

			foreach (Resource resource in _resources.Keys)
			{
				retval += resource.Name + ": " + _resources[resource] + "\n";
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
