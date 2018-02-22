using Barebones.Networking;

namespace Assets.Deviation.MasterServer.Scripts.ResourceBank
{
	public class ResourcePacket : SerializablePacket
	{
		public Resource Resource;

		public ResourcePacket()
		{

		}

		public ResourcePacket(Resource resource)
		{
			Resource = resource;
		}


		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(Resource.Name);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			string resourceName = reader.ReadString();
			Resource = ResourceLibrary.GetResource(resourceName);
		}

		public override string ToString()
		{
			return Resource.ToString();
		}
	}
}
