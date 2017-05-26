using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ModuleEditor
{
	public class SnapPoint : MonoBehaviour
	{
		public Rect Area;

		public void Awake()
		{
			Area = GetScreenCoordinates(GetComponent<RectTransform>());
		}
		public SnapPoint(Rect area)
		{
			Area = area;
		}

		private Rect GetScreenCoordinates(RectTransform uiElement)
		{
			var worldCorners = new Vector3[4];
			uiElement.GetWorldCorners(worldCorners);
			var result = new Rect(
						  worldCorners[0].x,
						  worldCorners[0].y,
						  worldCorners[2].x - worldCorners[0].x,
						  worldCorners[2].y - worldCorners[0].y);
			return result;
		}
	}
}
