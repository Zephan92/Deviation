using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Deviation.MasterServer.Scripts.ResourceBank
{
	public class ResourceBagPacket : ExtendedSerializablePacket
	{
		public ResourceBag ResourceBag;

		public ResourceBagPacket()
		{

		}

		public ResourceBagPacket(ResourceBag resourceBag)
		{
			ResourceBag = resourceBag;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			//setup
			Dictionary<string, int> resourcesDict = new Dictionary<string, int>();
			Dictionary<string, int> historyDict = new Dictionary<string, int>();

			Dictionary<Resource, int> resources = ResourceBag.Resources;
			Dictionary<Resource, int> history = ResourceBag.History;

			resources.ToList().ForEach(resource => resourcesDict.Add(resource.Key.Name, resource.Value));
			history.ToList().ForEach(resource => historyDict.Add(resource.Key.Name, resource.Value));

			//write
			WriteDictionary(resourcesDict, writer);
			WriteDictionary(historyDict, writer);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			//setup
			Dictionary<Resource, int> resourcesDict = new Dictionary<Resource, int>();
			Dictionary<Resource, int> historyDict = new Dictionary<Resource, int>();

			//read
			Dictionary<string, int> resources = ReadDictionary(reader);
			Dictionary<string, int> history = ReadDictionary(reader);

			//convert
			resources.ToList().ForEach(resource => resourcesDict.Add(ResourceLibrary.GetResource(resource.Key), resource.Value));
			history.ToList().ForEach(resource => historyDict.Add(ResourceLibrary.GetResource(resource.Key), resource.Value));

			//save
			ResourceBag = new ResourceBag(resourcesDict, historyDict);
		}

		public override string ToString()
		{
			return ResourceBag.ToString();
		}
	}
}
