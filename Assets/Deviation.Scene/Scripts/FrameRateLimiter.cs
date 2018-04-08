using UnityEngine;
using System.Collections;
using Assets.Scripts.Utilities;

[RequireComponent(typeof(StaticObject))]
public class FrameRateLimiter : MonoBehaviour
{
	public static FrameRateLimiter Instance = null;
	public int FrameRateCap = 60;

	// Use this for initialization
	void Awake()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = FrameRateCap;

		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}
}
