using Barebones.Networking;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Deviation.MasterServer.Scripts
{
	public class ExchangeDataEntry : SerializablePacket
	{
		public long ExchangeId { get; set; }
		public PlayerAccount Player { get; set; }
		public ActionModulePacket ActionGuids { get; set; }
		public Guid CharacterGuid { get; set; }

		public ExchangeDataEntry(){}

		public ExchangeDataEntry(long id, PlayerAccount player, ActionModulePacket actionGuids, Guid characterGuid)
		{
			ExchangeId = id;
			Player = player;
			ActionGuids = actionGuids;
			CharacterGuid = characterGuid;
		}

		public ExchangeDataEntry(BsonDocument document)
		{
			ExchangeId = document["ExchangeId"];
			Player = new PlayerAccount(document["Player"].AsDocument);
			CharacterGuid = document["CharacterGuid"];
			ActionGuids = new ActionModulePacket(document["ActionGuids"].AsDocument);
		}

		public BsonDocument ToBsonDocument()
		{
			var retVal = new BsonDocument();

			retVal.Add("ExchangeId", ExchangeId);
			retVal.Add("Player", Player.ToBsonDocument());
			retVal.Add("CharacterGuid", CharacterGuid);
			retVal.Add("ActionGuids", ActionGuids.ToBsonDocument());

			return retVal;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(ExchangeId);
			writer.Write(Player);
			writer.Write(CharacterGuid.ToByteArray());
			writer.Write(ActionGuids);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			ExchangeId = reader.ReadInt64();
			Player = reader.ReadPacket(new PlayerAccount());
			CharacterGuid = new Guid(reader.ReadBytes(16));
			ActionGuids = reader.ReadPacket(new ActionModulePacket());
		}

		public override string ToString()
		{
			return $"--ExchangeDataEntry--" +
					$"\nExchangeId: {ExchangeId}" +
					$"\nPlayer: {Player}" +
					$"\nCharacterGuid: {CharacterGuid}" +
					$"\nActionModule: {ActionGuids}";
		}
	}
}
