using UnityEngine;

[RequireComponent(typeof(Car))]
public class CarAI : MonoBehaviour
{
    [SerializeField] private int _rayCount = 15; // Количество выпускаемых лучей
    [SerializeField] private float _angleStep = 180f; // Угол обзора
    [SerializeField] private float _rayDistance = 100f; // Дистанция выпускаемых лучей
    [SerializeField] private float _avoidWeight = 0.25f; // Вес направления для избежания препятствий
    [SerializeField] private float _heightRay = 0f; // Высота откуда пускается луч
    [SerializeField] private float _overtakeDesire = 0.5f; // Желание обогнать

    [SerializeField] private LayerMask _obstacleLayer = 1; // Слой препятствий для проверки поумолчанию 1 - Default
    [SerializeField] private bool _visualizeRays = true; // Визуализировать ли лучи

    private float _accelerate = 0f; // текущее ускорение

    private Car _car; // Объект машинки

    private void Start()
    {
        _car = GetComponent<Car>();
    }

    private void Move(float moveInput, float turnInput)
    {
        // _car.direction.z отвечает за движение вперед/назад
        // а _car.direction.x отвечает за поворот
        _car.Direction.z = moveInput;
        _car.Direction.x = turnInput;
    }

    private Vector3 CalculateAvoidance()
    {
        _accelerate *= 0f;

        Vector3 averageDirection = Vector3.zero; // Для накопления всех векторов

        // Половина угла, чтобы распределить лучи симметрично от -45 до 45 градусов
        float halfAngle = _angleStep / 2f;
        // Шаг угла между лучами
        float angle = _angleStep / (_rayCount - 1);

        for (int i = 0; i < _rayCount; i++)
        {
            // Вычисляем угол для текущего луча
            float currentAngle = -halfAngle + (i * angle);
            // Преобразуем угол в направление в мировом пространстве
            Vector3 rayDirection = _car.TargetRotation * (Quaternion.Euler(0, currentAngle, 0) * Vector3.forward);

            // Выпускаем луч
            Ray ray = new Ray(_car.transform.position + _car.transform.up * _heightRay, rayDirection);
            RaycastHit hit;

            // Проверяем столкновение луча с препятствием
            if (Physics.Raycast(ray, out hit, _rayDistance, _obstacleLayer))
            {
                // Если луч столкнулся с препятствием, уменьшаем длину вектора
                float distanceFactor = hit.distance / _rayDistance;
                PriorityObject priorityObject;
                float value = hit.transform.gameObject.TryGetComponent<PriorityObject>(out priorityObject) ? _avoidWeight + priorityObject.Priority : _avoidWeight;
                Vector3 avoidanceDirection = rayDirection * distanceFactor * value;

                // Накопление этого вектора
                averageDirection += avoidanceDirection;

                // Визуализация луча с цветом, зависящим от расстояния
                if (_visualizeRays)
                {
                    Debug.DrawRay(_car.transform.position, rayDirection * hit.distance, Color.Lerp(Color.red, Color.green, distanceFactor));
                }
                if (Random.Range(0f, 1f) < _overtakeDesire) _accelerate += hit.transform.gameObject.TryGetComponent<Car>(out var aponent) ? Random.Range(1f, 10f) * distanceFactor : 0f;
            }
            else
            {
                // Если препятствий нет, луч идет на максимальное расстояние
                averageDirection += rayDirection;

                // Визуализация луча с зеленым цветом (без препятствий)
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
        // Возвращаем нормализованное среднее направление
        return averageDirection;
    }

    private void Update()
    {
        // Вычисляем направление для избежания препятствий
        Vector3 avoidanceDirection = CalculateAvoidance();

        // Например, можем передать это направление в функцию движения
        float turnInput = Vector3.SignedAngle(_car.transform.forward, avoidanceDirection, Vector3.up); // Поворот в зависимости от направления

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
