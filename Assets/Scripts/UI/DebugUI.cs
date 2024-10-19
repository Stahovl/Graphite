using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DebugUI : MonoBehaviour
{
    private Button _restartButton;
    private Label _fpsLabel;

    private float _deltaTime = 0.0f;

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Находим Label по имени
        _restartButton = root.Q<Button>("RestartButton");
        _fpsLabel = root.Q<Label>("FPSLabel");

        if (_fpsLabel != null)
        {
            Debug.Log($"Найден текстовый элемент: {_fpsLabel.text}");
        }
        else
        {
            Debug.LogWarning("Текстовый fps label элемент не найден!");
        }

        _restartButton.clicked += OnRestartGame;
    }

    private void OnRestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;
        _fpsLabel.text = Mathf.Ceil(fps).ToString() + " FPS";

    }
}