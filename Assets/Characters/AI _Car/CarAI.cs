using UnityEngine;
using UnityEngine.UIElements;

public class CarAI : MonoBehaviour
{
    public Car _car; // Ссылка на скрипт управления машинкой
    public float angleStep = 180f;
    public float rayDistance = 100f; // Дистанция выпускаемых лучей
    public int rayCount = 15; // Количество выпускаемых лучей
    public LayerMask obstacleLayer; // Слой препятствий для проверки
    public float avoidWeight = 0.5f; // Вес направления для избежания препятствий
    public bool visualizeRays = true; // Визуализировать ли лучи

    public float HeightRay = 0.25f;
    public float overtakeDesire = 0.5f;
    private float accelerate = 0f;
    private void move(float moveInput, float turnInput)
    {
        // _car.direction.z отвечает за движение вперед/назад
        // а _car.direction.x отвечает за поворот
        _car.direction.z = moveInput;
        _car.direction.x = turnInput;
    }

    private Vector3 CalculateAvoidance()
    {
        accelerate *= 0f;

        Vector3 averageDirection = Vector3.zero; // Для накопления всех векторов

        // Половина угла, чтобы распределить лучи симметрично от -45 до 45 градусов
        float halfAngle = angleStep / 2f;
        // Шаг угла между лучами
        float angle = angleStep / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            // Вычисляем угол для текущего луча
            float currentAngle = -halfAngle + (i * angle);
            // Преобразуем угол в направление в мировом пространстве
            Vector3 rayDirection = _car.targetRotation * (Quaternion.Euler(0, currentAngle, 0) * Vector3.forward);

            // Выпускаем луч
            Ray ray = new Ray(_car.transform.position + _car.transform.up * HeightRay, rayDirection);
            RaycastHit hit;

            // Проверяем столкновение луча с препятствием
            if (Physics.Raycast(ray, out hit, rayDistance, obstacleLayer))
            {
                // Если луч столкнулся с препятствием, уменьшаем длину вектора
                float distanceFactor = hit.distance / rayDistance;
                PriorityObject priorityObject;
                float value = hit.transform.gameObject.TryGetComponent<PriorityObject>(out priorityObject) ? avoidWeight+priorityObject.priority : avoidWeight;
                Vector3 avoidanceDirection = rayDirection * distanceFactor * value;

                // Накопление этого вектора
                averageDirection += avoidanceDirection;

                // Визуализация луча с цветом, зависящим от расстояния
                if (visualizeRays)
                {
                    Debug.DrawRay(_car.transform.position, rayDirection * hit.distance, Color.Lerp(Color.red, Color.green, distanceFactor));
                }
                if(Random.Range(0f, 1f) < overtakeDesire) accelerate += hit.transform.gameObject.TryGetComponent<Car>(out var aponent) ? Random.Range(1f, 10f) * distanceFactor : 0f;
            }
            else
            {
                // Если препятствий нет, луч идет на максимальное расстояние
                averageDirection += rayDirection;

                // Визуализация луча с зеленым цветом (без препятствий)
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
        // Возвращаем нормализованное среднее направление
        return averageDirection;
    }

    private void Update()
    {
        // Вычисляем направление для избежания препятствий
        Vector3 avoidanceDirection = CalculateAvoidance();

        // Например, можем передать это направление в функцию движения
        float turnInput = Vector3.SignedAngle(_car.transform.forward, avoidanceDirection, Vector3.up); // Поворот в зависимости от направления

        move(Random.Range(0.9f, 1f) + accelerate, Mathf.Clamp(turnInput,-1.5f,1.5f));
    }
}
