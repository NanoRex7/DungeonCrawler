using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ModelRoot : MonoBehaviour
{
    // The vertical offset of the model from the parent position
    public int verticalOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Order layers
        for (int i = 0; i < transform.childCount; i++)
        {
            // Get y coordinate (with first two decimals) as integer
            int yInt = (int)((transform.parent?.position.y ?? 0) * 100);

            // Insert negative y coordinate and layer number into one value
            SpriteRenderer spriteRenderer = transform.GetChild(i)
                .GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
                spriteRenderer.sortingOrder = 
                    yInt * -100 + (transform.childCount - i - 1);
        }
    }

    private void LateUpdate()
    {
        // Calculate intended local y coordinate based on vertical offset
        // and parent Z coordinate
        float y = verticalOffset + (transform.parent?.position.z * 0.5f) ?? 0;

        // Ignore any rotation of the parent when applying y coordinate
        transform.localPosition = Quaternion.Inverse(
            transform.parent?.rotation ?? Quaternion.identity) * 
            new Vector2(0, y);
    }
}
