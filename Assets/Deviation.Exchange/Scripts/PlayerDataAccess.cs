using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Deviation.Exchange.Scripts.Interface;
using LiteDB;
using UnityEngine;

namespace Assets.Deviation.Exchange.Scripts
{
	public class PlayerDataAccess
	{
		LiteDatabase db = new LiteDatabase(@"exchangePlayers.db");
		LiteCollection<PlayerAccount> _players;
		LiteCollection<PlayerAccount> _exchanges;

		string collectionName = "players";

		public PlayerDataAccess()
		{
			_players = db.GetCollection<PlayerAccount>(collectionName);
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
				Debug.LogErrorFormat("Creating Player and Inserting into DB. {0}", playerAccount);
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
			if (GetPlayer(id) != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool PlayerExists(string name)
		{
			if(GetPlayer(name) != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public PlayerAccount GetPlayer(long id)
		{
			return _players.FindOne(x => x.Id == id);
		}

		public PlayerAccount GetPlayer(string name)
		{
			return _players.FindOne(x => x.Name.Equals(name));
		}
	}
}
