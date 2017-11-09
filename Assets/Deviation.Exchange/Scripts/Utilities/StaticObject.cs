using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Utilities
{
	//this script makes an object persists after the switch of a scene
    public class StaticObject : MonoBehaviour
    {
        public void Awake()
        {
			//makes the game object persist after scene
            DontDestroyOnLoad(gameObject);
        }
    }
}

