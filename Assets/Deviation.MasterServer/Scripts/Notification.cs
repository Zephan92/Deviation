using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts
{
	public class Notification
	{
		private ConcurrentDictionary<long, IPeer> _players;

		public Notification()
		{
			_players = new ConcurrentDictionary<long, IPeer>();
		}

		public void AddPlayer(long playerID, IPeer peer)
		{
			if (!IsPlayerOnline(playerID))
			{
				Debug.LogError($"Player {playerID} is Online");
				_players.Add(playerID, peer);
			}
			else
			{
				throw new Exception($"Player is already logged. Player: {playerID}.");
			}
		}

		public void RemovePlayer(long playerID)
		{
			if (!IsPlayerOnline(playerID))
			{
				Debug.LogError($"Removing Player {playerID}");
				_players.Remove(playerID);
			}
		}

		public IPeer GetPlayerPeer(long playerID)
		{
			if (_players.ContainsKey(playerID))
			{
				return _players[playerID];
			}

			return null;
		}

		public bool IsPlayerOnline(long playerID)
		{
			return _players.ContainsKey(playerID);
		}
	}
}
