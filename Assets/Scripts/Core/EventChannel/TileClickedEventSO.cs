using System;
using UnityEngine;

namespace TileAdventure
{
    public struct TileClickedEventArgs
    {
        public TileData TileData;
        public TileBehavior TileBehavior;
        public TileClickedEventArgs(TileData tileData, TileBehavior tileBehavior)
        {
            TileData = tileData;
            TileBehavior = tileBehavior;
        }
    }

    [CreateAssetMenu(menuName = "Events/Tile Clicked Event", fileName = "TileClickedEvent")]
    public class TileClickedEventSO : EventChannelSO<TileClickedEventArgs>
    {

    }
}
