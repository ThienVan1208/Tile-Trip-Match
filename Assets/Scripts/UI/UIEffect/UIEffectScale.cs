using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace TileAdventure
{
    public class UIEffectScale : UIEffectBase
    {
        [Header("Scale Settings")]
        [SerializeField] private Vector3 fromScale = Vector3.zero;
        [SerializeField] private Vector3 toScale = Vector3.one;

        public override async UniTask Play(RectTransform target)
        {
            target.DOKill();
            target.localScale = fromScale;

            var tcs = new UniTaskCompletionSource();
            target.DOScale(toScale, duration)
                .SetEase(curve)
                .SetUpdate(true)
                .OnComplete(() => tcs.TrySetResult())
                .OnKill(() => tcs.TrySetCanceled());

            await tcs.Task;
        }

        public override void ResetEffect(RectTransform target)
        {
            target.DOKill();
            target.localScale = fromScale;
        }
    }
}
