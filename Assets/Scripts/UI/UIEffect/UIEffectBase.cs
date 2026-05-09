using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TileAdventure
{
    public abstract class UIEffectBase : MonoBehaviour
    {
        [SerializeField] protected float duration = 0.3f;
        [SerializeField] protected AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);


        public abstract UniTask Play(RectTransform target);

        public abstract void ResetEffect(RectTransform target);
    }
}
