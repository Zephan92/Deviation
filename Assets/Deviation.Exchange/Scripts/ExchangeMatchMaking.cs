using Assets.Deviation.Exchange.Scripts.Utilities;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Deviation.Exchange
{
	public class ExchangeMatchMaking : MonoBehaviour
	{
		private int matchesFound;
		private const int dequeueBatchSize = 500;
		private ConcurrentQueue<long> _joinExchange1v1;
		private ConcurrentQueue<long> _changeExchange1v1;
		private ConcurrentQueue<long> _leaveExchange1v1;

		private Dictionary<PlayerClass, Dictionary<long, PlayerMMR>> _poolsExchange1v1;
		private Dictionary<long, IPeer> connections = new Dictionary<long, IPeer>();

		public void Awake()
		{
			_joinExchange1v1 = new ConcurrentQueue<long>();
			_changeExchange1v1 = new ConcurrentQueue<long>();
			_leaveExchange1v1 = new ConcurrentQueue<long>();
			_poolsExchange1v1 = new Dictionary<PlayerClass, Dictionary<long, PlayerMMR>>();
			_poolsExchange1v1.Add(PlayerClass.E, new Dictionary<long, PlayerMMR>());
			_poolsExchange1v1.Add(PlayerClass.D, new Dictionary<long, PlayerMMR>());
			_poolsExchange1v1.Add(PlayerClass.C, new Dictionary<long, PlayerMMR>());
			_poolsExchange1v1.Add(PlayerClass.B, new Dictionary<long, PlayerMMR>());
			_poolsExchange1v1.Add(PlayerClass.A, new Dictionary<long, PlayerMMR>());
			_poolsExchange1v1.Add(PlayerClass.S, new Dictionary<long, PlayerMMR>());

			matchesFound = 0;

			StartCoroutine(Match(QueueTypes.Exchange1v1));
		}

		public bool JoinQueue(ExchangeMatchMakingPacket packet, IPeer peer)
		{
			if (!connections.ContainsKey(packet.PlayerId))
			{
				connections.Add(packet.PlayerId, peer);
			}
			else
			{
				Debug.LogErrorFormat("Player already in queue", packet);
				return false;
			}

			switch (packet.Queue)
			{
				case QueueTypes.Exchange1v1:
					_joinExchange1v1.Enqueue(packet.PlayerId);
					break;
				case QueueTypes.Exchange1v1Ranked:
					break;
				default:
					break;
			}

			return true;
		}

		public bool LeaveQueue(ExchangeMatchMakingPacket packet)
		{
			if (connections.ContainsKey(packet.PlayerId))
			{
				connections.Remove(packet.PlayerId);
			}
			else
			{
				Debug.LogErrorFormat("Player not in queue", packet);
				return false;
			}

			switch (packet.Queue)
			{
				case QueueTypes.Exchange1v1:
					_leaveExchange1v1.Enqueue(packet.PlayerId);
					break;
				case QueueTypes.Exchange1v1Ranked:
					break;
				default:
					break;
			}

			return true;
		}

		public void ChangeQueuePool(ExchangeMatchMakingPacket packet)
		{
			switch (packet.Queue)
			{
				case QueueTypes.Exchange1v1:
					_changeExchange1v1.Enqueue(packet.PlayerId);
					break;
				case QueueTypes.Exchange1v1Ranked:
					break;
				default:
					break;
			}
		}

		private void MatchFound(long player1Id, long player2Id)
		{
			int exchangeId = matchesFound;
			var packet = new MatchFoundPacket(exchangeId, player1Id, player2Id, QueueTypes.Exchange1v1);
			Debug.LogErrorFormat("MatchFound: {0}", packet);
			connections[player1Id].SendMessage((short)Exchange1v1MatchMakingOpCodes.RespondMatchFound, packet);
			connections[player2Id].SendMessage((short)Exchange1v1MatchMakingOpCodes.RespondMatchFound, packet);
			connections.Remove(player1Id);
			connections.Remove(player2Id);

			//todo create exchange
			matchesFound++;
		}

		private void RequestChangeQueue(PlayerMMR player)
		{
			var packet = new ExchangeMatchMakingPacket(player.PlayerId, QueueTypes.Exchange1v1, player.PlayerClass + 1);
			Debug.LogErrorFormat("ChangeQueue: {0}", packet);
			connections[player.PlayerId].SendMessage((short)Exchange1v1MatchMakingOpCodes.RespondChangeQueuePool, packet);
		}

		private PlayerMMR GetPlayerMMR(long playerId, QueueTypes queue)
		{
			return new PlayerMMR(playerId, 0, PlayerClass.E);
		}

		public IEnumerator Match(QueueTypes queue)
		{
			while (true)
			{
				foreach (long playerId in _changeExchange1v1.Dequeue(dequeueBatchSize))
				{
					ChangeQueue(playerId, queue);
				}

				foreach (long playerId in _leaveExchange1v1.Dequeue(dequeueBatchSize))
				{
					LeaveQueue(playerId, queue);
				}

				foreach (long playerId in _joinExchange1v1.Dequeue(dequeueBatchSize))
				{
					FindMatch(playerId, queue);
				}

				ExpandQueuePools(queue);
				FindMatches(queue);
				yield return new WaitForSeconds(0.1f);
			}
		}

		private void ExpandQueuePools(QueueTypes queue)
		{
			var pools = GetQueuePools(queue);
			foreach (var pool in pools.Values)
			{
				foreach (PlayerMMR player in pool.Values)
				{
					bool changeQueue = player.ExpandPool();

					if (changeQueue)
					{
						RequestChangeQueue(player);
					}
				}
			}
		}

		private void FindMatches(QueueTypes queue)
		{
			var pools = GetQueuePools(queue);
			foreach (var pool in pools.Values)
			{
				var orderedPool = pool.OrderBy(x => x.Value.MMR);

				HashSet<long> playersMatched = new HashSet<long>();
				foreach (var player in orderedPool)
				{
					PlayerMMR playerMMR = player.Value;
					if (!playersMatched.Contains(playerMMR.PlayerId))
					{
						PlayerMMR matchMMR = pool.FirstOrDefault(x => x.Key != playerMMR.PlayerId && x.Value.Matches(playerMMR)).Value;

						if (matchMMR != null)
						{
							pool.Remove(playerMMR.PlayerId);
							pool.Remove(matchMMR.PlayerId);
							MatchFound(playerMMR.PlayerId, matchMMR.PlayerId);
							playersMatched.Add(matchMMR.PlayerId);
						}
					}
				}
			}
		}

		private void ChangeQueue(long playerId, QueueTypes queue)
		{
			var pools = GetQueuePools(queue);
			foreach (var pool in pools.Values)
			{
				if (pool.ContainsKey(playerId))
				{
					PlayerMMR mmr = pool[playerId];
					if (!(mmr.PlayerClass + 1 > PlayerClass.S))
					{
						mmr.ChangeQueue();
						pool.Remove(playerId);
						pools[mmr.PlayerClass].Add(playerId, mmr);
						Debug.LogErrorFormat("ChangingQueuePool - PlayerId: {0}. Class: {1}", playerId, mmr.PlayerClass);
						return;
					}
				}
			}
		}

		private void LeaveQueue(long playerId, QueueTypes queue)
		{
			var pools = GetQueuePools(queue);
			foreach (var pool in pools.Values)
			{
				if (pool.ContainsKey(playerId))
				{
					pool.Remove(playerId);
				}
			}
		}

		private void FindMatch(long playerId, QueueTypes queue)
		{
			PlayerMMR mmr = GetPlayerMMR(playerId, queue);

			var pool = GetQueuePools(queue)[mmr.PlayerClass];

			//look for an existing match
			foreach (long match in pool.Keys)
			{
				if (pool[match].Matches(mmr))
				{
					pool.Remove(playerId);
					pool.Remove(match);
					MatchFound(playerId, match);
					return;
				}
			}

			pool.Add(playerId, mmr);
		}

		private Dictionary<PlayerClass, Dictionary<long, PlayerMMR>> GetQueuePools(QueueTypes queue)
		{
			switch (queue)
			{
				case QueueTypes.Exchange1v1:
					return _poolsExchange1v1;
				case QueueTypes.Exchange1v1Ranked:
					return null;//todo
				default:
					return null;
			}
		}
	}
}
