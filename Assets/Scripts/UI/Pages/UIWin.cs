using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace TileAdventure
{
    public class UIWin : UIPage
    {
        [Header("Buttons")]
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _homeButton;

        protected override void Awake()
        {
            base.Awake();

            _nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            _homeButton.onClick.AddListener(OnHomeClicked);
        }

        protected override void OnShown()
        {
            Vector3 originalNextLevelScale = _nextLevelButton.transform.localScale;
            Vector3 originalHomeScale = _homeButton.transform.localScale;

            Sequence showSequence = DOTween.Sequence();
            showSequence.Append(_nextLevelButton.transform.DOScale(originalNextLevelScale, 0.3f).From().SetEase(Ease.OutBack));
            showSequence.AppendInterval(0.1f);
            showSequence.Join(_homeButton.transform.DOScale(originalHomeScale, 0.3f).From().SetEase(Ease.OutBack));
        }

        private void OnNextLevelClicked()
        {
            LoadSceneManager.LoadScene(Constant.GAMEPLAY_SCENE_NAME).Forget();
        }

        private void OnHomeClicked()
        {
            LoadSceneManager.LoadScene(Constant.HOME_SCENE_NAME).Forget();
        }
    }
}
