using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.UserInterface
{
	public class VerticalScrollPanel : MonoBehaviour
	{
		public int MaxListSize;
		public float ItemHeight;
		public GameObject List;
		public RectTransform UIBoundary;
		public Rect Boundary;
		public Scrollbar Scroll;
		public VerticalLayoutGroup LayoutGroup;

		private bool _scrollEnabled;
		private int _itemCount;

		public void Awake()
		{
			List = transform.Find("List").gameObject;
			UIBoundary = GetComponent<RectTransform>();
			Scroll = GetComponentInChildren<Scrollbar>();
			LayoutGroup = GetComponentInChildren<VerticalLayoutGroup>();

			Scroll.onValueChanged.AddListener(OnScroll);
			PanelEnabled(false);
		}

		public void Start()
		{
		
		}

		public void Update()
		{
			if (Boundary.x <= 0)
			{
				Boundary = GetBoundaryFromRectTransform(UIBoundary);
			}

			if (_scrollEnabled && Boundary.Contains(Input.mousePosition) && Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				Scroll.value -= Input.GetAxis("Mouse ScrollWheel");
			}
		}

		public void PanelEnabled(bool enabled)
		{
			Scroll.gameObject.SetActive(enabled);
			_scrollEnabled = enabled;
			ResetPanel();
		}

		public void ResetPanel()
		{
			MoveList(0);
			Scroll.value = 0;
		}

		public void OnListChange(int itemCount)
		{
			PanelEnabled(itemCount > MaxListSize);
			ResetPanel();
			_itemCount = itemCount;
		}

		private void OnScroll(float value)
		{
			int overflowCount = _itemCount - MaxListSize;
			MoveList((int)(value * -overflowCount * ItemHeight));
		}

		private void MoveList(int value)
		{
			RectOffset tempPadding = new RectOffset(
				 LayoutGroup.padding.left,
				 LayoutGroup.padding.right,
				 LayoutGroup.padding.top,
				 LayoutGroup.padding.bottom);
			tempPadding.top = value;
			LayoutGroup.padding = tempPadding;
		}

		private Rect GetBoundaryFromRectTransform(RectTransform uiboundary)
		{
			var worldCorners = new Vector3[4];
			uiboundary.GetWorldCorners(worldCorners);
			var boundary = new Rect(
				worldCorners[0].x,
				worldCorners[0].y,
				worldCorners[2].x - worldCorners[0].x,
				worldCorners[2].y - worldCorners[0].y);
			
			return boundary;
		}
	}
}
