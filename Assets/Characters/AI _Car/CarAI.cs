using UnityEngine;
using UnityEngine.UIElements;

public class CarAI : MonoBehaviour
{
    public Car _car; // ������ �� ������ ���������� ��������
    public float angleStep = 180f;
    public float rayDistance = 100f; // ��������� ����������� �����
    public int rayCount = 15; // ���������� ����������� �����
    public LayerMask obstacleLayer; // ���� ����������� ��� ��������
    public float avoidWeight = 0.5f; // ��� ����������� ��� ��������� �����������
    public bool visualizeRays = true; // ��������������� �� ����

    public float HeightRay = 0.25f;
    public float overtakeDesire = 0.5f;
    private float accelerate = 0f;
    private void move(float moveInput, float turnInput)
    {
        // _car.direction.z �������� �� �������� ������/�����
        // � _car.direction.x �������� �� �������
        _car.direction.z = moveInput;
        _car.direction.x = turnInput;
    }

    private Vector3 CalculateAvoidance()
    {
        accelerate *= 0f;

        Vector3 averageDirection = Vector3.zero; // ��� ���������� ���� ��������

        // �������� ����, ����� ������������ ���� ����������� �� -45 �� 45 ��������
        float halfAngle = angleStep / 2f;
        // ��� ���� ����� ������
        float angle = angleStep / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            // ��������� ���� ��� �������� ����
            float currentAngle = -halfAngle + (i * angle);
            // ����������� ���� � ����������� � ������� ������������
            Vector3 rayDirection = _car.targetRotation * (Quaternion.Euler(0, currentAngle, 0) * Vector3.forward);

            // ��������� ���
            Ray ray = new Ray(_car.transform.position + _car.transform.up * HeightRay, rayDirection);
            RaycastHit hit;

            // ��������� ������������ ���� � ������������
            if (Physics.Raycast(ray, out hit, rayDistance, obstacleLayer))
            {
                // ���� ��� ���������� � ������������, ��������� ����� �������
                float distanceFactor = hit.distance / rayDistance;
                PriorityObject priorityObject;
                float value = hit.transform.gameObject.TryGetComponent<PriorityObject>(out priorityObject) ? avoidWeight+priorityObject.priority : avoidWeight;
                Vector3 avoidanceDirection = rayDirection * distanceFactor * value;

                // ���������� ����� �������
                averageDirection += avoidanceDirection;

                // ������������ ���� � ������, ��������� �� ����������
                if (visualizeRays)
                {
                    Debug.DrawRay(_car.transform.position, rayDirection * hit.distance, Color.Lerp(Color.red, Color.green, distanceFactor));
                }
                if(Random.Range(0f, 1f) < overtakeDesire) accelerate += hit.transform.gameObject.TryGetComponent<Car>(out var aponent) ? Random.Range(1f, 10f) * distanceFactor : 0f;
            }
            else
            {
                // ���� ����������� ���, ��� ���� �� ������������ ����������
                averageDirection += rayDirection;

                // ������������ ���� � ������� ������ (��� �����������)
                if (visualizeRays)
                {
                    Debug.DrawRay(_car.transform.position, rayDirection * rayDistance, Color.green);
                }
            }
        }
        averageDirection /= rayCount;
        accelerate /= rayCount;

        averageDirection.Normalize();

        Debug.DrawRay(_car.transform.position, averageDirection*5, Color.blue);
        // ���������� ��������������� ������� �����������
        return averageDirection;
    }

    private void Update()
    {
        // ��������� ����������� ��� ��������� �����������
        Vector3 avoidanceDirection = CalculateAvoidance();

        // ��������, ����� �������� ��� ����������� � ������� ��������
        float turnInput = Vector3.SignedAngle(_car.transform.forward, avoidanceDirection, Vector3.up); // ������� � ����������� �� �����������

        move(Random.Range(0.9f, 1f) + accelerate, Mathf.Clamp(turnInput,-1.5f,1.5f));
    }
}
