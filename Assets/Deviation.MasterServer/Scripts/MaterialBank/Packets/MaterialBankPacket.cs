using Assets.Deviation.Materials;
using Barebones.Networking;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Deviation.MasterServer.Scripts.MaterialBank
{
	public class MaterialBankPacket : ExtendedSerializablePacket
	{
		public MaterialBankObject MaterialBank;

		public MaterialBankPacket()
		{

		}

		public MaterialBankPacket(MaterialBankObject resourceBank)
		{
			MaterialBank = resourceBank;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			//setup
			var resourcesDict = new Dictionary<string, int>();
			var resources = MaterialBank.Materials;
			resources.ToList().ForEach(resource => resourcesDict.Add(resource.Key.Name, resource.Value));

			//write
			writer.Write(MaterialBank.MaterialBankType());
			WriteDictionary(resourcesDict, writer);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			//setup
			var resourcesDict = new Dictionary<Material, int>();

			//read
			var resourceBankType = reader.ReadString();
			var resources = ReadDictionary(reader);

			//convert
			resources.ToList().ForEach(resource => resourcesDict.Add(MaterialLibrary.GetMaterial(resource.Key), resource.Value));
			var type = GetBankType(resourceBankType);

			//save
			MaterialBank = new MaterialBankObject(resourcesDict, type);
		}

		public override string ToString()
		{
			return MaterialBank.ToString();
		}

		private MaterialBankTypeBase GetBankType(string type)
		{
			MaterialBankTypeBase retVal;
			switch (type)
			{
				case "MaterialBankTypeGeneral":
					retVal = new MaterialBankTypeGeneral();
					break;

				default:
					retVal = new MaterialBankTypeGeneral();
					break;
			}
			return retVal;
		}
	}
}
