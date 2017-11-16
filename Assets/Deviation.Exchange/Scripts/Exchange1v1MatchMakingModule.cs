using Assets.Deviation.Exchange.Scripts.Utilities;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Deviation.Exchange
{
	public enum QueueTypes
	{
		Exchange1v1,
		Exchange1v1Ranked
	}

	public enum PlayerClass
	{
		Default = -1,
		E = 0,
		D = 1,
		C = 2,
		B = 3,
		A = 4,
		S = 5
	}

	public class PlayerMMR
	{
		public long PlayerId;
		public int MMR;
		public PlayerClass PlayerClass;
		private DateTime queueTime;
		private int matchRound;
		public bool ExpandQueue;

		public PlayerMMR(long playerId, int mmr, PlayerClass playerClass)
		{
			PlayerId = playerId;
			MMR = mmr;
			PlayerClass = playerClass;
			queueTime = DateTime.UtcNow;
			matchRound = 1;
			ExpandQueue = false;
		}

		public bool Matches(PlayerMMR potentialMatch)
		{
			return Math.Abs(MMR - potentialMatch.MMR) <= (matchRound * 100);
		}

		public bool ExpandPool()
		{
			if (queueTime < DateTime.UtcNow - TimeSpan.FromSeconds(30))
			{
				queueTime = DateTime.UtcNow;
				matchRound += 1;
				if (matchRound % 10 == 0 && !ExpandQueue && PlayerClass != PlayerClass.S)//5 minutes
				{
					ExpandQueue = true;
					return true;
				}
			}

			return false;
		}

		public void ChangeQueue()
		{
			queueTime = DateTime.UtcNow;
			ExpandQueue = false;
			PlayerClass += 1;
		}
	}

	public enum Exchange1v1MatchMakingOpCodes
	{
		RequestJoinQueue = 128,
		RequestLeaveQueue = 132,
		RequestChangeQueuePool = 136,
		RespondMatchFound = 140,
		RespondChangeQueuePool = 144,

		// (OpCodes should be unique. MSF internal opCodes 
		// start from 32000, so you can use anything from 0 to 32000
	}

	public class ExchangeMatchMakingPacket : SerializablePacket
	{
		public long PlayerId;
		public QueueTypes Queue;
		public PlayerClass PlayerClass;

		public ExchangeMatchMakingPacket()
		{ }

		public ExchangeMatchMakingPacket(long playerId, QueueTypes queue, PlayerClass playerClass)
		{
			PlayerId = playerId;
			Queue = queue;
			PlayerClass = playerClass;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(PlayerId);
			writer.Write((short) Queue);
			writer.Write((short) PlayerClass);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			PlayerId = reader.ReadInt64();
			Queue = (QueueTypes) reader.ReadInt16();
			PlayerClass = (PlayerClass) reader.ReadInt16();
		}

		public override string ToString()
		{
			return String.Format("ExchangeMatchMakingPacket - PlayerId: {0}. Queue: {1}. PlayerClass: {2}", PlayerId, Queue, PlayerClass);
		}
	}

	public class MatchFoundPacket : SerializablePacket
	{
		public int ExchangeId;
		public long Player1Id;
		public long Player2Id;
		public QueueTypes Queue;

		public MatchFoundPacket()
		{ }

		public MatchFoundPacket(int exchangeId, long player1Id, long player2Id, QueueTypes queue)
		{
			ExchangeId = exchangeId;
			Player1Id = player1Id;
			Player2Id = player2Id;
			Queue = queue;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(ExchangeId);
			writer.Write(Player1Id);
			writer.Write(Player2Id);
			writer.Write((short)Queue);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			ExchangeId = reader.ReadInt32();
			Player1Id = reader.ReadInt64();
			Player2Id = reader.ReadInt64();
			Queue = (QueueTypes) reader.ReadInt16();
		}

		public override string ToString()
		{
			return String.Format("MatchFoundPacket - ExchangeId: {0}. Player1Id: {1}. Player2Id: {2}. Queue: {3}.", ExchangeId, Player1Id, Player2Id, Queue);
		}
	}

	public class Exchange1v1MatchMakingModule : ServerModuleBehaviour
	{
		private ExchangeMatchMaking matchMaker;

		public void Awake()
		{
			matchMaker = FindObjectOfType<ExchangeMatchMaking>();
		}

		public override void Initialize(IServer server)
		{
			base.Initialize(server);
			Debug.Log("Exchange 1v1 Match Making Module initialized");
			server.SetHandler((short)Exchange1v1MatchMakingOpCodes.RequestJoinQueue, HandleRequestJoin1v1Queue);
			server.SetHandler((short)Exchange1v1MatchMakingOpCodes.RequestLeaveQueue, HandleRequestLeave1v1Queue);
			server.SetHandler((short)Exchange1v1MatchMakingOpCodes.RequestChangeQueuePool, HandleRequestChange1v1QueuePool);
		}

		private void HandleRequestJoin1v1Queue(IIncommingMessage message)
		{
			//data is not correct...
			var packet = message.Deserialize(new ExchangeMatchMakingPacket());
			Debug.LogErrorFormat("HandleRequestJoin1v1Queue: {0}", packet);
			bool success = matchMaker.JoinQueue(packet, message.Peer);

			if (success)
			{
				message.Respond(ResponseStatus.Success);
			}
			else
			{
				message.Respond(ResponseStatus.Invalid);
			}
		}

		private void HandleRequestLeave1v1Queue(IIncommingMessage message)
		{
			var packet = message.Deserialize(new ExchangeMatchMakingPacket());
			Debug.LogErrorFormat("HandleRequestLeave1v1Queue: {0}", packet);
			bool success = matchMaker.LeaveQueue(packet);

			if (success)
			{
				message.Respond(ResponseStatus.Success);
			}
			else
			{
				message.Respond(ResponseStatus.Invalid);
			}
		}

		private void HandleRequestChange1v1QueuePool(IIncommingMessage message)
		{
			var packet = message.Deserialize(new ExchangeMatchMakingPacket());
			Debug.LogErrorFormat("HandleRequestChange1v1QueuePool: {0}", packet);
			matchMaker.ChangeQueuePool(packet);
			message.Respond(ResponseStatus.Success);
		}
	}
}
