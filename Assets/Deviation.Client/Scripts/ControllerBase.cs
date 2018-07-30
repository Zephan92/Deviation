using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Interface;
using UnityEngine;
using Assets.Scripts.Utilities;
using Assets.Deviation.Exchange.Scripts.Client;
using Barebones.MasterServer;

namespace Assets.Deviation.Client.Scripts
{
	[RequireComponent(typeof(TimerManager))]
	public class ControllerBase : MonoBehaviour
	{
		protected ITimerManager tm;

		public virtual void Awake()
		{
			CreateClientDataRepository();
			tm = GetComponent<TimerManager>();

			if (Application.isEditor)
			{
				ClientDataRepository.OnInstanceCreated(OnDataCreated);
			}
		}

		internal virtual void OnDataCreated()
		{
			ClientDataRepository.Instance.OnLogin += (AccountInfoPacket account, string error) => {
				ClientDataRepository.Instance.GetPlayerAccount();
			};

			if (Msf.Client.Connection.IsConnected)
			{
				ClientDataRepository.Instance.LoginAsGuest();
			}
			else
			{
				Msf.Client.Connection.Connected += () => {
					ClientDataRepository.Instance.LoginAsGuest();
				};
			}
		}

		internal void CreateClientDataRepository()
		{
			if (FindObjectOfType<ClientDataRepository>() == null)
			{
				var cdc = new GameObject("ClientDataRepository");
				cdc.AddComponent<ClientDataRepository>();
				cdc.AddComponent<StaticObject>();
			}
		}

		public virtual void Start()
		{
			
		}

		public virtual void Update()
		{

		}

		public virtual void FixedUpdate()
		{

		}
	}
}
