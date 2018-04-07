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
	[RequireComponent(typeof(ClientDataRepository))]
	public class ControllerBase : MonoBehaviour
	{
		protected ITimerManager tm;
		public static bool IsEditor;

		public virtual void Awake()
		{
			tm = GetComponent<TimerManager>();
			SetIsEditor();

			if (IsEditor)
			{
				ClientDataRepository.InstanceCreated += OnDataCreated;
			}
		}

		public virtual void OnDataCreated()
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

		public virtual void Start()
		{
			
		}

		public virtual void Update()
		{

		}

		public virtual void FixedUpdate()
		{

		}

		private void SetIsEditor()
		{
#if UNITY_EDITOR
			IsEditor = true;
#else
            IsEditor = false;
#endif
		}
	}
}
