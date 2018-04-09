using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Deviation.Materials;

namespace Assets.Deviation.MasterServer.Scripts.MaterialBank
{
	public class MaterialsPacket : ExtendedSerializablePacket
	{
		public Dictionary<Material, int> Materials;

		public MaterialsPacket()
		{
			Materials = new Dictionary<Material, int>();
		}

		public MaterialsPacket(Material material)
		{
			Materials = new Dictionary<Material, int>();
			Materials.Add(material, 1);
		}

		public MaterialsPacket(Dictionary<Material, int> materials)
		{
			Materials = materials;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			var materialsDict = new Dictionary<string, int>();
			Materials.ToList().ForEach(material => materialsDict.Add(material.Key.Name, material.Value));
			WriteDictionary(materialsDict, writer);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			var materials = ReadDictionary(reader);
			materials.ToList().ForEach(material => Materials.Add(MaterialLibrary.GetMaterial(material.Key), material.Value));
		}

		public override string ToString()
		{
			return Materials.ToString();
		}
	}
}
