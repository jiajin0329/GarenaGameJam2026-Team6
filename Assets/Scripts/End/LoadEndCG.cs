using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarenaGameJam2026Team6
{
    public class LoadEndCG : MonoBehaviour
    {
        public static EndType endType;

        [SerializeField]
        private EndCgConfig _endCgConfig;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private Image _black;

        [SerializeField]
        private Button _returnTitleButton;

        [SerializeField]
        private CanvasGroup _returnTitleButtonCanvasGroup;

        private bool _canReturnTitle = false;

        public enum EndType
        {
            AllCharacterEnd,
            CharacterAEnd,
            CharacterBEnd,
            CharacterCEnd,
            BadEnd
        }

        private void Awake()
        {
            switch (endType)
            {
                case EndType.AllCharacterEnd:
                    _image.sprite = _endCgConfig.allCharacterEndCg;
                    break;
                case EndType.CharacterAEnd:
                    _image.sprite = _endCgConfig.characterAEndCg;
                    break;
                case EndType.CharacterBEnd:
                    _image.sprite = _endCgConfig.characterBEndCg;
                    break;
                case EndType.CharacterCEnd:
                    _image.sprite = _endCgConfig.characterCEndCg;
                    break;
                case EndType.BadEnd:
                    _image.sprite = _endCgConfig.badEndCg;
                    break;
            }

            _returnTitleButton.onClick.AddListener(ReturnTitle);

            BlackFadeOut();
        }

        private void BlackFadeOut()
        {
            _black.color = Color.gray1;
            _returnTitleButtonCanvasGroup.alpha = 0f;
            _black.DOFade(0f, 3f).SetEase(Ease.Linear);
            _returnTitleButtonCanvasGroup.DOFade(1f, 3f).SetDelay(3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _canReturnTitle = true;
            });
        }

        private void ReturnTitle()
        {
            if (!_canReturnTitle)
                return;

            SceneManager.LoadScene("TitleScene");
        }
    }
}
