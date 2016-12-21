using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
	//this is the main menu utilties
    public class Multiplayer : MonoBehaviour
    {
		//this function loads the multiplayer exchange scene
        public void MultiplayerExchange()
        {
            SceneManager.LoadScene("MultiplayerExchange");
        }
    }
}
