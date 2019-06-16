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
    public Texture2D spriteSheet; // todo: add setter for this?

    // Property that is animated by the animation clip
    [HideInInspector]
    public float animatorInput;

    // 2D array of sprite references (same as sprite sheet)
    private Sprite[,] sprites;

    // Number of rotations for this sprite (should be = sprites.GetLength(1))
    private int rotationCount;

    // Variables to keep track of state
    private int rotationIndex, prevRotationIndex;
    private int animationFrame, prevAnimationFrame;

    private void OnEnable()
    {
        // Workaround for editor mode
        if (!Application.isPlaying)
            Awake();
    }

    void Awake()
    {
        // Skip if no sprite sheet is attached
        if (spriteSheet == null) return;

        // Skip if already initialized (temp)
        if (sprites != null) return;

        //Assert sprite sheet name is valid _xT
        if (!Regex.Match(spriteSheet.name, @"_\d+T$").Success) return;

        // Construct 2D array of sprite references based on name

        string path = AssetDatabase.GetAssetPath(spriteSheet);

        Object[] sprites1d = 
            AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

        Vector2Int[] coords = new Vector2Int[sprites1d.Length];
        
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

        sprites = new Sprite[spriteSheetSize.x, spriteSheetSize.y];

        // Populate the 2D array
        for (int i = 0; i < sprites1d.Length; i++)
            sprites[coords[i].x, coords[i].y] = (Sprite)sprites1d[i];

        // Set the rotation count equal to the number of sprites on the y axis
        rotationCount = spriteSheetSize.y;

        /*
        Debug.Log("Model initializing: " + spriteSheet.name + ", " + 
            spriteSheetSize.x + "x" + spriteSheetSize.y + ", " 
            + sprites.Length + " sprites");
        */
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
            GetComponent<SpriteRenderer>().sprite =
                sprites[animationFrame, rotationIndex];
        }
    }
    
    void LateUpdate()
    {
        // Lock rotation
        transform.rotation = Quaternion.identity;
    }
}
