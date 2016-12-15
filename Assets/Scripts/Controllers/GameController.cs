using UnityEngine;
using System.Collections;
using Assets.Scripts.Interfaces;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.External;
using System.IO;
using Assets.Scripts.Enum;
using Assets.Scripts.Utilities;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour, IGameController
    {
        public static GameController control;
        public static GameState GameState;

        public void Awake()
        {
            InstantiateGameController();
            DontDestroyOnLoad(gameObject);
        }

        public void Start()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void Update()
        {
            switch (GameState)
            {
                case GameState.Battle:
                    break;
                case GameState.Credits:
                    break;
                case GameState.MainMenu:
                    break;
                case GameState.Multiplayer:
                    break;
                //case GameState.Overworld:
                //    break;
                default:
                    break;
            }
        }

        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

            PlayerData data = new PlayerData();
            //add stuff to data
            //data.playerName = SaveFile.playerName;

            bf.Serialize(file, data);
            file.Close();
        }

        public void Load()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

            PlayerData data = (PlayerData) bf.Deserialize(file);
            file.Close();
            //grab stuff from data
            //SaveFile.playerName = data.playerName;
        }

        private void InstantiateGameController()
        {
            if (control == null)
            {
                control = this;
            }
            else if (control != this)
            {
                Destroy(gameObject);
            }
        }
    }
}

