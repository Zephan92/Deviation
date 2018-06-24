using Assets.Deviation.Exchange.Scripts.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Deviation.Exchange.Scripts
{
	public class Splat : NetworkBehaviour
	{
		public GameObject SplatGO;
		public Transform SplatTransform;
		public ObjectPool<GameObject> DamageSplatTextPool;
		
		public void Start()
		{
			SplatGO = GameObject.Find("ExchangeCanvas");
			SplatTransform = SplatGO.transform.Find("Splat");

			if (SplatTransform.childCount == 0)
			{
				DamageSplatTextPool = new ObjectPool<GameObject>(5, CreateDamageSplat);
			}
			else
			{
				DamageSplatTextPool = new ObjectPool<GameObject>(0, CreateDamageSplat);
				Debug.LogError("Hmm some how we already have objects");
				Text[] pool = SplatTransform.GetComponentsInChildren<Text>();
				foreach (var text in pool)
				{
					DamageSplatTextPool.Release(text.gameObject);
				}
			}
		}

		public void TakeDamage(int value, Vector3 playerPosition)
		{
			Vector2 screenPosition = Camera.main.WorldToScreenPoint(playerPosition);
			GameObject damageSplat = DamageSplatTextPool.Get();
			damageSplat.SetActive(true);
			Text damageSplatText = damageSplat.GetComponentInChildren<Text>();
			damageSplatText.text = value.ToString();
			damageSplat.transform.position = screenPosition;
			RpcTakeDamage(value, playerPosition);
			StartCoroutine(ReleaseDamageSplat(damageSplat));
		}

		[ClientRpc]
		private void RpcTakeDamage(int value, Vector3 playerPosition)
		{
			Vector2 screenPosition = Camera.main.WorldToScreenPoint(playerPosition);
			GameObject damageSplat = DamageSplatTextPool.Get();
			damageSplat.SetActive(true);
			Text damageSplatText = damageSplat.GetComponentInChildren<Text>();
			damageSplatText.text = value.ToString();
			damageSplat.transform.position = screenPosition;
			StartCoroutine(ReleaseDamageSplat(damageSplat));
		}

		private IEnumerator ReleaseDamageSplat(GameObject poolObject)
		{
			yield return new WaitForSeconds(1f);
			poolObject.SetActive(false);
			DamageSplatTextPool.Release(poolObject);
		}

		private GameObject CreateDamageSplat()
		{
			var gameObject = (GameObject)Instantiate(Resources.Load("DamageSplat"), new Vector3(), new Quaternion(), SplatTransform);
			gameObject.SetActive(false);
			return gameObject;
		}
	}
}
