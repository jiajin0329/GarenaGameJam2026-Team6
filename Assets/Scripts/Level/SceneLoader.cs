using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarenaGameJam2026Team6
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;

        private void Start()
        {
            _startButton.onClick.AddListener(LoadGameScene);
        }

        private void LoadGameScene()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
