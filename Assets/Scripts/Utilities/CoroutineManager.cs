using Assets.Scripts.Interface;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public class CoroutineManager : MonoBehaviour, ICoroutineManager
	{
		public void StartCoroutineThread_ForLoop(Action<int> method, float interval, ref IEnumerator coroutine)
		{
			StartCoroutine(coroutine = ForLoop_Coroutine(method, interval));
		}

		public void StartCoroutineThread_WhileLoop(Action method, float interval, ref IEnumerator coroutine)
		{
			StartCoroutine(coroutine = WhileLoop_Coroutine(method, interval));
		}

		public void StartCoroutineThread_AfterTimout(Action method, float interval, ref IEnumerator coroutine)
		{
			StartCoroutine(coroutine = AfterTimout_Coroutine(method, interval));
		}

		public void StartCoroutineThread_ForLoop(Action<int, object[]> method, object[] parameters, float interval, ref IEnumerator coroutine)
		{
			StartCoroutine(coroutine = ForLoop_Coroutine(method, parameters, interval));
		}

		public void StartCoroutineThread_WhileLoop(Action<object[]> method, object[] parameters, float interval, ref IEnumerator coroutine)
		{
			StartCoroutine(coroutine = WhileLoop_Coroutine(method, parameters, interval));
		}

		public void StartCoroutineThread_AfterTimout(Action<object[]> method, object[] parameters, float interval, ref IEnumerator coroutine)
		{
			StartCoroutine(coroutine = AfterTimout_Coroutine(method, parameters, interval));
		}

		public void PauseCoroutineThread(ref IEnumerator coroutine)
		{
			StopCoroutine(coroutine);
		}

		public void UnpauseCoroutineThread(ref IEnumerator coroutine)
		{
			StartCoroutine(coroutine);
		}

		public void StopCoroutineThread(ref IEnumerator coroutine)
		{
			StopCoroutine(coroutine);
			coroutine = null;
		}

		private IEnumerator ForLoop_Coroutine(Action<int> coroutine, float interval)
		{
			for (int i = 0; ; i++)
			{
				coroutine(i);
				yield return new WaitForSeconds(interval);
			}
		}

		private IEnumerator WhileLoop_Coroutine(Action coroutine, float interval)
		{
			while(true)
			{
				coroutine();
				yield return new WaitForSeconds(interval);
			}
		}

		private IEnumerator AfterTimout_Coroutine(Action coroutine, float timeout)
		{
			yield return new WaitForSeconds(timeout);
			coroutine();
		}

		private IEnumerator ForLoop_Coroutine(Action<int, object []> coroutine, object [] parameters, float interval)
		{
			for (int i = 0; ; i++)
			{
				coroutine(i, parameters);
				yield return new WaitForSeconds(interval);
			}
		}

		private IEnumerator WhileLoop_Coroutine(Action<object []> coroutine, object[] parameters, float interval)
		{
			while (true)
			{
				coroutine(parameters);
				yield return new WaitForSeconds(interval);
			}
		}

		private IEnumerator AfterTimout_Coroutine(Action<object[]> coroutine, object[] parameters, float timeout)
		{
			yield return new WaitForSeconds(timeout);
			coroutine(parameters);
		}
	}
}
