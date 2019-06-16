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
            transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = 
                yInt * -100 + (transform.childCount - i - 1);
        }
    }

    private void LateUpdate()
    {
        // Effectively ignore rotation when determining position by rotating
        // the position vector using the negative object rotation
        transform.localPosition = 
            Quaternion.Euler(0, 0, -transform.eulerAngles.z) *
            new Vector2(0, verticalOffset);

        // Fix the local rotation
        transform.localRotation = Quaternion.identity;
    }
}
