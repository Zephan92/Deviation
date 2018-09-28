using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Deviation.Client.Scripts.Client.Market
{
	public interface ITradeReceipt : ISerializablePacket
	{
		string Name { get; set; }
		long ID { get; set; }
	}

	public class TradeReceipt : SerializablePacket, ITradeReceipt
	{
		public string Name { get; set; }
		public long ID { get; set; }

		public TradeReceipt() { }

		public TradeReceipt(string name, long id)
		{
			Name = name;
			ID = id;
		}

		public TradeReceipt(ITradeItem trade)
		{
			Name = trade.Name;
			ID = trade.ID;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(Name);
			writer.Write(ID);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			Name = reader.ReadString();
			ID = reader.ReadInt64();
		}
	}
}
