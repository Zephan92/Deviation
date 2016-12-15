using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
    public class Main : MonoBehaviour
    {
        public void Awake()
        {
            
        }

        public void Multiplayer()
        {
            SceneManager.LoadScene("MultiplayerExchange");
        }
    }
}
