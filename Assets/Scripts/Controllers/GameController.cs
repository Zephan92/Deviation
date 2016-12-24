using UnityEngine;
using Assets.Scripts.Interface;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.External;
using System.IO;
using Assets.Scripts.Enum;
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

		//this saves the player data file
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

		//this loads the player data file
        public void Load()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

            PlayerData data = (PlayerData) bf.Deserialize(file);
            file.Close();
            //grab stuff from data
            //SaveFile.playerName = data.playerName;
        }

		//this makes sure that there is only one game controler
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

