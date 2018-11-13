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
		Int64 ID { get; set; }
		string Name { get; set; }
		int Price { get; set; }
		int Quantity { get; set; }
		long PlayerID { get; set; }
		ResourceType ResourceType { get; set; }
		OrderType OrderType { get; set; }

		int Total { get; }
	}

	public class TradeItem : SerializablePacket, ITradeItem
	{
		public Int64 ID { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		public int Quantity { get; set; }
		public long PlayerID { get; set; }
		public ResourceType ResourceType { get; set; }
		public OrderType OrderType { get; set; }
		public int Total { get { return Price * Quantity; } }

		public TradeItem(){}

		public TradeItem(Int64 tradeId, string name, int price, int quantity, long playerId, ResourceType resourceType, OrderType orderType)
		{
			ID = tradeId;
			Name = name;
			Price = price;
			Quantity = quantity;
			PlayerID = playerId;
			ResourceType = resourceType;
			OrderType = orderType;
		}

		public TradeItem(ITradeItem item)
		{
			ID = item.ID;
			Name = item.Name;
			Price = item.Price;
			Quantity = item.Quantity;
			PlayerID = item.PlayerID;
			ResourceType = item.ResourceType;
			OrderType = item.OrderType;
		}

		public TradeItem(BsonDocument document)
		{
			ID = document["TradeID"];
			Name = document["Name"];
			Price = document["Price"];
			Quantity = document["Quantity"];
			PlayerID = document["PlayerID"];
			ResourceType = (ResourceType) document["ResourceType"].AsInt32;
			OrderType = (OrderType)document["OrderType"].AsInt32;

		}

		public BsonDocument ToBsonDocument()
		{
			var retVal = new BsonDocument();

			retVal.Add("TradeID", ID);
			retVal.Add("Name", Name);
			retVal.Add("Price", Price);
			retVal.Add("Quantity", Quantity);
			retVal.Add("PlayerID", PlayerID);
			retVal.Add("ResourceType", (int)ResourceType);
			retVal.Add("OrderType", (int)OrderType);

			return retVal;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(ID);
			writer.Write(Name);
			writer.Write(Price);
			writer.Write(Quantity);
			writer.Write(PlayerID);
			writer.Write((int)ResourceType);
			writer.Write((int)OrderType);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			ID = reader.ReadInt64();
			Name = reader.ReadString();
			Price = reader.ReadInt32();
			Quantity = reader.ReadInt32();
			PlayerID = reader.ReadInt64();
			ResourceType = (ResourceType)reader.ReadInt32();
			OrderType = (OrderType)reader.ReadInt32();

		}

		public override string ToString()
		{
			return $"Trade ({OrderType})- Item: TradeID: {ID}. Name: {Name}. Price: {Price}. Quantity: {Quantity}. PlayerID: {PlayerID}";
		}

		public override bool Equals(object obj)
		{
			ITradeItem trade = (ITradeItem)obj;
			return trade.ID == ID &&
				trade.Price == Price &&
				trade.Quantity == Quantity &&
				trade.Name.Equals(Name) &&
				trade.OrderType == OrderType &&
				trade.ResourceType == ResourceType &&
				trade.PlayerID == PlayerID;
		}
	}

	public enum ResourceType
	{
		Action = 0,
		Resource = 1,
		Material = 2,
	}
}
