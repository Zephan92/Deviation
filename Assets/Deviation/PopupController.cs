using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation
{
	public class PopupController : MonoBehaviour
	{
		public Transform NotConnectedToServer;
		public bool RestartOnQuit = false;


		private void Awake()
		{
			NotConnectedToServer = transform.Find("NotConnectedToServer");
		}

		public void ToggleNotConnectedToServerPopup(bool active)
		{
			NotConnectedToServer = transform.Find("NotConnectedToServer");
			NotConnectedToServer.gameObject?.SetActive(active);
		}

		public void ShutdownClient()
		{
			Application.Quit();
		}

		public void RestartClient()
		{
			if (!Application.isEditor)
			{
				RestartOnQuit = true;
				Application.Quit();
			}
		}

		public void OnApplicationQuit()
		{
			if (RestartOnQuit)
			{
				System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
			}
		}
	}
}