using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deviation.Data.PlayerAccount
{
	public class Players : IPlayers
	{
		public Dictionary<string, IPlayerAccount> PlayerAccounts;

		public Players()
		{
			PlayerAccounts = new Dictionary<string, IPlayerAccount>();
		}

		public bool AddPlayerAccount(IPlayerAccount account)
		{
			if (PlayerAccounts.ContainsKey(account.AccountName))
			{
				return false;
			}
			
			PlayerAccounts.Add(account.AccountName, account);
			return true;
		}

		public IPlayerAccount GetPlayerAccount(string Name, string Password)
		{
			if (PlayerAccounts.ContainsKey(Name))
			{
				return PlayerAccounts[Name];
			}
			else
			{
				return null;
			}
		}
	}
}
