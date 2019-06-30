using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Sprite Sheet", 
    menuName = "Custom Assets/Sprite Sheet")]
public class SpriteSheet : ScriptableObject
{
    // Fake 2D array of Sprites by position in sprite sheet
    // Unity does not serialize 2D arrays, so we pretend this is 2D
    // by using [col + row * width] for access
    [SerializeField]
    private Sprite[] sprites;

    // Height and width (in sprite count) of sprite sheet
    [SerializeField]
    private Vector2Int size = new Vector2Int();

    // Number of columns of sprite sheet
    public int Columns
    { get { return size.x; } private set { size.x = value; } }

    // Number of rows of sprite sheet
    public int Rows
    { get { return size.y; } private set { size.y = value;  } }

    // Texture used to generate sprites
    [SerializeField]
    private Texture2D texture;
    public Texture2D Texture
    { get { return texture; } internal set { texture = value; } }

    // Size of one sprite on the texture
    [SerializeField]
    private Vector2Int spriteSize;
    public Vector2Int SpriteSize
    {
        get
        { return spriteSize; }

        internal set
        { spriteSize = new Vector2Int(value.x, value.y); }
    }

    // Returns true if sprite sheet has sprites to render
    public bool HasSprites()
    {
        return sprites != null && sprites.Length > 0;
    }

    // Retrieves the sprite at (column, row) of the sprite sheet
    // Origin at top left
    public Sprite GetSprite(int column, int row)
    {
        if (sprites == null ||
            column < 0 || column >= Columns ||
            row < 0 || row >= Rows)
            return null;

        return sprites[column + row * Columns];
    }

#if UNITY_EDITOR
    // Deletes existing sprites and re-slices them
    internal void RecreateSprites()
    {
        // Ensure data is valid
        if (spriteSize.x <= 0 || spriteSize.y <= 0)
        {
            Debug.Log("Sprite Sheet: Invalid sprite size!");
            return;
        }
        else if (texture == null)
        {
            Debug.Log("Sprite Sheet: Invalid texture!");
            return;
        }

        // Delete all existing sprites
        foreach (Sprite sprite in 
            AssetDatabase.LoadAllAssetRepresentationsAtPath(
                AssetDatabase.GetAssetPath(this)))
            DestroyImmediate(sprite, true);

        // Slice new sprites
        Rect[] rects = UnityEditorInternal.InternalSpriteUtility.
            GenerateGridSpriteRectangles(
            Texture, Vector2.zero, SpriteSize, Vector2.zero);

        Vector2Int[] coords = new Vector2Int[rects.Length];

        for (int i = 0; i < rects.Length; i++)
        {
            Rect rect = rects[i];

            // Find location on sprite sheet
            int col = (int)(rect.x / rect.width);
            int row = (int)((Texture.height - rect.y - rect.height) /
                rect.height);

            coords[i] = new Vector2Int(col, row);

            // Determine maximum coordinates
            if (col + 1 > size.x)
                size.x = col + 1;
            if (row + 1 > size.y)
                size.y = row + 1;
        }

        // Create sprite array
        sprites = new Sprite[Columns * Rows];

        for (int i = 0; i < rects.Length; i++)
        {
            // Create sprite
            Sprite sprite = Sprite.Create(Texture, rects[i],
                new Vector2(0.5f, 0.5f), 1);

            // Set name
            sprite.name = Texture.name + '_' + coords[i].x + '-' + coords[i].y;

            // Add to array
            sprites[coords[i].x + coords[i].y * Columns] = sprite;

            // Add as sub-asset
            AssetDatabase.AddObjectToAsset(sprite, this);
        }
        
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
}

#if UNITY_EDITOR

// Editor script to update sprite sheet
[CustomEditor(typeof(SpriteSheet), true)]
class SpriteSheetEditor : Editor
{
    private SpriteSheet spriteSheet
    { get { return (target as SpriteSheet); } }

    private void OnEnable()
    {
        // Initialize temporary variables to actual values
        spriteSize = spriteSheet.SpriteSize;
        texture = spriteSheet.Texture;
    }

    // Temporary variables while editing
    private Vector2Int spriteSize;
    private Texture2D texture;

    public override void OnInspectorGUI()
    {
        spriteSize = EditorGUILayout.Vector2IntField(
            "Sprite Size", spriteSize);

        texture = (Texture2D)EditorGUILayout.ObjectField(
            "Texture", texture, typeof(Texture2D), false, null);

        if (GUILayout.Button("Apply"))
        {
            // Save values and regenerate sprites
            spriteSheet.SpriteSize = spriteSize;
            spriteSheet.Texture = texture;
            
            spriteSheet.RecreateSprites();
        }
    }
}

/*

// Class that forces sprite sheets to update when textures are re-imported
public class SpriteSheetUpdater : AssetPostprocessor
{
    private static List<SpriteSheet> toUpdate = new List<SpriteSheet>();

    public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
    //public void OnPostprocessTexture(Texture2D texture)
    {
        Debug.Log("PROCESSING " + AssetDatabase.GetAssetPath(texture) + " // " + assetPath);

        // Find all SpriteSheet assets
        string[] guids = AssetDatabase.FindAssets("t:SpriteSheet");
        foreach (string guid in guids)
        {
            SpriteSheet spriteSheet = AssetDatabase.LoadAssetAtPath(
                AssetDatabase.GUIDToAssetPath(guid), typeof(SpriteSheet)) 
                as SpriteSheet;

            // Reload the sprite sheet texture if it uses this texture
            //Debug.Log(spriteSheet.name + (spriteSheet.Texture == null ? " null" : AssetDatabase.GetAssetPath(spriteSheet.Texture)));
            if (spriteSheet.Texture != null && AssetDatabase.GetAssetPath(spriteSheet.Texture) == assetPath)
            //if (spriteSheet.Texture == texture)
            {
                Debug.Log("FOUND SPRITE SHEET WITH MATCHING TEXTURE AT " + assetPath);
                //spriteSheet.Texture = texture;
                toUpdate.Add(spriteSheet);
                //spriteSheet.RecreateSprites(); //it seems to call this with the old texture
            }
        }
    }

    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        Debug.Log("OnPostProcessAllAssets");
        Debug.Log("Found " + toUpdate.Count + " sprite sheets to update");
        
        foreach (SpriteSheet s in toUpdate)
        {
            Debug.Log("ATTEMPING TO RECREATE SPRITES FOR :" + s.name);
            //s.RecreateSprites(); //this crashes the editor
        }

        toUpdate.Clear();
    }
}
*/
    
#endif
