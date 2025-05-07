using UnityEngine;

public class Input : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _speed = 1.5f;

    private void Update()
    {
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);

        // Convert to world-space using head direction
        Vector3 headForward = Camera.main.transform.forward;
        headForward.y = 0f; // flatten
        Vector3 worldMove = Quaternion.LookRotation(headForward) * moveDirection;

        _controller.Move(worldMove * _speed * Time.deltaTime);
    }
}
