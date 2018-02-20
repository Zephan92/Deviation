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
	public class ClientResultsController : MonoBehaviour
	{
		public void Awake()
		{
			ClientDataController.Instance.State = ClientState.Results;
			//Destroy(FindObjectOfType<ExchangeNetworkManager>().gameObject);
		}

		public void Done()
		{
			SceneManager.LoadScene("DeviationClient - Client");
		}
	}
}
