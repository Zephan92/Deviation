using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Deviation.Exchange.Scripts.Client
{
	public class ClientMatchController : MonoBehaviour
	{
		public Button ReadyButton;
		private ClientDataController cdc;

		public void Start()
		{
			cdc = FindObjectOfType<ClientDataController>();
			cdc.State = ClientState.Match;

			if (cdc.PlayerAccount != null)
			{
				ReadyButton.interactable = true;
			}
			else
			{
				cdc.PlayerAccountRecieved += () =>
				{
					ReadyButton.interactable = true;
				};
			}
		}

		public void FixedUpdate()
		{
		}

		public void Ready()
		{
			if (cdc.State == ClientState.Match && cdc.Exchange != null)
			{
				Guid characterGuid = GetPlayerCharacter();
				ActionModulePacket module = GetPlayerActionModule();
				InitExchangePlayerPacket packet = new InitExchangePlayerPacket(cdc.Exchange.ExchangeId, cdc.PlayerAccount, characterGuid, module);

				Msf.Client.Connection.SendMessage((short)ExchangePlayerOpCodes.CreateExchangeData, packet, (response, error) => {
					if (response == ResponseStatus.Error)
					{
						UnityEngine.Debug.LogErrorFormat("CreateExchangeData Error: {0}", error);
					}
					else if(response == ResponseStatus.Success)
					{
						ReadyButton.interactable = false;
						StartCoroutine(StartExchange());
					}
				});
			}
		}

		private IEnumerator StartExchange()
		{
			yield return new WaitUntil(() => cdc.RoomId != -1);
			SceneManager.LoadScene("DeviationStandalone");
		}

		private Guid GetPlayerCharacter()
		{
			return Guid.NewGuid();
		}

		private ActionModulePacket GetPlayerActionModule()
		{
			var q = new Guid("688b267a-fde1-4250-91a0-300aa3343147");
			var w = new Guid("dacb468b-658f-4daa-9400-cd3f005d06bd");
			var e = new Guid("d504df35-dc93-4f84-829e-01e202878341");
			var r = new Guid("36a1cf13-8b79-4800-8574-7cec0c405594");
			//this would go get the actions the player chose
			return new ActionModulePacket(q, w, e, r);
		}
	}
}
