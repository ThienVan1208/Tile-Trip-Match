using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TileAdventure
{
    public class UIGameplay : UIPage
    {
        [Header("Buttons")]
        [SerializeField] private Button _backButton;
        protected override void Awake()
        {
            base.Awake();
            _backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnBackButtonClicked()
        {
            LoadSceneManager.LoadScene(Constant.HOME_SCENE_NAME).Forget();
        }
    }
}