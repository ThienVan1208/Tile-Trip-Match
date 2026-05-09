using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace TileAdventure
{
    public class UIEffectFade : UIEffectBase
    {
        [Header("Fade Settings")]
        [SerializeField] private float fromAlpha = 0f;
        [SerializeField] private float toAlpha = 1f;

        [SerializeField]private CanvasGroup _canvasGroup;

        public override async UniTask Play(RectTransform target)
        {
            if (_canvasGroup == null)
            {
                return;
            }

            _canvasGroup.DOKill();
            _canvasGroup.alpha = fromAlpha;

            var tcs = new UniTaskCompletionSource();
            _canvasGroup.DOFade(toAlpha, duration)
                .SetEase(curve)
                .SetUpdate(true)
                .OnComplete(() => tcs.TrySetResult())
                .OnKill(() => tcs.TrySetCanceled());

            await tcs.Task;
        }

        public override void ResetEffect(RectTransform target)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.DOKill();
                _canvasGroup.alpha = fromAlpha;
            }
        }
    }
}
