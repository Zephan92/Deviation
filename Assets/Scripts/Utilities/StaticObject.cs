using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Utilities
{
    public class StaticObject : MonoBehaviour
    {

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

