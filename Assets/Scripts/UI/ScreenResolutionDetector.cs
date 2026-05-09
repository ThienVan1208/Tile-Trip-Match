using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TileAdventure
{

    public class ScreenResolutionDetector : UIBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private ScreenOrientationEventSO onOrientationChanged;
        [SerializeField] private CanvasScaler _canvasScaler;
        private Vector2 _lastSize;
        


        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            Vector2 currentSize = new Vector2(Screen.width, Screen.height);

            if (currentSize != _lastSize && currentSize.x > 0 && currentSize.y > 0)
            {
                _lastSize = currentSize;
                bool isPortrait = Screen.height > Screen.width;

                if (currentSize.x <= currentSize.y)
                {
                    // Portrait
                    _canvasScaler.referenceResolution = new Vector2(Constant.DESIGN_WIDTH, Constant.DESIGN_HEIGHT);
                    _canvasScaler.matchWidthOrHeight = 1;
                }
                else
                {
                    // Landscape
                    _canvasScaler.referenceResolution = new Vector2(Constant.DESIGN_HEIGHT, Constant.DESIGN_WIDTH);
                    _canvasScaler.matchWidthOrHeight = 0;
                }


                onOrientationChanged?.RaiseEvent(new ScreenOrientationEventArgs(isPortrait, currentSize));

            }
        }
    }
}
