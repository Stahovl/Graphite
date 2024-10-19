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

        // �������� �������� ���������� �������
        var root = GetComponent<UIDocument>().rootVisualElement;

        // �������� EnumField ��� ������ ���������
        _difficultyField = root.Q<EnumField>("DifficultyField");
        _difficultyField.Init(Difficulty.������); // ������������� "������" ��� �������� �� ���������

        // �������� ������ "������"
        _startButton = root.Q<Button>("StartButton");

        // ��������� ���������� ������� ������� �� ������
        _startButton.clicked += OnStartButtonClicked;

        _debugToggle = root.Q<Toggle>("DebugToggle");
    }

    private void OnStartButtonClicked()
    {
        // �������� ������� �������� ���������
        Difficulty selectedDifficulty = (Difficulty)_difficultyField.value;
        Debug.Log($"������� ���������: {selectedDifficulty}");

        // �������� ������� �������� Toggle
        bool showCarModel = _debugToggle.value;
        Debug.Log($"�������� ������ ������: {showCarModel}");

        // ����������� ������ � ���� ����� � �����
        SwitchCarModels(showCarModel);

        // ����� ����� �������� ������ ��� ������ ���� � ��������� ����������
        StartGame(selectedDifficulty);
    }

    private void SwitchCarModels(bool showCarModel)
    {
        // ������� ��� ������� � ����������� Car
        var cars = FindObjectsByType<Car>(FindObjectsSortMode.None);

        foreach (var car in cars)
        {
            // ����������� ��������� �������
            car.SetModelVisibility(showCarModel);
        }
    }

    private void StartGame(Difficulty difficulty)
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);

        // ������ ������ ���� � ����������� �� ���������
        var carsAi = FindObjectsByType<CarAI>(FindObjectsSortMode.None);

        switch (difficulty)
        {
            case Difficulty.������:
                Debug.Log("���� �������� �� ������ ������.");
                break;
            case Difficulty.�������:
                Debug.Log("���� �������� �� ������� ������.");
                break;
            case Difficulty.�������:
                Debug.Log("���� �������� �� ������� ������.");
                break;
        }

        foreach (var carAi in carsAi)
        {
            // ����������� ��������� �������
            carAi.SetDifficulty((int)difficulty);
        }
    }
}
