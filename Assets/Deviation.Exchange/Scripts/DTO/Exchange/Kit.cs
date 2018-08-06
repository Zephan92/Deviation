using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using System;
using Assets.Scripts.Library;
using Assets.Deviation.Exchange.Scripts.DTO.Exchange;
using Barebones.Networking;
using LiteDB;
using System.Linq;

namespace Assets.Scripts.DTO.Exchange
{
	public interface IKit
	{
		IExchangePlayer Player { get; set; }
		IClip[] Clips { get; }

		void Reset();
		BsonDocument ToBsonDocument();
	}

	public class Kit : SerializablePacket, IKit
	{
		public IExchangePlayer Player { get; set; }
		public IClip[] Clips { get; private set; }
		public IExchangeAction BasicAction { get; private set; }

		public Kit(){}
		public Kit(IClip[] clips, IExchangeAction basicAction)
		{
			Clips = clips;
			BasicAction = basicAction;
		}

		public void Reset()
		{
			foreach (IClip clip in Clips)
			{
				clip.Reset();
			}
			//TODO BasicAction.Reset();
		}

		public Kit(BsonDocument document)
		{
			Clips = document["Clips"].AsArray.Select(x => new Clip(x.AsDocument)).ToArray();
			BasicAction = ActionLibrary.GetActionInstance(document["BasicAction"].AsString);
		}

		public BsonDocument ToBsonDocument()
		{
			var retVal = new BsonDocument();

			retVal.Add("Clips", new BsonArray(Clips.Select(i => i.ToBsonDocument())));
			retVal.Add("BasicAction", BasicAction.Name);

			return retVal;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(BasicAction.Name);
			writer.Write(Clips.Length);
			foreach (Clip clip in Clips)
			{
				writer.Write(clip);
			}
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			BasicAction = ActionLibrary.GetActionInstance(reader.ReadString());
			int count = reader.ReadInt32();
			Clips = new IClip[count];

			for (int i = 0; i < count; i++)
			{
				Clips[i] = reader.ReadPacket(new Clip());
			}
		}
	}
}
