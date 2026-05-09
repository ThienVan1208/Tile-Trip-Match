using UnityEngine;

namespace TileAdventure
{
    public struct ScreenOrientationEventArgs
    {
        public bool IsPortrait;
        public Vector2 ScreenSize;
        public ScreenOrientationEventArgs(bool isPortrait, Vector2 screenSize)
        {
            IsPortrait = isPortrait;
            ScreenSize = screenSize;
        }
    }

    [CreateAssetMenu(menuName = "Events/Screen Orientation Event", fileName = "ScreenOrientationEvent")]
    public class ScreenOrientationEventSO : EventChannelSO<ScreenOrientationEventArgs>
    {
    }
}
