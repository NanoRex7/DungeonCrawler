using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body;
    private Vector2 velocity;
    private Collision coll;

    public float relativeSpeedFeedback; // 0.01f in java game
    public float traction; // 0.01f in java game
    public float maxSpeed; // 0.04f in java game
    public float minSpeed;
    public float walkAnimSpeedFactor;

    public bool colliding;

    void Start() {
        body = GetComponent<Rigidbody2D>();
        body.drag = 0;
        velocity = new Vector2();
    }

    // Called before 
    void Update()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        // raw directional input from the player
        Vector2 direction = new Vector2(moveHorizontal, moveVertical).normalized;

        direction += (-velocity.normalized * relativeSpeedFeedback);// / maxSpeed);
        direction.Normalize();

        velocity += direction * traction;

        if (velocity.magnitude > maxSpeed)
            velocity *= maxSpeed / velocity.magnitude;
        else if (velocity.magnitude < minSpeed)
            velocity = new Vector2(0, 0);

        body.velocity = velocity;
        //rb2d.MovePosition(rb2d.position + velocity);

        // Set animation speed
        foreach (Animator a in GetComponentsInChildren<Animator>())
            a.SetFloat("speed", body.velocity.magnitude * walkAnimSpeedFactor);

        // Face cursor
        Vector3 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursor.z = 0;

        Vector3 delta = cursor - transform.position;

        float p = 180f / Mathf.PI * Mathf.Atan2(delta.y, delta.x);
        Quaternion angle = Quaternion.Euler(new Vector3(0, 0, p));

        transform.localRotation = angle;
    }
}
