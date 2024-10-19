using UnityEngine;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    private EnumField _difficultyField;
    private Button _startButton;
    private Toggle _debugToggle;

    private void Start()
    {
        Time.timeScale = 0;

        // Получаем корневой визуальный элемент
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Получаем EnumField для выбора сложности
        _difficultyField = root.Q<EnumField>("DifficultyField");
        _difficultyField.Init(Difficulty.Слабый); // Устанавливаем "Слабый" как значение по умолчанию

        // Получаем кнопку "Начать"
        _startButton = root.Q<Button>("StartButton");

        // Добавляем обработчик события нажатия на кнопку
        _startButton.clicked += OnStartButtonClicked;

        _debugToggle = root.Q<Toggle>("DebugToggle");
    }

    private void OnStartButtonClicked()
    {
        // Получаем текущее значение сложности
        Difficulty selectedDifficulty = (Difficulty)_difficultyField.value;
        Debug.Log($"Выбрана сложность: {selectedDifficulty}");

        // Получаем текущее значение Toggle
        bool showCarModel = _debugToggle.value;
        Debug.Log($"Показать модель машины: {showCarModel}");

        // Переключаем модели у всех машин в сцене
        SwitchCarModels(showCarModel);

        // Здесь можно добавить логику для начала игры с выбранной сложностью
        StartGame(selectedDifficulty);
    }

    private void SwitchCarModels(bool showCarModel)
    {
        // Находим все объекты с компонентом Car
        var cars = FindObjectsByType<Car>(FindObjectsSortMode.None);

        foreach (var car in cars)
        {
            // Переключаем видимость моделей
            car.SetModelVisibility(showCarModel);
        }
    }

    private void StartGame(Difficulty difficulty)
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);

        // Логика старта игры в зависимости от сложности
        var carsAi = FindObjectsByType<CarAI>(FindObjectsSortMode.None);

        switch (difficulty)
        {
            case Difficulty.Слабый:
                Debug.Log("Игра началась на Слабом уровне.");
                break;
            case Difficulty.Средний:
                Debug.Log("Игра началась на Среднем уровне.");
                break;
            case Difficulty.Сильный:
                Debug.Log("Игра началась на Сильном уровне.");
                break;
        }

        foreach (var carAi in carsAi)
        {
            // Переключаем видимость моделей
            carAi.SetDifficulty((int)difficulty);
        }
    }
}
