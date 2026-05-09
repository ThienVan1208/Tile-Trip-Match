using DG.Tweening;
using UnityEngine;

namespace TileAdventure
{

    public class UIHome : UIPage
    {
        [SerializeField] private RectTransform _logoRect;
        protected override async void Awake()
        {
            base.Awake();
            _logoRect.gameObject.SetActive(false);
            await UIController.OpenUI<UIHome>();
        }
        protected override void OnShown()
        {
            _logoRect.localScale = Vector3.zero;
            _logoRect.gameObject.SetActive(true);
            _logoRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

    }
}
