using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.UserInterface
{
	public class TraderSelectScrollPanel : MonoBehaviour
	{
		public int MaxListSize;
		public float ItemWidth;
		public GameObject List;
		public RectTransform UIBoundary;
		public Rect Boundary;
		public Scrollbar Scroll;
		public HorizontalLayoutGroup LayoutGroup;

		private bool _scrollEnabled;
		private int _itemCount;

		public void Awake()
		{
			List = transform.Find("List").gameObject;
			UIBoundary = GetComponent<RectTransform>();
			Scroll = GetComponentInChildren<Scrollbar>();
			LayoutGroup = GetComponentInChildren<HorizontalLayoutGroup>();
			Boundary = GetBoundaryFromRectTransform(UIBoundary);
			Scroll.onValueChanged.AddListener(OnScroll);
		}

		public void Update()
		{
			if (Boundary.x < 0)
			{
				Boundary = GetBoundaryFromRectTransform(UIBoundary);
			}

			if (_scrollEnabled && Boundary.Contains(Input.mousePosition) && Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				Scroll.value -= Input.GetAxis("Mouse ScrollWheel")/2;
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
			Scroll.value = 0.0f;
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
			MoveList((int)(value * -overflowCount * ItemWidth));
		}

		private void MoveList(int value)
		{
			RectOffset tempPadding = new RectOffset(
				 LayoutGroup.padding.left,
				 LayoutGroup.padding.right,
				 LayoutGroup.padding.top,
				 LayoutGroup.padding.bottom);

			tempPadding.left = value;
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
