using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientCameraRotate : MonoBehaviour
{
    const float ROTATION_SPEED = 0.1f;
    void Update()
    {
        Transform parent = gameObject.transform.parent;
        gameObject.transform.LookAt(parent);
        gameObject.transform.RotateAround(parent.position, Vector3.up, ROTATION_SPEED);
    }
}
