using UnityEngine;

[RequireComponent(typeof(Car))]
public class CarAI : MonoBehaviour
{
    [SerializeField] private int _rayCount = 15; // ���������� ����������� �����
    [SerializeField] private float _angleStep = 180f; // ���� ������
    [SerializeField] private float _rayDistance = 100f; // ��������� ����������� �����
    [SerializeField] private float _avoidWeight = 0.25f; // ��� ����������� ��� ��������� �����������
    [SerializeField] private float _heightRay = 0f; // ������ ������ ��������� ���
    [SerializeField] private float _overtakeDesire = 0.5f; // ������� ��������

    [SerializeField] private LayerMask _obstacleLayer = 1; // ���� ����������� ��� �������� ����������� 1 - Default
    [SerializeField] private bool _visualizeRays = true; // ��������������� �� ����

    private float _accelerate = 0f; // ������� ���������

    private Car _car; // ������ �������

    private void Start()
    {
        _car = GetComponent<Car>();
    }

    private void Move(float moveInput, float turnInput)
    {
        // _car.direction.z �������� �� �������� ������/�����
        // � _car.direction.x �������� �� �������
        _car.Direction.z = moveInput;
        _car.Direction.x = turnInput;
    }

    private Vector3 CalculateAvoidance()
    {
        _accelerate *= 0f;

        Vector3 averageDirection = Vector3.zero; // ��� ���������� ���� ��������

        // �������� ����, ����� ������������ ���� ����������� �� -45 �� 45 ��������
        float halfAngle = _angleStep / 2f;
        // ��� ���� ����� ������
        float angle = _angleStep / (_rayCount - 1);

        for (int i = 0; i < _rayCount; i++)
        {
            // ��������� ���� ��� �������� ����
            float currentAngle = -halfAngle + (i * angle);
            // ����������� ���� � ����������� � ������� ������������
            Vector3 rayDirection = _car.TargetRotation * (Quaternion.Euler(0, currentAngle, 0) * Vector3.forward);

            // ��������� ���
            Ray ray = new Ray(_car.transform.position + _car.transform.up * _heightRay, rayDirection);
            RaycastHit hit;

            // ��������� ������������ ���� � ������������
            if (Physics.Raycast(ray, out hit, _rayDistance, _obstacleLayer))
            {
                // ���� ��� ���������� � ������������, ��������� ����� �������
                float distanceFactor = hit.distance / _rayDistance;
                PriorityObject priorityObject;
                float value = hit.transform.gameObject.TryGetComponent<PriorityObject>(out priorityObject) ? _avoidWeight + priorityObject.Priority : _avoidWeight;
                Vector3 avoidanceDirection = rayDirection * distanceFactor * value;

                // ���������� ����� �������
                averageDirection += avoidanceDirection;

                // ������������ ���� � ������, ��������� �� ����������
                if (_visualizeRays)
                {
                    Debug.DrawRay(_car.transform.position, rayDirection * hit.distance, Color.Lerp(Color.red, Color.green, distanceFactor));
                }
                if (Random.Range(0f, 1f) < _overtakeDesire) _accelerate += hit.transform.gameObject.TryGetComponent<Car>(out var aponent) ? Random.Range(1f, 10f) * distanceFactor : 0f;
            }
            else
            {
                // ���� ����������� ���, ��� ���� �� ������������ ����������
                averageDirection += rayDirection;

                // ������������ ���� � ������� ������ (��� �����������)
                if (_visualizeRays)
                {
                    Debug.DrawRay(_car.transform.position, rayDirection * _rayDistance, Color.green);
                }
            }
        }
        averageDirection /= _rayCount;
        _accelerate /= _rayCount;

        averageDirection.Normalize();

        Debug.DrawRay(_car.transform.position, averageDirection * 5, Color.blue);
        // ���������� ��������������� ������� �����������
        return averageDirection;
    }

    private void Update()
    {
        // ��������� ����������� ��� ��������� �����������
        Vector3 avoidanceDirection = CalculateAvoidance();

        // ��������, ����� �������� ��� ����������� � ������� ��������
        float turnInput = Vector3.SignedAngle(_car.transform.forward, avoidanceDirection, Vector3.up); // ������� � ����������� �� �����������

        Move(Random.Range(0.9f, 1f) + _accelerate, Mathf.Clamp(turnInput, -1f, 1f));
    }

    public void SetDifficulty(int difficult)
    {
        switch (difficult)
        {
            case 0:
                _rayCount = 9;
                _overtakeDesire = 0.25f;
                break;
            case 1:
                _rayCount = 15;
                _overtakeDesire = 0.5f;
                break;
            case 2:
                _rayCount = 25;
                _overtakeDesire = 0.75f;
                break;
        }
    }
}
