using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float _duration;
    private float _strength;
    private float _time;

    private Vector3 _originalPos;

    private void Awake()
    {
        _originalPos = transform.localPosition;
    }

    public void Shake(float duration, float strength)
    {
        _duration = duration;
        _strength = strength;
        _time = duration;
    }

    private void LateUpdate()
    {
        if (_time > 0)
        {
            float progress = _time / _duration;

            // затухание (не линейное — иначе выглядит плохо)
            float damper = progress * progress;

            float offsetX = Random.Range(-1f, 1f) * _strength * damper;
            float offsetY = Random.Range(-1f, 1f) * _strength * damper;

            transform.localPosition = _originalPos + new Vector3(offsetX, offsetY, 0);

            _time -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = _originalPos;
        }
    }
}