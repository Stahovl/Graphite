using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : Orientation
{
    [Header("Parameters")]
    public float baseMoveSpeed = 30f; // Базовая скорость движения
    public float baseRotationSpeed = 30f; // Базовая скорость поворота
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
            if (direction.z != 0)
            {
                // Умножаем скорость поворота на текущую скорость движения
                float currentRotationSpeed = baseRotationSpeed * Mathf.Abs(direction.z);
                float rotationAmount = direction.x * baseRotationSpeed * Time.deltaTime;
                transform.Rotate(0, rotationAmount, 0);
            }
        }
    }
}
