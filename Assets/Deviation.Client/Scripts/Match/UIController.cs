using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Deviation.Client.Scripts.Match
{
	public class UIController : MonoBehaviour
	{
		protected ClientMatchController cmc;
		protected ITimerManager tm;

		public virtual void Awake()
		{
			cmc = FindObjectOfType<ClientMatchController>();
			tm = FindObjectOfType<TimerManager>();
		}

		public virtual void Start()
		{

		}

		public virtual void Update()
		{

		}
	}
}