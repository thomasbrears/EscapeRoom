using UnityEngine;

public class JoystickSmoothLocomotion : MonoBehaviour
{
    public Transform rigRoot; // The parent of OVRCameraRig
    public Transform head;    // The center eye camera
    public float moveSpeed = 2f;

    void Update()
    {
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 direction = head.forward * input.y + head.right * input.x;
            direction.y = 0;
            direction.Normalize();

            rigRoot.position += direction * moveSpeed * Time.deltaTime;
        }
    }
}
