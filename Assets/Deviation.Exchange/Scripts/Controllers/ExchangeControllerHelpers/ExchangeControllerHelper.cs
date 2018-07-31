using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Deviation.Exchange.Scripts.Controllers.ExchangeControllerHelpers
{
	public enum ExchangeControllerHelperType
	{
		Client,
		Server,
	}

	public interface IExchangeControllerHelper
	{
		void Init();

		void Setup();
		void Setup_FixedUpdate();

		void PreBattle();
		void PreBattle_FixedUpdate();

		void Begin();
		void Begin_FixedUpdate();

		void Battle();
		void Battle_FixedUpdate();

		void End();
		void End_FixedUpdate();

		void PostBattle();
		void PostBattle_FixedUpdate();

		void Teardown();
		void Teardown_FixedUpdate();
	}

	public abstract class ExchangeControllerHelper : NetworkBehaviour, IExchangeControllerHelper
	{
		public ExchangeControllerHelperType HelperType;
		internal IExchangeController1v1 ec;

		public virtual void Init(ExchangeControllerHelperType helperType)
		{
			HelperType = helperType;
		}

		public abstract void Init();

		private bool ShouldExecute()
		{
			switch (HelperType)
			{
				case ExchangeControllerHelperType.Client:
					return isClient;
				case ExchangeControllerHelperType.Server:
					return isServer;
			}

			return false;
		}

		private void LogState()
		{
			Debug.LogError($"Exchange - {HelperType}: {ec.ExchangeState}");
		}

		public void Start()
		{
			ec = GetComponent<ExchangeController1v1>();
		}

		public virtual void Setup() { if (!ShouldExecute()) { return; } LogState(); }
		public virtual void Setup_FixedUpdate() { if (!ShouldExecute()) { return; } LogState(); }

		public virtual void PreBattle() { if (!ShouldExecute()) { return; } LogState(); }
		public virtual void PreBattle_FixedUpdate() { if (!ShouldExecute()) { return; } LogState(); }

		public virtual void Begin() { if (!ShouldExecute()) { return; } LogState(); }
		public virtual void Begin_FixedUpdate() { if (!ShouldExecute()) { return; } LogState(); }

		public virtual void Battle() { if (!ShouldExecute()) { return; } LogState(); }
		public virtual void Battle_FixedUpdate() { if (!ShouldExecute()) { return; } LogState(); }

		public virtual void End() { if (!ShouldExecute()) { return; } LogState(); }
		public virtual void End_FixedUpdate() { if (!ShouldExecute()) { return; } LogState(); }

		public virtual void PostBattle() { if (!ShouldExecute()) { return; } LogState(); }
		public virtual void PostBattle_FixedUpdate() { if (!ShouldExecute()) { return; } LogState(); }

		public virtual void Teardown() { if (!ShouldExecute()) { return; } LogState(); }
		public virtual void Teardown_FixedUpdate() { if (!ShouldExecute()) { return; } LogState(); }

		//public virtual void Paused(){ if(!ShouldExecute()) { return; } LogState(); }
		//public virtual void Paused_FixedUpdate(){ if(!ShouldExecute()) { return; } LogState(); }
	}
}
