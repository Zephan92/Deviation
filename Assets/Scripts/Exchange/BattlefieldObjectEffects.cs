
using UnityEngine;

namespace Assets.Scripts.Exchange
{
	public enum BattlefieldObjectEffect
	{
		FloatX,
		FloatY,
		FloatZ,
		CycleXY,
		CycleXZ,
		CycleYZ
	}

	public class BattlefieldObjectEffects : MonoBehaviour
	{

		public BattlefieldObjectEffect Effect = BattlefieldObjectEffect.FloatY;
		public bool RandomRotation;
		public bool RandomRadius;
		public int height;
		public int speed;
		private double y;

		public bool rotating = false;
		public Vector3 rotationSpeed;
		public void Awake()
		{
			y = Random.Range(-64, 64);

			if (RandomRadius)
			{
				height = Random.Range(1, 150);
				speed = Random.Range(32, 300);	
			}

			if (RandomRotation)
			{
				rotationSpeed = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1),Random.Range(-1, 1));
			}
		}

		public void FixedUpdate()
		{
			if (rotating)
			{

				transform.Rotate(rotationSpeed.x, rotationSpeed.y, rotationSpeed.z);
			}
			switch (Effect)
			{
				case BattlefieldObjectEffect.FloatX:

					y += System.Math.PI / height;
					transform.position = transform.position + new Vector3((float)System.Math.Sin(y) / speed, 0, 0);
					break;
				case BattlefieldObjectEffect.FloatY:

					y += System.Math.PI / height;
					transform.position = transform.position + new Vector3(0, (float)System.Math.Sin(y) / speed, 0);

					break;
				case BattlefieldObjectEffect.FloatZ:

					y += System.Math.PI / height;
					transform.position = transform.position + new Vector3(0, 0, (float)System.Math.Sin(y) / speed);

					break;
			}
		}
	}
}
