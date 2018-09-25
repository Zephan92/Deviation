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
		long ID { get; set; }
	}

	public class TradeReceipt : SerializablePacket, ITradeReceipt
	{
		public long ID { get; set; }

		public TradeReceipt() { }

		public TradeReceipt(long id)
		{
			ID = id;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(ID);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			ID = reader.ReadInt64();
		}
	}
}
