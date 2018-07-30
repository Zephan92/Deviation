using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Exchange.Scripts.Display
{
	public class FadeImage : MonoBehaviour
	{
		public static IEnumerator Fade(Transform parent, Sprite sprite, float beforeWait, float displayWait)
		{
			GameObject splash = new GameObject("SplashImage");
			splash.transform.SetParent(parent);
			Image img = splash.AddComponent<Image>();
			img.sprite = sprite;
			img.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

			yield return new WaitForSeconds(beforeWait);
			FadeIn(img);

			yield return new WaitForSeconds(displayWait);
			FadeOut(img);
		}

		private static void FadeIn(Image img)
		{
			img.CrossFadeAlpha(1.0f, 1.5f, false);
		}

		private static void FadeOut(Image img)
		{
			img.CrossFadeAlpha(0f, 2.5f, false);
		}
	}
}
