using UnityEngine;
using UnityEngine.InputSystem;
namespace TileAdventure
{
    public class RaycastController : MonoBehaviour
    {
        [SerializeField]private Camera _mainCam;

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ProcessClick();
            }
        }
        private void ProcessClick()
        {
            if (_mainCam == null) return;
            Vector3 mousePos = _mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);
            if (hits.Length > 0)
            {
                TileBehavior topTile = null;
                int highestLayer = -1;
                foreach (var hit in hits)
                {
                    TileBehavior tileBehavior = hit.collider.GetComponent<TileBehavior>();
                    if (tileBehavior != null && tileBehavior.TileData.IsExposed)
                    {
                        if (tileBehavior.TileData.Layer > highestLayer)
                        {
                            highestLayer = tileBehavior.TileData.Layer;
                            topTile = tileBehavior;
                        }
                    }
                }
                if (topTile != null)
                {
                    topTile.Interact();
                }
            }
        }
    }
}
