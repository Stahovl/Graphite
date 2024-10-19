using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : Orientation
{
    [Header("Parameters")]
    public float baseMoveSpeed = 30f; // Базовая скорость движения
    public float baseRotationSpeed = 1.5f; // Базовая скорость поворота
    public Vector3 direction;

    private void Start()
    {
        Gravity = new Vector3(0, -20, 0);
    }

    public override void OnUpdate()
    {
        Vector3 surfaceNormal = targetRotation * Vector3.up;

        // Вычисляем текущую скорость автомобиля на основе базовой скорости
        float currentMoveSpeed = baseMoveSpeed * direction.z;

        // Проецируем скорость на поверхность
        Vector3 vProj = (targetRotation * new Vector3(0, 0, currentMoveSpeed)) + Vector3.Project(_rb.linearVelocity, surfaceNormal);

        if (isGrounded())
        {
            //_rb.linearVelocity += vProj * (baseMoveSpeed - _rb.linearVelocity.magnitude) * Time.deltaTime;
            _rb.linearVelocity = vProj;

            // Поворачиваем машину, только если она движется вперёд или назад
            float rotationAmount = direction.x * Mathf.Min(_rb.linearVelocity.magnitude, 1f) * baseRotationSpeed;
            transform.Rotate(0, rotationAmount, 0);
        }
    }
}
