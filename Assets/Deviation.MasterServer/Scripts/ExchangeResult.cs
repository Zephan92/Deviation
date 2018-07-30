using Assets.Deviation.Exchange.Scripts;
using Barebones.Networking;
using LiteDB;
using System;

namespace Assets.Deviation.MasterServer.Scripts
{
	public class ExchangeResult : SerializablePacket
	{
		public long ExchangeId { get; set; }
		public DateTime Timestamp { get; set; }
		public PlayerAccount Player { get; set; }
		public PlayerStatsPacket PlayerStats { get; set; }
		public ActionModulePacket ActionGuids { get; set; }
		public Guid CharacterGuid { get; set; }

		public ExchangeResult(){}

		public ExchangeResult(	long exchangeId, 
								DateTime timestamp, 
								PlayerAccount player, 
								PlayerStatsPacket playerStats, 
								ActionModulePacket actionGuids, 
								Guid characterGuid)
		{
			ExchangeId = exchangeId;
			Timestamp = timestamp;
			Player = player;
			PlayerStats = playerStats;
			ActionGuids = actionGuids;
			CharacterGuid = characterGuid;
		}

		public ExchangeResult(BsonDocument document)
		{
			ExchangeId = document["ExchangeId"];
			Timestamp = document["Timestamp"];
			Player = new PlayerAccount(document["Player"].AsDocument);
			PlayerStats = new PlayerStatsPacket(document["PlayerStats"].AsDocument);
			ActionGuids = new ActionModulePacket(document["ActionGuids"].AsDocument);
			CharacterGuid = document["CharacterGuid"];
		}

		public BsonDocument ToBsonDocument()
		{
			var retVal = new BsonDocument();

			retVal.Add("ExchangeId", ExchangeId);
			retVal.Add("Timestamp", Timestamp);
			retVal.Add("Player", Player.ToBsonDocument());
			retVal.Add("PlayerStats", PlayerStats.ToBsonDocument());
			retVal.Add("ActionGuids", ActionGuids.ToBsonDocument());
			retVal.Add("CharacterGuid", CharacterGuid);

			return retVal;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(ExchangeId);
			writer.Write(Timestamp.Ticks);
			writer.Write(Player);
			writer.Write(PlayerStats);
			writer.Write(ActionGuids);
			writer.Write(CharacterGuid.ToByteArray());
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			ExchangeId = reader.ReadInt64();
			Timestamp = new DateTime(reader.ReadInt64());
			Player = reader.ReadPacket(new PlayerAccount());
			PlayerStats = reader.ReadPacket(new PlayerStatsPacket());
			ActionGuids = reader.ReadPacket(new ActionModulePacket());
			CharacterGuid = new Guid(reader.ReadBytes(16));
		}

		public override string ToString()
		{
			return $"--ExchangeResult--" +
					$"\nExchangeId: {ExchangeId}" +
					$"\nTimestamp: {Timestamp}" +
					$"\nPlayerAccount: {Player}" +
					$"\nPlayerStats: {PlayerStats}" +
					$"\nActionGuids: {ActionGuids}" +
					$"\nCharacterGuid: {CharacterGuid}";
		}
	}
}
