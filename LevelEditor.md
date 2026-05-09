# Tile Adventure - Level Editor Manual

## 1. Open the Tool
Navigate to **Tile Adventure > Level Editor** in the Unity top menu to open the custom editor window.

## 2. Setup References
Before editing, ensure the following fields are assigned in the editor window:
- **Level Database**: Assign the `LevelDatabaseSO` (found in `Assets/SO/LevelData`).
- **Icon Palette**: Assign the `IconPaletteSO` (contains the list of tile icons).
- **Container Sprite**: Assign the `tile-base` sprite used as the background for tiles.

## 3. Level Management
- **Create New Level**: Click this button to generate a new `LevelDataSO` asset file in `Assets/SO/LevelData/LevelDatas`.
- **Select Level**: You can select any existing level from the database list to modify its current layout.
- **Clear Data**: Resets all tile configurations for the currently selected level.

## 4. Editing the Layout
- **Layer Slider**: Use this to choose which depth (layer) you are working on.
- **2D Canvas**: 
    - **Left-click**: Place a tile at the current layer.
    - **Click on existing tile**: Remove the tile or update its icon.
- **Icon Palette**: Select an icon from the palette, then click on tiles in the canvas to assign that specific Icon ID.

## 5. Addressables Configuration (CRITICAL)
Since the game uses an asynchronous loading system, every new level must be registered in the **Addressables** system:
1. Select the newly created `LevelDataSO` file in the Project window.
2. In the **Inspector**, check the **Addressable** checkbox.
3. Open the **Addressables Groups** window (`Window > Asset Management > Addressables > Groups`).
4. Ensure the level asset is placed within the **LevelData** group (or your designated level group).
5. (Optional) Simplify the Address name to match the filename (e.g., `Level_1`).

## 6. Solvability & Persistence
- **Fill Tile Auto**: After placing your tiles, click this button to automatically assign Icon IDs in triplets. This guarantees the level is solvable.

