using Barebones.Networking;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Deviation.MasterServer.Scripts.ResourceBank
{
	public class ResourceBankPacket : ExtendedSerializablePacket
	{
		public ResourceBankObject ResourceBank;

		public ResourceBankPacket()
		{

		}

		public ResourceBankPacket(ResourceBankObject resourceBank)
		{
			ResourceBank = resourceBank;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			//setup
			var resourcesDict = new Dictionary<string, int>();
			var resources = ResourceBank.Resources;
			resources.ToList().ForEach(resource => resourcesDict.Add(resource.Key.Name, resource.Value));

			//write
			writer.Write(ResourceBank.ResourceBankType());
			WriteDictionary(resourcesDict, writer);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			//setup
			var resourcesDict = new Dictionary<Resource, int>();

			//read
			var resourceBankType = reader.ReadString();
			var resources = ReadDictionary(reader);

			//convert
			resources.ToList().ForEach(resource => resourcesDict.Add(ResourceLibrary.GetResource(resource.Key), resource.Value));
			var type = GetBankType(resourceBankType);

			//save
			ResourceBank = new ResourceBankObject(resourcesDict, type);
		}

		public override string ToString()
		{
			return ResourceBank.ToString();
		}

		private ResourceBankTypeBase GetBankType(string type)
		{
			ResourceBankTypeBase retVal;
			switch (type)
			{
				case "ResourceBankTypeGeneral":
					retVal = new ResourceBankTypeGeneral();
					break;

				default:
					retVal = new ResourceBankTypeGeneral();
					break;
			}
			return retVal;
		}
	}
}
