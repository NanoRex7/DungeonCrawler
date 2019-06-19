using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Text.RegularExpressions;

public class SpriteSheetProcessor : AssetPostprocessor
{
    // File name without extension
    private string fileName;

    // Regex match for tile slicing indicator at end of file name
    private Match tileIndicator;

    private void OnPreprocessTexture()
    {
        // Set importer settings
        TextureImporter importer = assetImporter as TextureImporter;
        importer.textureType = TextureImporterType.Sprite;

        importer.spriteImportMode = SpriteImportMode.Multiple;

        importer.spritePixelsPerUnit = 1;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        // Search for "_#T" (where # is a number) at end of file name
        fileName = Path.GetFileNameWithoutExtension(assetPath);
        tileIndicator = Regex.Match(fileName, @"_\d+T$");
    }

    public void OnPostprocessTexture(Texture2D texture)
    {
        // Ignore this script if tile slicing indicator was not found
        if (!tileIndicator.Success) return;

        TextureImporter importer = assetImporter as TextureImporter;
        if (importer.spriteImportMode != SpriteImportMode.Multiple)
            return;

        // Parse tile slicing indicator to retrieve tile size value
        int tileSize;
        string tileSizeString = tileIndicator.Value.Substring(1, 
            tileIndicator.Value.Length - 2);
        
        if (!int.TryParse(tileSizeString, out tileSize))
            return;

        Debug.Log("Found tiled texture of size " + 
            texture.width + "x" + texture.height + 
            " with tile size " + tileSize + "x" + tileSize);

        // Slice sprites
        Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(
            texture, Vector2.zero, new Vector2(tileSize, tileSize), Vector2.zero);
  
        List<Rect> rectsList = new List<Rect>(rects);

        // Add sprite metadata
        List<SpriteMetaData> metas = new List<SpriteMetaData>();

        foreach (Rect rect in rectsList)
        {
            SpriteMetaData meta = new SpriteMetaData();
            meta.rect = rect;
            meta.name = fileName.Substring(0, 
                fileName.Length - tileIndicator.Value.Length) + 
                "_" + rect.x / tileSize + 
                //Invert y
                "_" + (texture.height - rect.y - tileSize) / tileSize;
            metas.Add(meta);
        }

        // Save sprite sheet
        importer.spritesheet = metas.ToArray();
        AssetDatabase.Refresh();
    }

    public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
    {
        // Ignore this script if tile slicing indicator was not found
        if (!tileIndicator.Success) return;
        Debug.Log("Sprites created: " + sprites.Length);
    }
}
