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
		string Name { get; set; }
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
		public string Name { get; set; }
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
		public Kit(string name, IClip[] clips, IBasicAction basicAction)
		{
			Name = name;
			Clips = clips;
			BasicAction = basicAction;
		}

		public void Reset()
		{
			try
			{
				foreach (IClip clip in Clips)
				{
					clip.Reset();
				}
				BasicAction.Reset();
			}
			catch (Exception ex)
			{
				throw new KitException("Failed to Reset Kit", ex);
			}
		}

		public Kit(BsonDocument document)
		{
			try
			{
				Name = document["Name"].AsString;
				Clips = document["Clips"].AsArray.Select(x => new Clip(x.AsDocument)).ToArray();
				BasicAction = new BasicAction(document["BasicAction"].AsString);
			}
			catch (Exception ex)
			{
				throw new KitException("Failed to deserialize Kit",ex);
			}
		}

		public BsonDocument ToBsonDocument()
		{
			try
			{
				var retVal = new BsonDocument();

				retVal.Add("Name", Name);
				retVal.Add("Clips", new BsonArray(Clips.Select(i => i.ToBsonDocument())));
				retVal.Add("BasicAction", BasicAction.Action.Name);

				return retVal;
			}
			catch (Exception ex)
			{
				throw new KitException("Failed to serialize Kit", ex);
			}
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			try
			{
				writer.Write(Name);
				writer.Write(BasicAction.Action.Name);
				writer.Write(Clips.Length);
				foreach (Clip clip in Clips)
				{
					writer.Write(clip);
				}
			}
			catch (Exception ex)
			{
				throw new KitException("Failed to serialize Kit", ex);
			}
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			try
			{
				Name = reader.ReadString();
				BasicAction = new BasicAction(reader.ReadString());
				int count = reader.ReadInt32();
				Clips = new IClip[count];

				for (int i = 0; i < count; i++)
				{
					Clips[i] = reader.ReadPacket(new Clip());
				}
			}
			catch (Exception ex)
			{
				throw new KitException("Failed to deserialize Kit", ex);
			}
		}

		public override string ToString()
		{
			string retVal =  $"Kit" +
					$"\nName: {Name}" +
					$"\nBasicAction: {BasicAction}";

			foreach (IClip clip in Clips)
			{
				retVal += $"\n{clip}";
			}

			return retVal;
		}
	}

	public class KitException : Exception
	{
		public KitException() { }
		public KitException(string message) : base(message) { }
		public KitException(string message, Exception inner) : base(message, inner) { }
	}
}
