using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wrapper for SpriteSheet to make member names more appropriate
[CreateAssetMenu(fileName = "New Entity Sprite Sheet",
    menuName = "Custom Assets/Entity Sprite Sheet")]
public class EntitySpriteSheet : SpriteSheet
{
    public int AnimationFrameCount
    { get { return Columns; } set { } }

    public int RotationCount
    { get { return Rows; } set { } }

    public new Sprite GetSprite(int animationFrame, int rotation)
    {
        return base.GetSprite(animationFrame, rotation);
    }
}