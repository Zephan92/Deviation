using Barebones.Networking;
using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Deviation.Client.Scripts.Client.Market
{
	public interface ITradeItem : ISerializablePacket
	{
		long ID { get; set; }
		string Name { get; set; }
		int Price { get; set; }
		int Quantity { get; set; }
		long PlayerID { get; set; }
		TradeType Type { get; set; }
		int Total { get; }
	}

	public class TradeItem : SerializablePacket, ITradeItem
	{
		public long ID { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		public int Quantity { get; set; }
		public long PlayerID { get; set; }
		public TradeType Type { get; set; }
		public int Total { get { return Price * Quantity; } }

		public TradeItem(){}

		public TradeItem(long tradeId, string name, int price, int quantity, long playerId, TradeType type)
		{
			ID = tradeId;
			Name = name;
			Price = price;
			Quantity = quantity;
			PlayerID = playerId;
			Type = type;
		}

		public TradeItem(ITradeItem item)
		{
			ID = item.ID;
			Name = item.Name;
			Price = item.Price;
			Quantity = item.Quantity;
			PlayerID = item.PlayerID;
			Type = item.Type;
		}

		public TradeItem(BsonDocument document)
		{
			ID = document["TradeID"];
			Name = document["Name"];
			Price = document["Price"];
			Quantity = document["Quantity"];
			PlayerID = document["PlayerID"];
			Type = (TradeType) document["Type"].AsInt32;
		}

		public BsonDocument ToBsonDocument()
		{
			var retVal = new BsonDocument();

			retVal.Add("TradeID", ID);
			retVal.Add("Name", Name);
			retVal.Add("Price", Price);
			retVal.Add("Quantity", Quantity);
			retVal.Add("PlayerID", PlayerID);
			retVal.Add("Type", (int) Type);

			return retVal;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(ID);
			writer.Write(Name);
			writer.Write(Price);
			writer.Write(Quantity);
			writer.Write(PlayerID);
			writer.Write((int)Type);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			ID = reader.ReadInt64();
			Name = reader.ReadString();
			Price = reader.ReadInt32();
			Quantity = reader.ReadInt32();
			PlayerID = reader.ReadInt64();
			Type = (TradeType)reader.ReadInt32();
		}

		public override string ToString()
		{
			return $"Trade - Item: TradeID: {ID}. Name: {Name}. Price: {Price}. Quantity: {Quantity}. PlayerID: {PlayerID}";
		}

		public override bool Equals(object obj)
		{
			ITradeItem trade = (ITradeItem)obj;
			return trade.ID == ID &&
				trade.Price == Price &&
				trade.Quantity == Quantity &&
				trade.Name.Equals(Name) &&
				trade.Type == Type &&
				trade.PlayerID == PlayerID;
		}
	}

	public enum TradeType
	{
		Action = 0,
		Resource = 1,
		Material = 2,
	}
}
