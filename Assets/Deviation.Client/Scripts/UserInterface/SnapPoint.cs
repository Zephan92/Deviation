using Assets.Scripts.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.ModuleEditor
{
	public class SnapPoint : MonoBehaviour
	{
		public Rect Area;
		public GameObject CurrentOccupant;
		public UnityAction onOccupied;
		public UnityAction onUnoccupied;

		public bool NeverOccupied = false;
		private bool _isOccupied = false;
		public bool IsOccupied { get { return _isOccupied && !NeverOccupied; } }

		public void Awake()
		{
			_isOccupied = false;
			Area = GetScreenCoordinates(GetComponent<RectTransform>());
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

		public void OccupySnap(GameObject currentOccupant)
		{
			CurrentOccupant = currentOccupant;
			_isOccupied = true;
			onOccupied?.Invoke();
		}

		public void UnoccupySnap()
		{
			CurrentOccupant = null;
			_isOccupied = false;
			onUnoccupied?.Invoke();
		}
	}
}
