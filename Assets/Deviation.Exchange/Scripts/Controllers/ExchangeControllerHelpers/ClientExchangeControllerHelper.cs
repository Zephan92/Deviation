using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

namespace Assets.Deviation.Exchange.Scripts.Controllers.ExchangeControllerHelpers
{
	public interface IClientExchangeControllerHelper : IExchangeControllerHelper
	{
	}

	public class ClientExchangeControllerHelper : ExchangeControllerHelper, IClientExchangeControllerHelper
	{
		private ExchangeNetworkManager etm;

		public override void Init()
		{
			base.Init(ExchangeControllerHelperType.Client);
		}

		public void Awake()
		{
			etm = FindObjectOfType<ExchangeNetworkManager>();
		}

		public override void Teardown()
		{
			base.Teardown();

			etm.StopClient();
			SceneManager.LoadScene("DeviationClient - Results");
		}
	}
}
