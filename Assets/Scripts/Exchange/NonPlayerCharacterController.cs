using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Exchange
{
	public class NonPlayerCharacterController : MonoBehaviour
	{
		private bool _readyToMakeDecision;
		private IEnumerator _coroutine;

		public void Awake()
		{
			_readyToMakeDecision = true;
		}

		public void StartDecisionMaker()
		{
			if (_coroutine == null)
			{
				_coroutine = DecisionMakerCoroutine();
				StartCoroutine(_coroutine);
			}
		}

		public void PauseDecisionMaker()
		{
			
			StopCoroutine(_coroutine);
			Debug.Log("Paused Decision Maker");
		}

		public void UnpauseDecisionMaker()
		{
			Debug.Log("Unpausing Decision Maker");
			StartCoroutine(_coroutine);
		}

		public void StopDecisionMaker()
		{
			StopCoroutine(_coroutine);
			_coroutine = null;
			Debug.Log("Stopped Decision Maker Thread");
		}

		public IEnumerator DecisionMakerCoroutine()
		{
			Debug.Log("Starting Decision Maker Thread");
			for (int i = 0;;i++)
			{
				yield return new WaitForSeconds(.1f);
			}
		}
	}
}
