using System;
using System.Collections;

namespace Assets.Scripts.Interface
{
	public interface ICoroutineManager
	{
		void PauseCoroutineThread(ref IEnumerator coroutine);
		void UnpauseCoroutineThread(ref IEnumerator coroutine);
		void StopCoroutineThread(ref IEnumerator coroutine);

		void StartCoroutineThread(Action method, ref IEnumerator coroutine);
		void StartCoroutineThread(Action<object[]> method, object[] parameters, ref IEnumerator coroutine);

		void StartFixedCoroutineThread(Action method, ref IEnumerator coroutine);
		void StartFixedCoroutineThread(Action<object[]> method, object[] parameters, ref IEnumerator coroutine);

		void StartCoroutineThread_ForLoop(Action<int> method, float interval, ref IEnumerator coroutine);
		void StartCoroutineThread_ForLoop(Action<int, object[]> method, object[] parameters, float interval, ref IEnumerator coroutine);

		void StartCoroutineThread_WhileLoop(Action method, float interval, ref IEnumerator coroutine);
		void StartCoroutineThread_WhileLoop(Action<object[]> method, object[] parameters, float interval, ref IEnumerator coroutine);

		void StartCoroutineThread_AfterTimout(Action method, float interval, ref IEnumerator coroutine);
		void StartCoroutineThread_AfterTimout(Action<object[]> method, object[] parameters, float interval, ref IEnumerator coroutine);
	}
}
