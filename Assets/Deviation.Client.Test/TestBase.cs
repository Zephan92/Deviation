using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Deviation.Client.Test
{
	public class TestBase : MonoBehaviour
	{
		public GameObject TestingSuite;

		public virtual void Awake()
		{
			if (!Application.isEditor && !UnityEngine.Debug.isDebugBuild)
			{
				Destroy(gameObject.transform.root.gameObject);
			}

			TestingSuite = GameObject.Find("TestingSuite");
		}

		public virtual void Start()
		{

		}
	}
}
