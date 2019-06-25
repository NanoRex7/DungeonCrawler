using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Sprite Sheet", 
    menuName = "Custom Assets/Sprite Sheet")]
public class SpriteSheet : ScriptableObject
{
    // Texture with sprites as sub-assets
    protected Texture2D texture;

    // 2D array of Sprites by position in sprite sheet
    protected Sprite[,] sprites;

    // Number of columns of sprite sheet
    public int Columns
    { get { return sprites?.GetLength(0) ?? 0; } set { } }

    // Number of rows of sprite sheet
    public int Rows
    { get { return sprites?.GetLength(1) ?? 0; } set { } }

    // Texture setter to index sprites in the array
    protected internal Texture2D Texture
    {
        set
        {
            // 1D Array of all sprites for the sprite sheet
            Object[] sprites1d =
                AssetDatabase.LoadAllAssetRepresentationsAtPath(
                    AssetDatabase.GetAssetPath(value));

            // Only execute if each tile is the same size and aligned with no 
            // padding
            if (sprites1d == null || sprites1d.Length == 0)
            {
                Debug.Log("No sprites found on \"" + value.name + "\"");
                return;
            }

            Vector2Int tileSize = new Vector2Int(
                (int)((Sprite)sprites1d[0]).rect.width,
                (int)((Sprite)sprites1d[0]).rect.height);

            for (int i = 0; i < sprites1d.Length; i++)
            {
                Rect rect = ((Sprite)sprites1d[i]).rect;
                if (rect.width != tileSize.x || rect.x % rect.width != 0 ||
                    rect.height != tileSize.y || rect.y % rect.height != 0)
                {
                    Debug.Log("\"" + value.name + "\" is not sliced properly " + 
                        "for a sprite sheet. Ensure the texture is sliced into " +
                        "a grid with no offset and no padding.");
                    return;
                }
            }

            // Parallel array of coordinates of a sprite on the sprite sheet
            Vector2Int[] coords = new Vector2Int[sprites1d.Length];

            // Size of sprite sheet
            Vector2Int spriteSheetSize = new Vector2Int(0, 0);

            // Search for sprite coordinates, store, and find maximum
            for (int i = 0; i < sprites1d.Length; i++)
            {
                Rect rect = ((Sprite)sprites1d[i]).rect;
                int col = (int)(rect.x / rect.width);
                int row = (int)((value.height - rect.y - rect.height) / 
                    rect.height);

                coords[i] = new Vector2Int(col, row);

                if (col >= spriteSheetSize.x)
                    spriteSheetSize.x = col + 1;
                if (row >= spriteSheetSize.y)
                    spriteSheetSize.y = row + 1;
            }

            sprites = new Sprite[
                spriteSheetSize.x,
                spriteSheetSize.y];

            // Populate the 2D array
            for (int i = 0; i < sprites1d.Length; i++)
                sprites[coords[i].x, coords[i].y] = (Sprite)sprites1d[i];
            
            texture = value;
        }
        get
        { return texture; }
    }

    // Retrieves the sprite at (column, row) of the sprite sheet
    // Origin at top left
    public Sprite GetSprite(int column, int row)
    {
        if (sprites == null ||
            column < 0 || column >= Columns ||
            row < 0 || row >= Rows)
            return null;

        return sprites[column, row];
    }
}


// Editor script to update sprite sheet
[CustomEditor(typeof(SpriteSheet))]
class SpriteSheetEditor : Editor
{
    private SpriteSheet spriteSheet
    { get { return (target as SpriteSheet); } }

    public override void OnInspectorGUI()
    {
        Texture2D newTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Grid-Sliced Texture", spriteSheet.Texture, 
            typeof(Texture2D), false, null);

        // If texture is a new texture, set the new texture
        if (newTexture != spriteSheet.Texture)
        {
            spriteSheet.Texture = newTexture;
            EditorUtility.SetDirty(spriteSheet);
        }
    }
}


// Class that forces sprite sheets to update when textures are re-imported
public class SpriteSheetUpdater : AssetPostprocessor
{
    public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
    {
        // Find all SpriteSheet assets
        string[] guids = AssetDatabase.FindAssets("t:SpriteSheet");
        foreach (string guid in guids)
        {
            SpriteSheet spriteSheet = AssetDatabase.LoadAssetAtPath(
                AssetDatabase.GUIDToAssetPath(guid), typeof(SpriteSheet)) 
                as SpriteSheet;

            // Reload the sprite sheet texture if it uses this texture
            if (spriteSheet.Texture == texture)
                spriteSheet.Texture = spriteSheet.Texture;
        }
    }
}
