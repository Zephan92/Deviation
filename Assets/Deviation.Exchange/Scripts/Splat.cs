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
		public ObjectPool<GameObject> HealthSplatTextPool;
		public IExchangePlayer Player;

		public void Start()
		{
			Player = GetComponent<ExchangePlayer>();
			SplatGO = GameObject.Find("ExchangeCanvas");
			SplatTransform = SplatGO.transform.Find("Splat");

			DamageSplatTextPool = new ObjectPool<GameObject>(5, CreateDamageSplat);
			HealthSplatTextPool = new ObjectPool<GameObject>(5, CreateHealthSplat);
		}

		public void AddHealth(int value)
		{
			Vector2 screenPosition = GetScreenPosition();
			GameObject splatText;
			if (value < 0)
			{
				splatText = DamageSplatTextPool.Get();
			}
			else
			{
				splatText = HealthSplatTextPool.Get();
			}

			splatText.SetActive(true);
			Text damageSplatText = splatText.GetComponentInChildren<Text>();
			damageSplatText.text = value.ToString();
			splatText.transform.position = screenPosition;
			RpcTakeDamage(value);
			StartCoroutine(ReleaseDamageSplat(splatText));
		}

		[ClientRpc]
		private void RpcTakeDamage(int value)
		{
			Vector2 screenPosition = GetScreenPosition();
			GameObject splatText;
			if (value < 0)
			{
				splatText = DamageSplatTextPool.Get();
			}
			else
			{
				splatText = HealthSplatTextPool.Get();
			}
			splatText.SetActive(true);
			Text damageSplatText = splatText.GetComponentInChildren<Text>();
			damageSplatText.text = value.ToString();
			splatText.transform.position = screenPosition;
			if (value < 0)
			{
				StartCoroutine(ReleaseDamageSplat(splatText));
			}
			else
			{
				StartCoroutine(ReleaseHealthSplat(splatText));
			}
		}

		private IEnumerator ReleaseDamageSplat(GameObject poolObject)
		{
			yield return new WaitForSeconds(1f);
			poolObject.SetActive(false);
			DamageSplatTextPool.Release(poolObject);
		}

		private IEnumerator ReleaseHealthSplat(GameObject poolObject)
		{
			yield return new WaitForSeconds(1f);
			poolObject.SetActive(false);
			HealthSplatTextPool.Release(poolObject);
		}

		private GameObject CreateDamageSplat()
		{
			var gameObject = (GameObject)Instantiate(Resources.Load("DamageSplat"), new Vector3(), new Quaternion(), SplatTransform);
			gameObject.SetActive(false);
			return gameObject;
		}

		private GameObject CreateHealthSplat()
		{
			var gameObject = (GameObject)Instantiate(Resources.Load("HealthSplat"), new Vector3(), new Quaternion(), SplatTransform);
			gameObject.SetActive(false);
			return gameObject;
		}

		private Vector2 GetScreenPosition()
		{
			Vector2 screenPosition = Camera.main.WorldToScreenPoint(Player.Position);

			int x = UnityEngine.Random.Range(-20, 21);
			int y = UnityEngine.Random.Range(-20, 21);

			screenPosition += new Vector2(x, y);

			return screenPosition;
		}
	}
}
