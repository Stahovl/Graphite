using UnityEngine;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    private EnumField difficultyField;
    private Button startButton;

    // ��������� ������ ���������
    private enum Difficulty
    {
        ������,
        �������,
        �������
    }

    void Start()
    {
        // �������� �������� ���������� �������
        var root = GetComponent<UIDocument>().rootVisualElement;

        // �������� EnumField ��� ������ ���������
        difficultyField = root.Q<EnumField>("DifficultyField");
        difficultyField.Init(Difficulty.������); // ������������� "������" ��� �������� �� ���������

        // �������� ������ "������"
        startButton = root.Q<Button>("StartButton");

        // ��������� ���������� ������� ������� �� ������
        startButton.clicked += OnStartButtonClicked;
    }

    private void OnStartButtonClicked()
    {
        // �������� ������� �������� ���������
        Difficulty selectedDifficulty = (Difficulty)difficultyField.value;
        Debug.Log($"������� ���������: {selectedDifficulty}");

        // ����� ����� �������� ������ ��� ������ ���� � ��������� ����������
        StartGame(selectedDifficulty);
    }

    private void StartGame(Difficulty difficulty)
    {
        // ������ ������ ���� � ����������� �� ���������
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
    }
}
