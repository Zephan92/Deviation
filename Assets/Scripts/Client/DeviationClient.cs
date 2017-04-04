using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Client
{
	public class DeviationClient : MonoBehaviour, IDeviationClient
	{
		public IPlayerAccount currentPlayer { get; set; }

		public void Awake()
		{
			IResourceBag resourceBag = new ResourceBag();
			currentPlayer = new PlayerAccount("CrazyJello15","Jello Eater", resourceBag);
		}
	}
}
