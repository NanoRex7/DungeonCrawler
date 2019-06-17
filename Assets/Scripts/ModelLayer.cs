using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

/*
 * -animation interpolation can be turned off by:
 * select all keyframes
 * right click on one
 * both tangents -> constant
 */
[ExecuteAlways, RequireComponent(typeof(SpriteRenderer))]
public class ModelLayer : MonoBehaviour
{
    // The sprite sheet to be rendered
    public Texture2D spriteSheet;
    private Texture2D prevSpriteSheet;

    // Property that is animated by the animation clip
    [HideInInspector]
    public float animatorInput;

    // 2D array of sprite references (same as sprite sheet)
    private Sprite[,] sprites;

    // Variables to keep track of state
    private int rotationIndex, prevRotationIndex;
    private int animationFrame, prevAnimationFrame;


    private static Dictionary<string, Sprite[,]> spriteDatabase;


    private void OnEnable()
    {
        UpdateSpriteSheet();
        UpdateSprite();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Detect changes in sprite sheet
        if (spriteSheet != prevSpriteSheet)
        {
            UpdateSpriteSheet();
            UpdateSprite();
        }

        prevSpriteSheet = spriteSheet;

        // Do not execute if no sprite sheet is set
        if (spriteSheet == null)
            return;

        // Set previous variables
        prevRotationIndex = rotationIndex;
        prevAnimationFrame = animationFrame;
        
        // Set animation frame
        animationFrame = (int)animatorInput;

        // Get angle from parent (model root)
        float angle = transform.parent?.eulerAngles.z ?? 0;

        // Set the rotation count equal to the number of sprites on the y axis
        int rotationCount = sprites.GetLength(1);

        // Convert angle to fraction
        rotationIndex = Mathf.RoundToInt(angle / 360.0f * rotationCount);
        rotationIndex %= rotationCount;

        // Update sprite if it needs changing and if the new sprite is within 
        // index bounds
        if ((rotationIndex != prevRotationIndex ||
            animationFrame != prevAnimationFrame) &&
            animationFrame < sprites.GetLength(0) &&
            rotationIndex < sprites.GetLength(1))
        {
            UpdateSprite();
        }
    }
    
    void LateUpdate()
    {
        // Lock rotation
        transform.rotation = Quaternion.identity;
    }

    /*
     * Updates the sprite property of the attached sprite renderer
     */
    private void UpdateSprite()
    {
        GetComponent<SpriteRenderer>().sprite =
            sprites?[animationFrame, rotationIndex];
    }

    /*
     * Performs the necessary updates that must occur when the sprite sheet 
     * property is changed
     */
    private void UpdateSpriteSheet()
    {
        // Ensure sprite sheet texture is valid
        if (spriteSheet != null &&
            Regex.Match(spriteSheet.name, @"_\d+T$").Success)
        {
            sprites = GetSprites(spriteSheet);
        }
        else
        {
            spriteSheet = null;
            sprites = null;
        }
    }

    /*
     * Retrieves a 2D array of sprites on a sprite sheet or generates such an
     * array if it does not exist for a given sprite sheet
     */
    private static Sprite[,] GetSprites(Texture2D spriteSheet)
    {
        // Initialize dictionary if not initialized
        if (spriteDatabase == null)
            spriteDatabase = new Dictionary<string, Sprite[,]>();

        // Use path to identify sprites
        string assetPath = AssetDatabase.GetAssetPath(spriteSheet);

        // If sprite has already been indexed, fetch it
        if (spriteDatabase.ContainsKey(assetPath))
        {
            return spriteDatabase[assetPath];
        }

        // Index the sprite by constructing 2D array of sprite references
        else
        {
            // 1D Array of all sprites for the sprite sheet
            Object[] sprites1d =
                AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

            // Parallel array of coordinates of a sprite on the sprite sheet
            Vector2Int[] coords = new Vector2Int[sprites1d.Length];

            // Size of sprite sheet
            Vector2Int spriteSheetSize = new Vector2Int(0, 0);

            // Search for sprite coordinates, store, and find maximum
            for (int i = 0; i < sprites1d.Length; i++)
            {
                Match num = Regex.Match(sprites1d[i].name, @"\d+",
                    RegexOptions.RightToLeft);
                int y = int.Parse(num.Value);
                num = num.NextMatch();
                int x = int.Parse(num.Value);
                coords[i] = new Vector2Int(x, y);

                if (x >= spriteSheetSize.x)
                    spriteSheetSize.x = x + 1;
                if (y >= spriteSheetSize.y)
                    spriteSheetSize.y = y + 1;
            }

            Sprite[,] sprites = new Sprite[
                spriteSheetSize.x, 
                spriteSheetSize.y];

            // Populate the 2D array
            for (int i = 0; i < sprites1d.Length; i++)
                sprites[coords[i].x, coords[i].y] = (Sprite)sprites1d[i];

            // Add to the database
            spriteDatabase.Add(assetPath, sprites);
            
            /*
            Debug.Log("Indexing sprites: " + spriteSheet.name + ", " + 
                spriteSheetSize.x + "x" + spriteSheetSize.y + ", " 
                + sprites.Length + " sprites");
            */

            return sprites;
        }
    }
}
