using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Orientation
{
    [Header("��������� ��������")]
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
        
    }

}
