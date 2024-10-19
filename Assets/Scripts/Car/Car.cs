using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : Orientation
{
    [Header("Parameters")]
    [SerializeField] private float _baseMoveSpeed = 30f; // Базовая скорость движения
    [SerializeField] private float _baseRotationSpeed = 1.5f; // Базовая скорость поворота

    [SerializeField] private GameObject _debugCube;
    [SerializeField] private GameObject _carModel;
    
    public Vector3 Direction;

    private void Start()
    {
        Gravity = new Vector3(0, -20, 0);
    }

    public void SetModelVisibility(bool showCarModel)
    {
        if (_carModel != null && _debugCube != null)
        {
            _carModel.SetActive(showCarModel); // Включить или выключить модель машины
            _debugCube.SetActive(!showCarModel); // Включить или выключить куб
        }
    }

    public override void OnUpdate()
    {
        Vector3 surfaceNormal = TargetRotation * Vector3.up;

        // Вычисляем текущую скорость автомобиля на основе базовой скорости
        float currentMoveSpeed = _baseMoveSpeed * Direction.z;

        // Проецируем скорость на поверхность
        Vector3 vProj = (TargetRotation * new Vector3(0, 0, currentMoveSpeed)) + Vector3.Project(Rb.linearVelocity, surfaceNormal);

        if (IsGrounded())
        {
            //_rb.linearVelocity += vProj * (baseMoveSpeed - _rb.linearVelocity.magnitude) * Time.deltaTime;
            Rb.linearVelocity = vProj;

            // Поворачиваем машину, только если она движется вперёд или назад
            float rotationAmount = Direction.x * Mathf.Min(Rb.linearVelocity.magnitude, 1f) * _baseRotationSpeed;
            transform.Rotate(0, rotationAmount, 0);
        }
    }
}
