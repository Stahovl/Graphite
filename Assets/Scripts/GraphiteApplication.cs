using Unity.VisualScripting;
using UnityEngine;

public class GraphiteApplication : MonoBehaviour
{
    /// <summary>
    /// Устанавливаем targetFrameRate для снятия лока в 30фпс на мобильных устройствах, теперь фпс зависит 
    /// от герцовки и производительности смартфона
    /// </summary>
    private void Awake()
    {
        Application.targetFrameRate = 1380;
    }
}
