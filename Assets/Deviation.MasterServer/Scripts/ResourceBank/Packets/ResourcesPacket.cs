using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Deviation.MasterServer.Scripts.ResourceBank
{
	public class ResourcesPacket : ExtendedSerializablePacket
	{
		public Dictionary<Resource, int> Resources;

		public ResourcesPacket()
		{
			Resources = new Dictionary<Resource, int>();
		}

		public ResourcesPacket(Resource resource)
		{
			Resources = new Dictionary<Resource, int>();
			Resources.Add(resource, 1);
		}

		public ResourcesPacket(Dictionary<Resource, int> resources)
		{
			Resources = resources;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			var resourcesDict = new Dictionary<string, int>();
			Resources.ToList().ForEach(resource => resourcesDict.Add(resource.Key.Name, resource.Value));
			WriteDictionary(resourcesDict, writer);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			var resources = ReadDictionary(reader);
			resources.ToList().ForEach(resource => Resources.Add(ResourceLibrary.GetResource(resource.Key), resource.Value));
		}

		public override string ToString()
		{
			return Resources.ToString();
		}
	}
}
