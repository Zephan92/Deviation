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

		private void LogState()
		{
			Debug.LogError($"Exchange - {HelperType}: {ec.ExchangeState}");
		}

		public void Start()
		{
			ec = GetComponent<ExchangeController1v1>();
		}

		public virtual void Setup() { LogState(); }
		public virtual void Setup_FixedUpdate() {}

		public virtual void PreBattle() { LogState(); }
		public virtual void PreBattle_FixedUpdate() { }

		public virtual void Begin() { LogState(); }
		public virtual void Begin_FixedUpdate() { }

		public virtual void Battle() { LogState(); }
		public virtual void Battle_FixedUpdate() { }

		public virtual void End() { LogState(); }
		public virtual void End_FixedUpdate() { }

		public virtual void PostBattle() { LogState(); }
		public virtual void PostBattle_FixedUpdate() { }

		public virtual void Teardown() { LogState(); }
		public virtual void Teardown_FixedUpdate() {}

		//public virtual void Paused(){ LogState(); }
		//public virtual void Paused_FixedUpdate(){ }
	}
}
