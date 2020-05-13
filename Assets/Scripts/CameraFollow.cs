
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //object the camera will follow
    public Transform target;
    //speed the camera travels
    public float smoothSpeed = 0.125f;
    //offset of the camera
    public Vector3 offset;

    void FixedUpdate()
    {
        //allows a smooth follow of the object
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
