using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Deviation.Client.Scripts.Client.Market
{
	public class ActionTradeItem : SerializablePacket, ITradeItem
	{
		public string Name { get; set; }
		public int Price { get; set; }
		public int Quantity { get; set; }
		public long PlayerID { get; set; }
		public TradeType Type { get; set; }

		private IExchangeAction _action;

		public ActionTradeItem()
		{
			Type = TradeType.Action;
		}

		public ActionTradeItem(string name, int price, int quantity, long playerId)
		{
			_action = ActionLibrary.GetActionInstance(name);

			Name = name;
			Price = price;
			Quantity = quantity;
			PlayerID = playerId;
			Type = TradeType.Action;
		}

		public ActionTradeItem(IExchangeAction action, int price, int quantity, long playerId)
		{
			_action = action;

			Name = action.Name;
			Price = price;
			Quantity = quantity;
			PlayerID = playerId;
			Type = TradeType.Action;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(Name);
			writer.Write(Price);
			writer.Write(Quantity);
			writer.Write(PlayerID);
			writer.Write((int) Type);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			Name = reader.ReadString();
			Price = reader.ReadInt32();
			Quantity = reader.ReadInt32();
			PlayerID = reader.ReadInt64();
			Type = (TradeType) reader.ReadInt32();

			_action = ActionLibrary.GetActionInstance(Name);
		}
	}
}
