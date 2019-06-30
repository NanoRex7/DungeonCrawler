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
    public EntitySpriteSheet spriteSheet;
    private EntitySpriteSheet prevSpriteSheet;

    // Property that is animated by the animation clip
    [HideInInspector]
    public float animatorInput;

    // Variables to keep track of state
    private int rotationIndex, prevRotationIndex;
    private int animationFrame, prevAnimationFrame;

    private void OnEnable()
    {
        UpdateSprite();
    }

    // Update is called once per frame
    void Update()
    {
        // Detect changes in sprite sheet
        if (spriteSheet != prevSpriteSheet)
            UpdateSprite();

        prevSpriteSheet = spriteSheet;

        // Do not execute if no sprite sheet is set or it has no sprites
        if (spriteSheet == null || !spriteSheet.HasSprites())
            return;

        // Set previous variables
        prevRotationIndex = rotationIndex;
        prevAnimationFrame = animationFrame;
        
        // Set animation frame
        animationFrame = (int)animatorInput;

        // Get angle from parent (model root)
        float angle = transform.parent?.eulerAngles.z ?? 0;

        // Convert angle to fraction
        rotationIndex = Mathf.RoundToInt(angle / 360.0f * spriteSheet.RotationCount);
        rotationIndex %= spriteSheet.RotationCount;

        // Update sprite if it needs changing and if the new sprite is within 
        // index bounds
        if (rotationIndex != prevRotationIndex ||
            animationFrame != prevAnimationFrame)
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
            spriteSheet?.GetSprite(animationFrame, rotationIndex);
    }
}
