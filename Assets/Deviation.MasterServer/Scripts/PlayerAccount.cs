using Barebones.Networking;
using LiteDB;

namespace Assets.Deviation.MasterServer.Scripts
{
	public class PlayerAccount : SerializablePacket
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Alias { get; set; }

		public PlayerAccount(){}

		public PlayerAccount(long id, string name, string alias)
		{
			Id = id;
			Name = name;
			Alias = alias;
		}

		public PlayerAccount(BsonDocument document)
		{
			Id = document["Id"];
			Name = document["Name"];
			Alias = document["Alias"];
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(Id);
			writer.Write(Name);
			writer.Write(Alias);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			Id = reader.ReadInt64();
			Name = reader.ReadString();
			Alias = reader.ReadString();
		}

		public override string ToString()
		{
			return  $"--PlayerAccount--" +
					$"\nID: {Id}" +
					$"\nName: {Name}" +
					$"\nAlias: {Alias}";
		}

		public BsonDocument ToBsonDocument()
		{
			var document = new BsonDocument();

			document.Add("Id", Id);
			document.Add("Name", Name);
			document.Add("Alias", Alias);

			return document;
		}
	}
}
