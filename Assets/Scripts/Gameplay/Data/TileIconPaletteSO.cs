using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileIconPaletteSO", menuName = "Tile Icon Palette")]
public class TileIconPaletteSO : ScriptableObject 
{
    [SerializeField] private List<Sprite> iconList = new List<Sprite>();
    public List<Sprite> IconList => iconList;
}