using Assets.Scripts.Interface;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
//using Deviation.Data.PlayerAccount;
//using Deviation.Data.ResourceBag;

namespace Assets.Scripts.Client
{
	public class DeviationClient : MonoBehaviour//, IDeviationClient
	{
		//public IPlayerAccount currentPlayer { get; set; }

		public void Awake()
		{
		//	IResourceBag resourceBag = new ResourceBag();
			//currentPlayer = new PlayerAccount("CrazyJello15","Jello Eater", resourceBag);
			StartCoroutine(GetResourceBag());
		}

		private IEnumerator GetResourceBag()
		{
			UnityWebRequest getreq = UnityWebRequest.Get("http://localhost:50012/api/LootPool");
			yield return getreq.Send();

			if (getreq.isNetworkError)
			{
				//Debug.Log("Error: " + getreq.error);
			}
			else
			{
				//Debug.Log("Received " + getreq.downloadHandler.text);
				//currentPlayer.ResourceBag.AddResource(getreq.downloadHandler.text.Replace("\"", ""));
			}
		}
	}
}
