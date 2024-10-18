using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 trackingDirection;
    [SerializeField] private float trackingCurve;



    private void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position
            Vector3 desiredPosition = target.position + trackingDirection;

            float dist = (1.0f + Vector3.Magnitude(transform.position - desiredPosition)) * trackingCurve;

            // Плавно перемещаем камеру к желаемой позиции
            transform.position = Vector3.Lerp(transform.position, desiredPosition, dist * Time.deltaTime);

            // Make the camera look at the target
            transform.LookAt(target);
        }
    }

}
