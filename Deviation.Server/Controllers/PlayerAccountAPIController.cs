using Deviation.Data.PlayerAccount;
using Deviation.Data.ResourceBag;
using Newtonsoft.Json;
using System;
using System.Web.Http;

namespace Deviation.Server.Controllers
{
	public interface IPlayerAccountAPIController
	{
		[HttpPost]
		bool AddPlayerAccount(string name, string alias);

		[HttpGet]
		string GetPlayerAccount(string name, string password);
	}

	public class PlayerAccountAPIController : ApiController, IPlayerAccountAPIController
	{
		private IPlayers _players;

		public PlayerAccountAPIController(IPlayers players)
		{
			_players = players;
		}

		[HttpPost]
		[Route("api/PlayerAccount/AddPlayerAccount/{name}/{alias}")]
		public bool AddPlayerAccount(string name,string alias)
		{
			try
			{
				IPlayerAccount accountToAdd = new PlayerAccount(name, alias, new ResourceBag());
				return _players.AddPlayerAccount(accountToAdd);
			}
			catch (Exception ex)
			{
				//figure out exception handling at some point
				return false;
			}

			return true;
		}

		[HttpGet]
		[Route("api/PlayerAccount/GetPlayerAccount/{name}/{password}")]
		public string GetPlayerAccount(string name, string password)
		{
			IPlayerAccount account = _players.GetPlayerAccount(name, password);
			return JsonConvert.SerializeObject(account);
		}
	}
}