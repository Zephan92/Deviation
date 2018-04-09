using Assets.Deviation.Exchange.Scripts.Client;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Deviation.Client.Scripts.Results
{
	public class ClientResultsController : ControllerBase
	{
		public override void Awake()
		{
			base.Awake();
			ClientDataRepository.Instance.State = ClientState.Results;
		}

		public void Done()
		{
			SceneManager.LoadScene("DeviationClient - Client");
		}
	}
}
