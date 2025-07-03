using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float duration = 4f;      // Длительность поворота
    public float targetAngle = 180f;  // Угол поворота (в градусах)

    private Quaternion _startRotation;
    private Quaternion _targetRotation;
    private float _elapsedTime;

    void Start()
    {
        // Запоминаем начальный поворот
        _startRotation = transform.rotation;

        // Рассчитываем целевой поворот (текущий угол + targetAngle)
        _targetRotation = _startRotation * Quaternion.Euler(0f, targetAngle, 0f);
    }

    void Update()
    {
        if (_elapsedTime < duration)
        {
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / duration);

            // Плавный поворот
            transform.rotation = Quaternion.Slerp(
                _startRotation,
                _targetRotation,
                t
            );
        }
    }
}
