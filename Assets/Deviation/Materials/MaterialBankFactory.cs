using Assets.Deviation.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Assets.Deviation.Materials
{
	public class MaterialBankFactory
	{
		private RNGCryptoServiceProvider _provider = new RNGCryptoServiceProvider();

		public MaterialBankObject Create(MaterialBankTypeBase resourceBankType)
		{
			UnityEngine.Debug.Log("Creating new Material Bank. Type: " + resourceBankType);
			Dictionary<Material, int> resources = new Dictionary<Material, int>();
			int rarityTypes = (int)Rarity.Count;

			for (int i = 0; i < rarityTypes; i++)
			{
				Rarity rarity = (Rarity)i;
				int rarityCount = resourceBankType.GetCount(rarity);
				Queue<Material> resourceList = GetRandomMaterialList(rarity, rarityCount);

				while (resourceList.Count > 0)
				{
					Material resource = resourceList.Dequeue();
					resources.Add(resource, resource.DropRate);
				}
			}

			var retval = new MaterialBankObject(resources, resourceBankType);
			UnityEngine.Debug.LogError("Created new Material Bank. " + retval.ToString());
			return retval;
		}

		private Queue<Material> GetRandomMaterialList(Rarity rarity, int count)
		{
			Queue<Material> retVal = new Queue<Material>();
			List<Material> shuffledMaterials = Shuffle(MaterialLibrary.GetMaterials(rarity).ToList());

			for (int i = 0; i < count; i++)
			{
				retVal.Enqueue(shuffledMaterials[i]);
			}

			return retVal;
		}

		private List<Material>[] CreateMaterialListsByRarity(List<Material> resources, int rarityCount)
		{
			List<Material>[] resourcesByRarity = new List<Material>[rarityCount];
			for (int i = 0; i < (int)Rarity.Count; i++)
			{
				resourcesByRarity[i] = new List<Material>();
			}

			foreach (Material resource in resources)
			{
				resourcesByRarity[(int)resource.Rarity].Add(resource);
			}

			return resourcesByRarity;
		}

		private List<Material> Shuffle(List<Material> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				byte[] box = new byte[1];
				do _provider.GetBytes(box);
				while (!(box[0] < n * (Byte.MaxValue / n)));
				int k = (box[0] % n);
				n--;
				Material value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}
	}
}
