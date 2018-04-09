using Assets.Deviation.Materials;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Deviation.MasterServer.Scripts.MaterialBank
{
	public class MaterialBagPacket : ExtendedSerializablePacket
	{
		public MaterialBag MaterialBag;

		public MaterialBagPacket()
		{

		}

		public MaterialBagPacket(MaterialBag resourceBag)
		{
			MaterialBag = resourceBag;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			//setup
			Dictionary<string, int> resourcesDict = new Dictionary<string, int>();
			Dictionary<string, int> historyDict = new Dictionary<string, int>();

			Dictionary<Material, int> resources = MaterialBag.Materials;
			Dictionary<Material, int> history = MaterialBag.History;

			resources.ToList().ForEach(resource => resourcesDict.Add(resource.Key.Name, resource.Value));
			history.ToList().ForEach(resource => historyDict.Add(resource.Key.Name, resource.Value));

			//write
			WriteDictionary(resourcesDict, writer);
			WriteDictionary(historyDict, writer);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			//setup
			Dictionary<Material, int> resourcesDict = new Dictionary<Material, int>();
			Dictionary<Material, int> historyDict = new Dictionary<Material, int>();

			//read
			Dictionary<string, int> resources = ReadDictionary(reader);
			Dictionary<string, int> history = ReadDictionary(reader);

			//convert
			resources.ToList().ForEach(resource => resourcesDict.Add(MaterialLibrary.GetMaterial(resource.Key), resource.Value));
			history.ToList().ForEach(resource => historyDict.Add(MaterialLibrary.GetMaterial(resource.Key), resource.Value));

			//save
			MaterialBag = new MaterialBag(resourcesDict, historyDict);
		}

		public override string ToString()
		{
			return MaterialBag.ToString();
		}
	}
}
