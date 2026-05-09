## 1. System Architecture
I use the MVP pattern combined with Event-Driven Architecture (ScriptableObject-based)  .

Reason:
- Decoupling:Gameplay logic (Model) is entirely unaware of visual implementations like DOTween or Particle Systems (View). This allows UI/Visual changes without affecting core game mechanics.
- Scalability: Using ScriptableObjects as Event Channels allows disparate systems (Audio, UI, Gameplay) to communicate without direct references or over-reliance on Singletons, preventing Spaghetti code.
- Easy to test: This modular approach makes the project easier to Unit Test and highly maintainable.

---

## 2. Level Data Structure
Level data is structured using ScriptableObjects (`LevelDataSO`):
- Data: Each level consists of a list of `TileSpawnInfor`, which stores grid coordinates (X, Y), layer index (Z), and Icon ID.
- Hierarchical Management: The Layer Index combined with specific offsets allows for complex 3D stacking (e.g., Pyramids or overlapping piles).
- Memory Optimization: Instead of loading all 10 levels into memory at startup, I implemented Addressables for lazy loading. Levels are loaded on-demand.

---

## 3. Guaranteed Solvability
To ensure 100% solvability for every level, I developed a custom editor tool with auto tile fill feature:
1. It traverses the layout from the top layer to the bottom.
2. It identifies exposed tiles (those not covered by higher layers) and assigns matching Icon IDs in triplets.
3. This process repeats until all tiles are colored, ensuring that at any point during gameplay, at least one valid match is accessible to the player.

---

## 4. Trade-offs & Future Improvements
- Level Editor & Workflow Automation: A future improvement would be to fully automate level editor tool.
- Advanced Auto Tile Filling: The current `Fill-Tile-Auto` algorithm guarantees solvability by assigning triplets top-down. To generate more complex and challenging levels, this could be upgraded using a backtracking or recursive search algorithm that factors in the player's limited Rack capacity, simulating actual gameplay constraints to ensure the generated layout is not only solvable but appropriately balanced.
- **Raycast & Component Caching:** To further optimize the input system, a caching mechanism (e.g., a Dictionary mapping Colliders to TileBehaviors) could replace repeated `GetComponent` calls during raycasting, reducing CPU overhead during intense gameplay.
- **Dynamic Paged Level Selector:** The current level selector is optimized for 10 levels. For a production environment with hundreds of levels, a paged UI system with navigation arrows would be implemented to prevent UI overflow and improve the user experience (UX).
- **Visual Polish & Art Consistency:** In this version, a more unified art direction would be established to ensure all assets are consistent. This would be paired with advanced VFX and custom shaders to provide a more "juicy" and premium feel to the game's feedback systems.
