using UnityEngine;

namespace Assets.Scripts.Exchange
{
	public class ProjectileOnHit : MonoBehaviour
	{
		void Awake()
		{
			if (gameObject.name.Equals("PortalRocket(Clone)"))
			{
				GetComponent<Rigidbody>().velocity = 2 * new Vector3(0, 0, -15);
			}

			if (gameObject.name.Equals("Rocket(Clone)"))
			{
				GetComponent<Rigidbody>().velocity = 2 * new Vector3(0, 0, 15);
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.tag.Equals("Player") || other.tag.Equals("MainPlayer"))
			{
				Debug.Log("I hit " + other.name + " I'm destroying myself!!");
				Destroy(gameObject);
			}
		}

		public void Update()
		{
			if (gameObject.transform.position.y <= -15.0)
			{
				Destroy(gameObject);
			}
		}
	}
}
