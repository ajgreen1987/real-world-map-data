using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the car's transform
    public Vector3 offset;   // Offset position of the camera in the car's local space
    public float positionSmoothSpeed = 0.125f; // Smoothness of the camera movement
    public float rotationSmoothSpeed = 0.125f; // Smoothness of the camera rotation

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        // Calculate the desired position in the car's local space
        Vector3 desiredPosition = target.TransformPoint(offset);
        // Smoothly move the camera to the desired position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, positionSmoothSpeed);
        transform.position = smoothedPosition;

        // Calculate the target rotation to look at the car
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        // Smoothly rotate the camera to the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed);
    }
}
