using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
	//this is the main menu utilties
    public class Main : MonoBehaviour
    {
		//this function loads the multiplayer exchange scene
        public void Multiplayer()
        {
            SceneManager.LoadScene("MultiplayerMenu");
        }
    }
}
