using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;

    public int speed;
    public float walkAnimSpeedFactor;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Set velocity
        body.velocity = speed * 
            new Vector2(
                Input.GetAxis("Horizontal"), 
                Input.GetAxis("Vertical"))
                .normalized;

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

        transform.localRotation = angle;

        /*
        if (Input.GetKeyDown("space"))
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("doMeleeAttack");
        */
    }
}
