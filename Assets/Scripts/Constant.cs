using UnityEngine;

namespace TileAdventure
{
    public static class Constant
    {
        // Gameplay Logic
        public const int TILE_MATCH_NUMBER = 3;
        public const int MAX_RACK_CAPACITY = 7;
        public const float TILE_MOVE_SPEED = 0.15f;
        
        // Colors
        public static readonly Color LOCKED_COLOR = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Color DIMMED_COLOR = new Color(1f, 1f, 1f, 0.5f);
        
        // UI Settings
        public const int TOTAL_LEVEL_SLOTS = 10;
        public const int DESIGN_WIDTH = 1080;
        public const int DESIGN_HEIGHT = 1920;
        
        // Editor Settings
        public const float EDITOR_CELL_SIZE = 60f;
        public const int DEFAULT_GRID_SIZE = 10;
        public const int MAX_LAYERS = 10;
        public const float PALETTE_BUTTON_SIZE = 40f;

        // Scene Names
        public const string LOAD_SCENE_NAME = "LoadingScene";
        public const string HOME_SCENE_NAME = "Home";
        public const string GAMEPLAY_SCENE_NAME = "GamePlay";
    }
}
