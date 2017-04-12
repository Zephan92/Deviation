using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deviation.Data.PlayerAccount
{
	public interface IPlayers
	{
		bool AddPlayerAccount(IPlayerAccount account);
		IPlayerAccount GetPlayerAccount(string Name, string Password);
	}
}
