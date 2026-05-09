using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TileAdventure
{

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPage : MonoBehaviour
    {
        [Header("UI Effect")]
        [SerializeField] protected UIEffectBase showEffect;

        [SerializeField] protected UIEffectBase hideEffect;

        public bool IsShowing { get; private set; }

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public async UniTask Show()
        {
            if (IsShowing) return;

            gameObject.SetActive(true);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            if (showEffect != null)
            {
                await showEffect.Play(_rectTransform);
            }

            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            IsShowing = true;
            OnShown();
        }

        public async UniTask Hide()
        {
            if (!IsShowing) return;

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            if (hideEffect != null)
            {
                await hideEffect.Play(_rectTransform);
            }

            IsShowing = false;
            OnClosed();
            gameObject.SetActive(false);
        }

        protected virtual void OnShown(){}


        protected virtual void OnClosed(){}
    }
}
