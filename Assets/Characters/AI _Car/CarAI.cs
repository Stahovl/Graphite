using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarAI : Agent
{
    public Car _car; // Ссылка на скрипт управления машинкой
    public float rayDistance = 10f; // Дальность лучей
    public int numRays = 36; // Количество лучей

    private void FixedUpdate()
    {
        // Получаем входные данные
        Vector2[] inputData = DetectObjects();

        // Обрабатываем входные данные и получаем движение
        Vector2 movement = ProcessInputData(inputData);

        // Двигаем машинку
        move(movement.y, movement.x);
    }

    private Vector2[] DetectObjects()
    {
        Vector2[] inputData = new Vector2[numRays];
        float angleStep = 360f / numRays;

        for (int i = 0; i < numRays; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 direction = rotation * transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, rayDistance))
            {
                PriorityObject priorityObject = hit.collider.GetComponent<PriorityObject>();
                if (priorityObject != null)
                {
                    float priority = priorityObject.priority; // -1 или 1
                    float distance = hit.distance;

                    inputData[i] = new Vector2(priority, distance);
                }
                else
                {
                    inputData[i] = new Vector2(0, rayDistance);
                }
            }
            else
            {
                inputData[i] = new Vector2(0, rayDistance);
            }
        }

        return inputData;
    }

    private Vector2 ProcessInputData(Vector2[] inputData)
    {
        Vector3 moveDirection = Vector3.zero;
        float angleStep = 360f / numRays;

        for (int i = 0; i < numRays; i++)
        {
            float priority = inputData[i].x;
            float distance = inputData[i].y;

            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 direction = rotation * Vector3.forward;

            // Вычисляем вес на основе приоритета и расстояния
            float weight = priority / Mathf.Max(distance, 0.1f);

            // Аккумулируем направление движения
            moveDirection += direction * weight;
        }

        // Нормализуем направление движения
        if (moveDirection != Vector3.zero)
            moveDirection.Normalize();

        // Преобразуем мировое направление в локальное
        Vector3 localMoveDirection = transform.InverseTransformDirection(moveDirection);

        // Извлекаем движение вперед и поворот
        float moveInput = localMoveDirection.z; // Движение вперед
        float turnInput = localMoveDirection.x; // Поворот

        return new Vector2(turnInput, moveInput);
    }

    private void move(float moveInput, float turnInput)
    {
        // _car.direction.z отвечает за движение вперед/назад
        // _car.direction.x отвечает за поворот
        _car.direction.z = moveInput;
        _car.direction.x = turnInput;
    }
}
