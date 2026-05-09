using UnityEngine;

namespace TileAdventure
{

    public class UIRoot : MonoBehaviour
    {
        private void Awake()
        {
            UIController.Initialize(transform);
        }
    }
}
