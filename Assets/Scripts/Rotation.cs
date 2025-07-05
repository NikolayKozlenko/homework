using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float duration = 4f;     
    public float targetAngle = 180f;  
    private Quaternion _startRotation;
    private Quaternion _targetRotation;
    private float _elapsedTime;

    void Start()
    {
        _startRotation = transform.rotation;
        _targetRotation = _startRotation * Quaternion.Euler(0f, targetAngle, 0f);
    }

    void Update()
    {
        if (_elapsedTime < duration)
        {
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / duration);

            transform.rotation = Quaternion.Slerp(
                _startRotation,
                _targetRotation,
                t
            );
        }
    }
}
