using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Deviation.Exchange.Scripts.Interface;
using LiteDB;

namespace Assets.Deviation.Exchange.Scripts
{
	public class PlayerDataAccess
	{
		LiteDatabase db = new LiteDatabase(@"testing.db");
		LiteCollection<PlayerAccount> _players;
		string collectionName = "players";

		public void Start()
		{
			_players = db.GetCollection<PlayerAccount>(collectionName);
		}

		public PlayerAccount CreatePlayer(string name, string alias)
		{
			if (!PlayerExists(name))
			{
				PlayerAccount playerAccount = new PlayerAccount(_players.Count(), name, alias);
				_players.Insert(playerAccount);
				return playerAccount;
			}
			else
			{
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
			return _players.FindOne(x => x.Name == name);
		}
	}
}
