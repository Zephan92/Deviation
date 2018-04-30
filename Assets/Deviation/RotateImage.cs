using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation
{
	public class RotateImage : MonoBehaviour
	{
		public Image RotatingImage;

		public float Speed = 2f;

		void Awake()
		{
			RotatingImage = RotatingImage ?? GetComponent<Image>();
		}

		private void Update()
		{
			RotatingImage.transform.Rotate(Vector3.forward, Time.deltaTime * 360 * Speed);
		}
	}
}
