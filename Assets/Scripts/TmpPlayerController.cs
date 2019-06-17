using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpPlayerController : MonoBehaviour
{
    Rigidbody2D body;

    public int speed;
    public float walkSpeedFactor;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = speed * new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        foreach (Animator a in GetComponentsInChildren<Animator>())
            a.SetFloat("speed", body.velocity.magnitude * walkSpeedFactor);

        /*
        // Face direction of movement 
        
        if (body.velocity.sqrMagnitude > 0.01)
        {
            float p = 180f / Mathf.PI * Mathf.Atan2(body.velocity.y, body.velocity.x);
            Quaternion angle = Quaternion.Euler(new Vector3(0, 0, p));

            transform.localRotation = angle;
        }
        */

        // Face cursor
        Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pz.z = 0;

        Vector3 d = pz - transform.position;

        float p = 180f / Mathf.PI * Mathf.Atan2(d.y, d.x);
        Quaternion angle = Quaternion.Euler(new Vector3(0, 0, p));

        transform.localRotation = angle;

        if (Input.GetKeyDown("space"))
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("doMeleeAttack");
    }
}
