using UnityEngine;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    private EnumField difficultyField;
    private Button startButton;

    // Определим уровни сложности
    private enum Difficulty
    {
        Слабый,
        Средний,
        Сильный
    }

    void Start()
    {
        // Получаем корневой визуальный элемент
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Получаем EnumField для выбора сложности
        difficultyField = root.Q<EnumField>("DifficultyField");
        difficultyField.Init(Difficulty.Слабый); // Устанавливаем "Слабый" как значение по умолчанию

        // Получаем кнопку "Начать"
        startButton = root.Q<Button>("StartButton");

        // Добавляем обработчик события нажатия на кнопку
        startButton.clicked += OnStartButtonClicked;
    }

    private void OnStartButtonClicked()
    {
        // Получаем текущее значение сложности
        Difficulty selectedDifficulty = (Difficulty)difficultyField.value;
        Debug.Log($"Выбрана сложность: {selectedDifficulty}");

        // Здесь можно добавить логику для начала игры с выбранной сложностью
        StartGame(selectedDifficulty);
    }

    private void StartGame(Difficulty difficulty)
    {
        // Логика старта игры в зависимости от сложности
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
    }
}
