using System.Collections.Generic;
using System.Linq;

namespace Assets.Deviation.Materials
{
	public class MaterialBag
	{
		public Dictionary<Material, int> Materials;
		public Dictionary<Material, int> History;

		public MaterialBag()
		{
			Materials = new Dictionary<Material, int>();
			History = new Dictionary<Material, int>();
		}

		public MaterialBag(Dictionary<Material, int> materials)
		{
			Materials = materials;
			History = materials;
		}

		public MaterialBag(Dictionary<Material, int> materials, Dictionary<Material, int> history)
		{
			Materials = materials;
			History = history;
		}

		public void AddMaterial(Material material, int count = 1)
		{
			if (Materials.ContainsKey(material))
			{
				Materials[material] += count;
			}
			else
			{
				Materials.Add(material, count);
			}

			if (History.ContainsKey(material))
			{
				History[material] += count;
			}
			else
			{
				History.Add(material, count);
			}
		}
		public void AddMaterial(Dictionary<Material, int> materials)
		{
			foreach (Material material in materials.Keys)
			{
				AddMaterial(material, materials[material]);
			}
		}

		public void RemoveMaterial(Material material, int count = 1)
		{
			if (Materials.ContainsKey(material))
			{
				Materials[material] -= count;
			}
		}

		public void RemoveMaterial(Dictionary<Material, int> materials)
		{
			foreach (Material material in materials.Keys)
			{
				RemoveMaterial(material, materials[material]);
			}
		}

		public int MaterialCount()
		{
			return Materials.Values.Sum();
		}

		public int HistoryCount()
		{
			return History.Values.Sum();
		}

		public int MaterialCount(Material material)
		{
			if (Materials.ContainsKey(material))
			{
				return Materials[material];
			}
			else
			{
				return 0;
			}
		}

		public int HistoryCount(Material material)
		{
			if (History.ContainsKey(material))
			{
				return History[material];
			}
			else
			{
				return 0;
			}
		}

		public Dictionary<Material, int> MaterialsByType(MaterialType type)
		{
			var retval = new Dictionary<Material, int>();

			foreach (var material in Materials)
			{
				if (material.Key.Type == type)
				{
					retval.Add(material.Key, material.Value);
				}
			}

			return retval;
		}

		public string HistoryToString()
		{
			string retval = "MaterialBag\n";

			foreach (Material material in History.Keys)
			{
				retval += material.Name + ": " + History[material] + "\n";
			}

			return retval;
		}

		public override string ToString()
		{
			string retval = "MaterialBag\n";

			foreach (Material material in Materials.Keys)
			{
				retval += material.Name + ": " + Materials[material] + "\n";
			}

			return retval;
		}
	}
}
