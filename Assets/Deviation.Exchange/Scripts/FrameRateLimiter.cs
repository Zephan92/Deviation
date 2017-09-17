using UnityEngine;
using System.Collections;

public class FrameRateLimiter : MonoBehaviour
{
	public int FrameRateCap = 60;
	// Use this for initialization
	void Awake()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = FrameRateCap;
	}
}
