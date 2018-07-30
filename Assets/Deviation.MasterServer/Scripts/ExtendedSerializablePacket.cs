using Barebones.Networking;
using System.Collections.Generic;

namespace Assets.Deviation.MasterServer.Scripts
{
	public abstract class ExtendedSerializablePacket : SerializablePacket
	{
		public const string Dilimiter = "";

		public void WriteDictionary(Dictionary<string, int> dictionary, EndianBinaryWriter writer)
		{
			foreach (string key in dictionary.Keys)
			{
				int value = dictionary[key];
				writer.Write(key);
				writer.Write(value);
			}
			writer.Write(Dilimiter);
		}

		public Dictionary<string, int> ReadDictionary(EndianBinaryReader reader)
		{
			Dictionary<string, int> retval = new Dictionary<string, int>();

			while (true)
			{
				string key = reader.ReadString();

				if (!key.Equals(Dilimiter))
				{
					int value = reader.ReadInt32();
					retval.Add(key, value);
					continue;
				}

				break;
			}

			return retval;
		}
	}
}
