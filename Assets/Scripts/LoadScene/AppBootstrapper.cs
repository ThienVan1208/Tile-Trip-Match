using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TileAdventure
{
    public class AppBootstrapper : MonoBehaviour
    {
        private void Start()
        {
            LoadSceneManager.LoadScene(Constant.HOME_SCENE_NAME).Forget();
        }
    }
}
