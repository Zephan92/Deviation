using Assets.Deviation.Exchange.Scripts;
using Barebones.Networking;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Assets.Deviation.MasterServer.Scripts.Exchange
{
	public class ExchangeResults : SerializablePacket
	{
		public long ExchangeId { get; set; }
		public DateTime Timestamp { get; set; }
		public List<ExchangeResult> Results { get; set; }

		public ExchangeResults(){}

		public ExchangeResults(long exchangeId, DateTime timestamp, List<ExchangeResult> results)
		{
			ExchangeId = exchangeId;
			Timestamp = timestamp;
			Results = results;
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			ExchangeId = reader.ReadInt64();
			Timestamp = new DateTime(reader.ReadInt64());
			int resultCount = reader.ReadInt32();
			Results = new List<ExchangeResult>();
			for (int i = 0; i < resultCount; i++)
			{
				Results.Add(reader.ReadPacket(new ExchangeResult()));
			}
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(ExchangeId);
			writer.Write(Timestamp.Ticks);
			writer.Write(Results.Count);
			foreach (var result in Results)
			{
				writer.Write(result);
			}
		}

		public override string ToString()
		{
			string results = $"--ExchangeResults--" +
					$"\nExchangeId: {ExchangeId}" +
					$"\nTimestamp: {Timestamp}";

			foreach (var result in Results)
			{
				results += $"\nResult: {result}";
			}

			return results;
		}
	}
}
