using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject targetObject;
    public float accel;
    private Vector2 velocity;

    private Vector2 getXY(Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity = accel * getXY(targetObject.transform.position - transform.position);
        transform.position = new Vector3(
            transform.position.x + velocity.x,
            transform.position.y + velocity.y,
            transform.position.z);
    }
}
