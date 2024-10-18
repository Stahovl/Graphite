using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : Orientation
{
    [Header("Parameters")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f; // Rotation speed factor
    public Vector3 direction;

    private void Start()
    {
        Gravity = new Vector3(0, -20, 0);
    }

    public override void OnUpdate()
    {
        Vector3 surfaceNormal = targetRotation * Vector3.up;

        // Calculate the projected velocity
        Vector3 vProj = (targetRotation * new Vector3(0,0, direction.z) * moveSpeed) + Vector3.Project(_rb.linearVelocity, surfaceNormal);

        if (isGrounded())
        {
            _rb.linearVelocity = vProj;

            // Rotate the car only when moving forward
            if (direction.z > 0)
            {
                float rotationAmount = direction.x * rotationSpeed * Time.deltaTime;
                transform.Rotate(0, rotationAmount, 0);
            }
        }

    }
}
