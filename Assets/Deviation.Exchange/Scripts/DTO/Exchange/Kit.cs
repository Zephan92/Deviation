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
		IBasicAction BasicAction { get; }

		void Reset();
		BsonDocument ToBsonDocument();
	}

	public interface IKitComponent
	{
		IExchangePlayer Player { get; set; }
		float TimeRemaining { get; }
		bool Ready { get; }
		void Reset();
		void StartCooldown();
	}

	public class Kit : SerializablePacket, IKit
	{
		private IExchangePlayer _player;
		public IExchangePlayer Player {
			get { return _player; }
			set
			{
				_player = value;
				foreach (IClip clip in Clips)
				{
					clip.Player = value;
				}
				BasicAction.Player = value;
			}
		}
		public IClip[] Clips { get; private set; }
		public IBasicAction BasicAction { get; private set; }

		public Kit(){}
		public Kit(IClip[] clips, IBasicAction basicAction)
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
			BasicAction.Reset();
		}

		public Kit(BsonDocument document)
		{
			//add try catch
			Clips = document["Clips"].AsArray.Select(x => new Clip(x.AsDocument)).ToArray();
			BasicAction = new BasicAction(document["BasicAction"].AsString);
		}

		public BsonDocument ToBsonDocument()
		{//add try catch
			var retVal = new BsonDocument();

			retVal.Add("Clips", new BsonArray(Clips.Select(i => i.ToBsonDocument())));
			retVal.Add("BasicAction", BasicAction.Action.Name);

			return retVal;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{//add try catch
			writer.Write(BasicAction.Action.Name);
			writer.Write(Clips.Length);
			foreach (Clip clip in Clips)
			{
				writer.Write(clip);
			}
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{//add try catch
			BasicAction = new BasicAction(reader.ReadString());
			int count = reader.ReadInt32();
			Clips = new IClip[count];

			for (int i = 0; i < count; i++)
			{
				Clips[i] = reader.ReadPacket(new Clip());
			}
		}
	}
}
