using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;

    public float walkAnimSpeedFactor;
    public float relativeSpeedFeedback;
    public float traction;
    public float maxSpeed;

    private Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponentInParent<Rigidbody2D>();
        body.drag = 0;
        velocity = new Vector2();
    }

    // Update is called once per frame
    void Update()
    {
        // Set velocity
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        // raw directional input from the player
        Vector2 direction = new Vector2(moveHorizontal, moveVertical).normalized;

        direction += (-velocity * relativeSpeedFeedback / maxSpeed);
        direction.Normalize();

        velocity += direction * traction;

        if (velocity.magnitude > maxSpeed)
            velocity *= maxSpeed / velocity.magnitude;
        if (velocity.magnitude < 1)
            velocity.Set(0, 0);

        body.velocity = velocity;

        // Set animation speed
        foreach (Animator a in GetComponentsInChildren<Animator>())
            a.SetFloat("speed", body.velocity.magnitude * walkAnimSpeedFactor);

        /*
        // Face direction of movement 
        
        if (body.velocity.sqrMagnitude > 0.01)
        {
            float eulerZ = 180f / Mathf.PI * Mathf.Atan2(body.velocity.y, body.velocity.x);
            Quaternion angle = Quaternion.Euler(new Vector3(0, 0, eulerZ));

            transform.localRotation = angle;
        }
        */

        // Face cursor
        Vector3 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursor.z = 0;

        Vector3 delta = cursor - transform.position;

        float p = 180f / Mathf.PI * Mathf.Atan2(delta.y, delta.x);
        Quaternion angle = Quaternion.Euler(new Vector3(0, 0, p));

        transform.GetChild(0).localRotation = angle;

        /*
        if (Input.GetKeyDown("space"))
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("doMeleeAttack");
        */
    }
}
