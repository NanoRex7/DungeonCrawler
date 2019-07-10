using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Locks the rotation of a game object
[ExecuteAlways]
public class LockRotation : MonoBehaviour
{
    private void FixedUpdate() { Lock(); }

    private void Update() { Lock(); }

    private void LateUpdate() { Lock(); }

    private void Lock() { transform.rotation = Quaternion.identity; }
}