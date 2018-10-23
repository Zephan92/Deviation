using LiteDB;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts.Exchange
{
	public class PlayerDataAccess
	{
		LiteDatabase db = new LiteDatabase(@"Players.db");
		LiteCollection<PlayerAccount> _players;
		string playerCollectionName = "Player";

		public PlayerDataAccess()
		{
			BsonMapper.Global.RegisterType
			(
				serialize: (packet) => packet.ToBsonDocument(),
				deserialize: (bson) => new PlayerAccount(bson.AsDocument)
			);

			_players = db.GetCollection<PlayerAccount>(playerCollectionName);
		}

		public PlayerAccount CreatePlayer(string name, string alias = "")
		{
			if (!PlayerExists(name) && !name.Equals(""))
			{
				if (alias.Equals(""))
				{
					alias = name;
				}

				PlayerAccount playerAccount = new PlayerAccount(_players.Count(), name, alias);
				_players.Insert(playerAccount);
				return playerAccount;
			}
			else
			{
				Debug.LogErrorFormat("Failed to create player because player exists or name is not valid. Name: {0}. Alias: {1}", name, alias);
				return null;
			}
		}

		public bool PlayerExists(long id)
		{
			return GetPlayer(id) != null;
		}

		public bool PlayerExists(string name)
		{
			return GetPlayer(name) != null;
		}

		public PlayerAccount GetPlayer(long id)
		{
			return _players.FindOne(x => x.Id == id);
		}

		public PlayerAccount GetPlayer(string name)
		{
			return _players.FindOne(x => x.Name == name);
		}
	}
}
