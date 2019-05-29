using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Vector2 velocity;
    public float relativeSpeedFeedback; // 0.01f in java game
    public float traction; // 0.01f in java game
    public float maxSpeed; // 0.04f in java game

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.drag = 0;
        velocity = new Vector2();
    }

    // Called before 
    void Update() {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        // raw directional input from the player
        Vector2 direction = new Vector2(moveHorizontal, moveVertical).normalized;
        direction += (-velocity * relativeSpeedFeedback / maxSpeed);
        direction.Normalize();

        velocity += direction * traction;

        if (velocity.magnitude > maxSpeed)
            velocity *= maxSpeed / velocity.magnitude;
        if (velocity.magnitude < 1E-4)
            velocity.Set(0, 0);

        rb2d.MovePosition(rb2d.position + velocity);
    }
}
