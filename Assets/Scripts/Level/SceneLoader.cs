using Logy.UnityCommonV01;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace GarenaGameJam2026Team6
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;

        private void Start()
        {
            _startButton.onClick.AddListener(LoadGameScene);
            SFXPlayer.instance.Play(AudioName.Menu);
        }

        private void LoadGameScene()
        {
            SFXPlayer.instance.PlayOneShot(AudioName.menuClick);
            Invoke("loadScene", 1f);
        }
    
        void loadScene()
        {
            SceneManager.LoadScene("Game");
            SFXPlayer.instance.Play(AudioName.Game, _isLoop: true);
        }
    }


}
