using Unity.VisualScripting;
using UnityEngine;

public class GraphiteApplication : MonoBehaviour
{
    /// <summary>
    /// ������������� targetFrameRate ��� ������ ���� � 30��� �� ��������� �����������, ������ ��� ������� 
    /// �� �������� � ������������������ ���������
    /// </summary>
    private void Awake()
    {
        Application.targetFrameRate = 1380;
    }
}
