using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace TileAdventure
{
    public static class LoadSceneManager 
    {
        public static async UniTask LoadScene(string nextSceneName = "")
        {
            nextSceneName = string.IsNullOrEmpty(nextSceneName) ? Constant.HOME_SCENE_NAME : nextSceneName;
            
            await SceneManager.LoadSceneAsync(Constant.LOAD_SCENE_NAME, LoadSceneMode.Additive).ToUniTask();
            Scene loadingScene = SceneManager.GetSceneByName(Constant.LOAD_SCENE_NAME);

            LoadingSceneController controller = FindLoadingController(loadingScene);
            if (controller != null)
            {
                await controller.FadeIn();
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != Constant.LOAD_SCENE_NAME)
            {
                await SceneManager.UnloadSceneAsync(activeScene).ToUniTask();
            }

            var operation = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                if (controller != null)
                {
                    controller.UpdateProgress(operation.progress);
                }

                if (operation.progress >= 0.9f)
                {
                    if (controller != null) controller.UpdateProgress(0.9f);
                    

                    await UniTask.Delay(100); 
                    
                    operation.allowSceneActivation = true;
                }

                await UniTask.Yield();
            }

            Scene newScene = SceneManager.GetSceneByName(nextSceneName);


            SceneManager.SetActiveScene(newScene);

            if (controller != null)
            {
                await controller.FadeOut();
            }

            await SceneManager.UnloadSceneAsync(loadingScene).ToUniTask();
        }

        private static LoadingSceneController FindLoadingController(Scene scene)
        {
            if (!scene.IsValid()) return null;

            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (var go in rootObjects)
            {
                var controller = go.GetComponentInChildren<LoadingSceneController>(true);
                if (controller != null) return controller;
            }
            return null;
        }
    }
}
