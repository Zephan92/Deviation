using Assets.Scripts.Interface;
using System.Collections.Generic;
using Assets.Scripts.Interface.DTO;
using System.Security.Cryptography;
using System;
using Assets.Scripts.Library;
using UnityEngine;
using Assets.Scripts.Enum;

namespace Assets.Scripts.External
{
	public class LootPool : ILootPool
	{
		private Dictionary<IResource,int> _pool;
		private int _lootPoolLeft;

		private RNGCryptoServiceProvider _provider;

		public LootPool()
		{
			_provider = new RNGCryptoServiceProvider();
			_lootPoolLeft = 0;
			_pool = CreateLootPool();
			
		}

		public IResource GetLoot()
		{
			if (_lootPoolLeft == 0)
			{
				_pool = CreateLootPool();
			}

			IResource resource = GetRandomResource();
			_pool[resource]--;
			if (_pool[resource] == 0)
			{
				_pool.Remove(resource);
			}
			_lootPoolLeft--;

			return resource;
		}

		private IResource GetRandomResource()
		{
			IResource retval = null;
			int[] rarityCount = new int[(int)Rarity.Count];
			rarityCount.Initialize();
			List<IResource>[] resourcesByRarity = CreateResourceListsByRarity(new List<IResource>(_pool.Keys),(int) Rarity.Count);
			foreach (List<IResource> list in resourcesByRarity)
			{
				foreach (IResource resource in list)
				{
					rarityCount[(int)resource.Rarity] += _pool[resource];
				}
			}

			int ranInt = UnityEngine.Random.Range(0, _lootPoolLeft);

			int resourceCount = 0;
			for (int i = 0; i < rarityCount.Length; i++)
			{
				resourceCount += rarityCount[i];
				rarityCount[i] = resourceCount;

				if (ranInt < rarityCount[i])
				{
					IResource randomResource = Shuffle(resourcesByRarity[i]).ToArray()[0];
					
					return randomResource;
				}
			}

			return retval;
		}

		private Dictionary<IResource, int> CreateLootPool()
		{
			Debug.Log("Replenishing loot pool");
			int[] resourceTypeCountByRarity = new int[] { 3, 2, 1, 1, 0 };
			int[] resourceCountByRarity = new int[] { 10, 5, 3, 2, 1 };
			var pool = ChooseResourceTypesInPool(resourceTypeCountByRarity, resourceCountByRarity);
			foreach (int count in pool.Values)
			{
				_lootPoolLeft += count;
			}
			return pool;
		}

		private Dictionary<IResource, int> ChooseResourceTypesInPool(int [] resourceTypeCountByRarity, int [] resourceCountByRarity)
		{
			Dictionary<IResource, int> pool = new Dictionary<IResource, int>();
			List<IResource> [] resourceListsByRarity = CreateResourceListsByRarity(new List<IResource>(ResourceLibary.ResourceLibraryTable.Values), (int)Rarity.Count);
			foreach (List<IResource> resourceList in resourceListsByRarity)
			{
				Queue<IResource> shuffledResourceList = new Queue<IResource>(Shuffle(resourceList));
				int resourceCount = resourceTypeCountByRarity[(int)resourceList[0].Rarity];
				for (int i = 0; i < resourceCount; i++)
				{
					IResource resource = shuffledResourceList.Dequeue();
					pool.Add(resource, resourceCountByRarity[(int)resource.Rarity]);
				}
			}
			return pool;
		}

		private List<IResource>[] CreateResourceListsByRarity(List<IResource> resources, int rarityCount)
		{
			List<IResource>[] resourcesByRarity = new List<IResource>[rarityCount];
			for (int i = 0;  i < (int) Rarity.Count; i ++)
			{
				resourcesByRarity[i] = new List<IResource>();
			} 

			foreach (IResource resource in resources)
			{
				resourcesByRarity[(int)resource.Rarity].Add(resource);
			}

			return resourcesByRarity;
		}

		private List<IResource> Shuffle(List<IResource> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				byte[] box = new byte[1];
				do _provider.GetBytes(box);
				while (!(box[0] < n * (Byte.MaxValue / n)));
				int k = (box[0] % n);
				n--;
				IResource value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}
	}
}
