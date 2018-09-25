using Assets.Deviation.Exchange.Scripts.Utilities;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts
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

	public class ExchangeMatchMakingPacket : SerializablePacket
	{
		public long PlayerId;
		public QueueTypes Queue;
		public PlayerClass PlayerClass;

		public ExchangeMatchMakingPacket(){}

		public ExchangeMatchMakingPacket(long playerId, QueueTypes queue, PlayerClass playerClass)
		{
			PlayerId = playerId;
			Queue = queue;
			PlayerClass = playerClass;
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(PlayerId);
			writer.Write((short)Queue);
			writer.Write((short)PlayerClass);
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			PlayerId = reader.ReadInt64();
			Queue = (QueueTypes)reader.ReadInt16();
			PlayerClass = (PlayerClass)reader.ReadInt16();
		}

		public override string ToString()
		{
			return String.Format("ExchangeMatchMakingPacket - PlayerId: {0}. Queue: {1}. PlayerClass: {2}", PlayerId, Queue, PlayerClass);
		}
	}

	public class MatchFoundPlayer
	{
		public long Id;
		public IPeer Peer;
		public bool Accepted;

		public MatchFoundPlayer(long id, IPeer peer)
		{
			Id = id;
			Peer = peer;
			Accepted = false;
		}
	}

	public class MatchFound
	{
		public long ExchangeId;
		public QueueTypes Queue;
		public PlayerClass PlayerClass;
		public MatchFoundPacket Packet;
		public List<MatchFoundPlayer> Players;
		public DateTime MatchFoundStart;
		public SpawnTask SpawnTask;

		public MatchFound(long exchangeId, IPeer player1, long player1Id, IPeer player2, long player2Id, QueueTypes queue, PlayerClass playerClass)
		{
			ExchangeId = exchangeId;
			Queue = queue;
			PlayerClass = playerClass;
			Packet = new MatchFoundPacket(exchangeId, player1Id, player2Id, queue);
			Players = new List<MatchFoundPlayer> { new MatchFoundPlayer(player1Id, player1), new MatchFoundPlayer(player2Id, player2) };
			MatchFoundStart = DateTime.UtcNow;
		}

		public bool MatchFoundTimerUp()
		{
			return DateTime.UtcNow - TimeSpan.FromSeconds(10) >= MatchFoundStart;
		}

		public void InformPlayers()
		{
			foreach (MatchFoundPlayer player in Players)
			{
				player.Peer.SendMessage((short)Exchange1v1MatchMakingOpCodes.RespondMatchFound, Packet);
			}
		}

		public void GiveRoomIdToPlayers(int roomId)
		{
			foreach (MatchFoundPlayer player in Players)
			{
				player.Peer.SendMessage((short)Exchange1v1MatchMakingOpCodes.RespondRoomId, roomId);
			}
		}

		public List<MatchFoundPlayer> Rematch()
		{
			List<MatchFoundPlayer> retval = new List<MatchFoundPlayer>();

			foreach (MatchFoundPlayer player in Players)
			{
				if (player.Accepted)
				{
					retval.Add(player);
				}
			}

			return retval;
		}

		public void AcceptMatch(long playerId)
		{
			foreach (MatchFoundPlayer player in Players)
			{
				if (player.Id == playerId)
				{
					player.Accepted = true;
					return;
				}
			}
		}

		public bool MatchReady()
		{
			foreach (MatchFoundPlayer player in Players)
			{
				if (!player.Accepted)
				{
					return false;
				}
			}

			return true;
		}

		public override string ToString()
		{
			return Packet.ToString();
		}
	}


	public class MatchFoundPacket : SerializablePacket
	{
		public long ExchangeId;
		public long Player1Id;
		public long Player2Id;
		public QueueTypes Queue;

		public MatchFoundPacket(){}

		public MatchFoundPacket(long exchangeId, long player1Id, long player2Id, QueueTypes queue)
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
			ExchangeId = reader.ReadInt64();
			Player1Id = reader.ReadInt64();
			Player2Id = reader.ReadInt64();
			Queue = (QueueTypes) reader.ReadInt16();
		}

		public override string ToString()
		{
			return	$"--MatchFoundPacket--" +
					$"\nExchangeId: {ExchangeId}. " +
					$"\nPlayer1Id: {Player1Id}. " +
					$"\nPlayer2Id: {Player2Id}. " +
					$"\nQueue: {Queue}.";
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
			server.SetHandler((short)Exchange1v1MatchMakingOpCodes.RequestJoinMatch, HandleRequestJoinMatch);
			server.SetHandler((short)Exchange1v1MatchMakingOpCodes.RequestDeclineMatch, HandleRequestDeclineMatch);			
		}

		private void HandleRequestJoin1v1Queue(IIncommingMessage message)
		{
			//data is not correct...
			var packet = message.Deserialize(new ExchangeMatchMakingPacket());
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
			matchMaker.ChangeQueuePool(packet);
			message.Respond(ResponseStatus.Success);
		}

		private void HandleRequestJoinMatch(IIncommingMessage message)
		{
			var packet = message.Deserialize(new MatchFoundPacket());
			matchMaker.JoinMatch(packet.ExchangeId, packet.Player1Id);
			message.Respond(ResponseStatus.Success);
		}

		private void HandleRequestDeclineMatch(IIncommingMessage message)
		{
			var packet = message.Deserialize(new MatchFoundPacket());
			matchMaker.DeclineMatch(packet.ExchangeId, packet.Player1Id);
			message.Respond(ResponseStatus.Success);
		}
	}
}
