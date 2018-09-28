using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.UserInterface
{
	public class DialogPopup : MonoBehaviour
	{
		public Text Title;
		public Text Content;

		public Transform DialogBox;

		public Button Action1;
		public Button Action2;
		public Button CloseButton;

		public void Awake()
		{
			transform.Find("Focus").GetComponent<Button>().onClick.AddListener(Close);
			DialogBox = transform.Find("DialogBox");
			Title = DialogBox.Find("Details").Find("Title").GetComponent<Text>();
			Content = DialogBox.Find("Details").Find("Content").GetComponent<Text>();
			Action1 = DialogBox.Find("Actions").Find("Action1").GetComponent<Button>();
			Action2 = DialogBox.Find("Actions").Find("Action2").GetComponent<Button>();
			CloseButton = DialogBox.Find("CloseButton").GetComponent<Button>();
			CloseButton.onClick.AddListener(Close);
		}

		public void Init(string title, string content, string actionName1, UnityAction actionOnClick1)
		{
			Title.text = title;
			Content.text = content;
			Action1.GetComponentInChildren<Text>().text = actionName1;
			Action1.onClick.AddListener(actionOnClick1);
			Action1.onClick.AddListener(Close);
			Action2.gameObject.SetActive(false);
		}

		public void Init(string title, string content, string actionName1, UnityAction actionOnClick1, string actionName2, UnityAction actionOnClick2)
		{
			Title.text = title;
			Content.text = content;
			Action1.GetComponentInChildren<Text>().text = actionName1;
			Action1.onClick.AddListener(actionOnClick1);
			Action1.onClick.AddListener(Close);
			Action2.GetComponentInChildren<Text>().text = actionName2;
			Action2.onClick.AddListener(actionOnClick2);
			Action2.onClick.AddListener(Close);
		}

		public void Close()
		{
			Destroy(gameObject);
		}
	}
}
