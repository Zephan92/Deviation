using System;
using System.Collections;

namespace Assets.Scripts.Interface
{
	public interface ICoroutineManager
	{
		void StartCoroutineThread_ForLoop(Action<int> method, float interval, ref IEnumerator coroutine);
		void StartCoroutineThread_WhileLoop(Action method, float interval, ref IEnumerator coroutine);
		void StartCoroutineThread_AfterTimout(Action method, float interval, ref IEnumerator coroutine);
		void PauseCoroutineThread(ref IEnumerator coroutine);
		void UnpauseCoroutineThread(ref IEnumerator coroutine);
		void StopCoroutineThread(ref IEnumerator coroutine);
	}
}
