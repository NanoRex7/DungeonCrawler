using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sprite Sheet",
    menuName = "Custom Assets/Tile Sheet")]
public class TileSheet : SpriteSheet
{
    public Sprite GetTile(bool tl, bool tr, bool bl, bool br, 
        int variant = 0, int animationFrame = 0)
    {
        int col = animationFrame * 4 + (br ? 2 : 0) + (bl ? 1 : 0);
        int row = variant * 4 + (tl ? 2 : 0) + (tr ? 1 : 0);
        return GetSprite(col, row);
    }
}
