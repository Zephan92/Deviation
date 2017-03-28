using UnityEngine;
using Assets.Scripts.Interface;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.Menu
{
	//this is the main menu utilties
    public class Multiplayer : MonoBehaviour
    {
		private IMultiplayerController mc;

		public void Awake()
		{
			mc = FindObjectOfType<MultiplayerController>();
		}

		//this function loads the multiplayer exchange scene
        public void MultiplayerExchange()
        {
			mc.StartMultiplayerExchangeInstance();
		}
    }
}
