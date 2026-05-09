using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace TileAdventure
{
    public class LoadingSceneController : MonoBehaviour
    {
        [SerializeField] private UIEffectBase _fadeInEffect;
        [SerializeField] private UIEffectBase _fadeOutEffect;
        [SerializeField] private Image _progressBar;
        
        [SerializeField]private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            
            if (_fadeInEffect != null)
            {
                _fadeInEffect.ResetEffect(_rectTransform);
            }

            if (_progressBar != null)
            {
                _progressBar.fillAmount = 0f;
            }
        }

        public void UpdateProgress(float progress)
        {
            if (_progressBar != null)
            {
                _progressBar.fillAmount = Mathf.Clamp01(progress / 0.9f);
            }
        }

        public async UniTask FadeIn()
        {
            if (_fadeInEffect != null)
            {
                await _fadeInEffect.Play(_rectTransform);
            }
        }

        public async UniTask FadeOut()
        {
            if (_fadeOutEffect != null)
            {
                await _fadeOutEffect.Play(_rectTransform);
            }

            _rectTransform.gameObject.SetActive(false);
        }
    }
}
