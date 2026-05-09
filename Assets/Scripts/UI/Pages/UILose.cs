using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace TileAdventure
{
    public class UILose : UIPage
    {
        [Header("Buttons")]
        [SerializeField] private Button _playAgainButton;
        [SerializeField] private Button _homeButton;

        protected override void Awake()
        {
            base.Awake();
            _playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            _homeButton.onClick.AddListener(OnHomeClicked);
        }


        protected override void OnShown()
        {
            Vector3 originalPlayAgainScale = _playAgainButton.transform.localScale;
            Vector3 originalHomeScale = _homeButton.transform.localScale;

            Sequence showSequence = DOTween.Sequence();
            showSequence.Append(_playAgainButton.transform.DOScale(originalPlayAgainScale, 0.3f).From().SetEase(Ease.OutBack));
            showSequence.AppendInterval(0.1f);
            showSequence.Join(_homeButton.transform.DOScale(originalHomeScale, 0.3f).From().SetEase(Ease.OutBack));
        }



        private void OnPlayAgainClicked()
        {
            LoadSceneManager.LoadScene(Constant.GAMEPLAY_SCENE_NAME).Forget();
        }

        private void OnHomeClicked()
        {
            LoadSceneManager.LoadScene(Constant.HOME_SCENE_NAME).Forget();
        }
    }
}
