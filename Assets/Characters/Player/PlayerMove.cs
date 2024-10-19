using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Orientation
{
    /*[Header("��������� ��������")]
    public float moveSpeed = 5f;      // �������� ����������� (�/�)
    public float jumpHeight = 4f;     // ������ ������ (�����)


    private void Start()
    {
        Gravity = new Vector3(0, -20, 0);
        RotateInAir = false;
    }

    public override void OnUpdate()
    {
        Vector3 dir = new Vector3();

        if (Input.GetKey(KeyCode.D)) dir.z += 1f;
        if (Input.GetKey(KeyCode.A)) dir.z += -1f;

        Vector3 SurfaceNormal = targetRotation * Vector3.up;

        Vector3 vProj = (targetRotation * dir * moveSpeed) + Vector3.Project(_rb.linearVelocity, SurfaceNormal);

        if (isGrounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float jumpForce = Mathf.Sqrt(jumpHeight * 2f * _gravity_magnitude);
                _rb.AddForce(SurfaceNormal * jumpForce, ForceMode.VelocityChange);
            }
            _rb.linearVelocity = vProj;
        }

        if (dir != Vector3.zero)
        {
            _rb.linearVelocity = vProj;
        }
        
    }*/
    [Header("Parameters")]
    public float baseMoveSpeed = 5f; // Базовая скорость движения
    public float baseRotationSpeed = 100f; // Базовая скорость поворота
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
            _rb.linearVelocity = vProj;

            // Поворачиваем машину, только если она движется вперёд или назад
            if (direction.z != 0)
            {
                // Умножаем скорость поворота на текущую скорость движения
                float currentRotationSpeed = baseRotationSpeed * Mathf.Abs(direction.z);
                float rotationAmount = direction.x * currentRotationSpeed * Time.deltaTime;
                transform.Rotate(0, rotationAmount, 0);
            }
        }
    }

}
