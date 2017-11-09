using Assets.Deviation.Exchange.Scripts.Interface;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Deviation.Exchange.Scripts.Client
{
	public class PlayerAccount : IPlayerAccount
	{
		[BsonId]
		public long Id { get; set; }
		public string Name { get; set; }
		public string Alias { get; set; }

		public PlayerAccount()
		{
		}

		public PlayerAccount(long id,string name, string alias)
		{
			Id = id;
			Name = name;
			Alias = alias;
		}
	}
}
