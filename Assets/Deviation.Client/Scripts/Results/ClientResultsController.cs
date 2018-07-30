using Assets.Deviation.Exchange.Scripts.Client;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Barebones.MasterServer;
using Assets.Deviation.MasterServer.Scripts;

namespace Assets.Deviation.Client.Scripts.Results
{
	public class ClientResultsController : ControllerBase
	{
		private ExchangeResults _results;

		public override void Awake()
		{
			base.Awake();
			var packet = new ExchangePlayerPacket(ClientDataRepository.Instance.Exchange.ExchangeId, ClientDataRepository.Instance.PlayerAccount.Name);
			Msf.Connection.SendMessage((short)ExchangePlayerOpCodes.GetExchangeResultData, packet, (status, response) => {
				_results = response.Deserialize(new ExchangeResults());
			});
			ClientDataRepository.Instance.State = ClientState.Results;
		}

		public void Done()
		{
			SceneManager.LoadScene("DeviationClient - Client");
		}

		public void DisplayResults()
		{
			Debug.LogError(_results);
		}
	}
}
