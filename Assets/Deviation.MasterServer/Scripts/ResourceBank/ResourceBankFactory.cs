using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Assets.Deviation.MasterServer.Scripts.ResourceBank
{
	public class ResourceBankFactory
	{
		private RNGCryptoServiceProvider _provider = new RNGCryptoServiceProvider();

		public ResourceBankObject Create(ResourceBankTypeBase resourceBankType)
		{
			UnityEngine.Debug.Log("Creating new Resource Bank. Type: " + resourceBankType);
			Dictionary<Resource, int> resources = new Dictionary<Resource, int>();
			int rarityTypes = (int)Rarity.Count;

			for (int i = 0; i < rarityTypes; i++)
			{
				Rarity rarity = (Rarity)i;
				int rarityCount = resourceBankType.GetCount(rarity);
				Queue<Resource> resourceList = GetRandomResourceList(rarity, rarityCount);

				while (resourceList.Count > 0)
				{
					Resource resource = resourceList.Dequeue();
					resources.Add(resource, resource.DropRate);
				}
			}

			var retval = new ResourceBankObject(resources, resourceBankType);
			UnityEngine.Debug.LogError("Created new Resource Bank. " + retval.ToString());
			return retval;
		}

		private Queue<Resource> GetRandomResourceList(Rarity rarity, int count)
		{
			Queue<Resource> retVal = new Queue<Resource>();
			List<Resource> shuffledResources = Shuffle(ResourceLibrary.GetResources(rarity).ToList());

			for (int i = 0; i < count; i++)
			{
				retVal.Enqueue(shuffledResources[i]);
			}

			return retVal;
		}

		private List<Resource>[] CreateResourceListsByRarity(List<Resource> resources, int rarityCount)
		{
			List<Resource>[] resourcesByRarity = new List<Resource>[rarityCount];
			for (int i = 0; i < (int)Rarity.Count; i++)
			{
				resourcesByRarity[i] = new List<Resource>();
			}

			foreach (Resource resource in resources)
			{
				resourcesByRarity[(int)resource.Rarity].Add(resource);
			}

			return resourcesByRarity;
		}

		private List<Resource> Shuffle(List<Resource> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				byte[] box = new byte[1];
				do _provider.GetBytes(box);
				while (!(box[0] < n * (Byte.MaxValue / n)));
				int k = (box[0] % n);
				n--;
				Resource value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}
	}
}
