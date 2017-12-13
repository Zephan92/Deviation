using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Barebones.MasterServer;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using Assets.Deviation.Exchange.Scripts.Client;

public class ClientLoginController : MonoBehaviour
{
	public InputField Username;
	public InputField Password;
	public Toggle RememberMe;

	private EventSystem system;
	private Button[] buttons;
	private ClientDataController cdc;

	void Start()
	{
		cdc = FindObjectOfType<ClientDataController>();
		system = EventSystem.current;

		if (PlayerPrefs.HasKey("RememberUsername"))
		{
			string remUser = PlayerPrefs.GetString("RememberUsername");

			if(remUser.Equals("True"))
			{
				RememberMe.isOn = true;
			}
		} 

		if (PlayerPrefs.HasKey("Username") && RememberMe.isOn)
		{
			Username.text = PlayerPrefs.GetString("Username");
		}

		buttons = FindObjectsOfType<Button>();

		foreach (Button button in buttons)
		{
			if (button.name.Equals("Sign In Button") &&          
				button.interactable)
			{
				button.interactable = false;
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab) && system.currentSelectedGameObject != null)
		{
			Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

			if (next != null)
			{
				InputField inputfield = next.GetComponent<InputField>();
				if (inputfield != null)
				{
					inputfield.OnPointerClick(new PointerEventData(system));
				}

				system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
			}
		}

		foreach (Button button in buttons)
		{
			if (button.name.Equals("Sign In Button") &&
				!button.interactable &&
				!Username.text.Equals("") && 
				!Password.text.Equals("") &&
				Msf.Client.Connection.IsConnected)
			{
				button.interactable = true;
			}
		}
	}

	public void Login()
	{
		OnRememberMe();

		Msf.Client.Auth.LogInAsGuest((successful, error) =>
		{
			UnityEngine.Debug.Log("Is successful: " + successful + "; Error (if exists): " + error);
			cdc.GetPlayerAccount();
			SceneManager.LoadScene("DeviationClient - Client");
		});

		//Msf.Client.Auth.LogIn(Username.text, Password.text, (successful, error) =>
		//{
		//	UnityEngine.Debug.Log("Is successful: " + successful + "; Error (if exists): " + error);
		//});

	}

	public void CreateAccount()
	{
		Application.OpenURL("http://unity3d.com/");
	}
	public void OnRememberMe()
	{
		if (!Username.text.Equals("") && RememberMe.isOn)
		{
			PlayerPrefs.SetString("Username", Username.text);
			PlayerPrefs.SetString("RememberUsername", "True");
			PlayerPrefs.Save();
		}
		else if(!RememberMe.isOn)
		{
			PlayerPrefs.SetString("Username", "");
			PlayerPrefs.SetString("RememberUsername", "False");
		}
	}
}
