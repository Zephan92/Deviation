using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Utilities
{
	//this script makes an object persists after the switch of a scene
    public class StaticObject : MonoBehaviour
    {
		public void Start()
		{
			//makes the game object persist after scene
			DontDestroyOnLoad(gameObject);
		}
	}
}