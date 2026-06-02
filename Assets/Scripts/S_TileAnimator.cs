using System;
using System.Collections;
using UnityEngine;

public class S_TileAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private AnimationCurve _moveCurve;
    [SerializeField] private float _moveDuration = 0.1f;
    [SerializeField] private AnimationCurve _spawnCurve;
    [SerializeField] private float _spawnDuration = 0.15f;
    [SerializeField] private AnimationCurve _mergeCurve;
    [SerializeField] private float _mergeExpandScale = 1.2f;
    [SerializeField] private float _mergeDuration = 0.15f;

    private static readonly AnimationCurve _linearCurve = AnimationCurve.Linear(0, 0, 1, 1);
    private RectTransform _rectTransform;
    private Coroutine _moveCoroutine;
    private Coroutine _spawnCoroutine;
    private Coroutine _mergeCoroutine;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void AnimateMerge()
    {
       _mergeCoroutine = StartCoroutine(MergeCoroutine(_mergeDuration));
    }

    public void AnimateToPosition(Vector2 targetPosition)
    {
        _moveCoroutine = StartCoroutine(MoveToPositionCoroutine(targetPosition, _moveDuration));
    }

    public void AnimateSpawn()
    {
        _spawnCoroutine = StartCoroutine(SpawnCoroutine(_spawnDuration));
    }

    // =================== Merge Animation ===================
    private IEnumerator MergeCoroutine(float duration)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 expandedScale = Vector3.one * _mergeExpandScale;

        AnimationCurve curve = _mergeCurve != null ? _mergeCurve : _linearCurve;

        float halfDuration = duration / 2f;
        float elapsedTime = 0f;

        // Expandir
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            float curveValue = curve.Evaluate(t);
            _rectTransform.localScale = Vector3.Lerp(originalScale, expandedScale, curveValue);
            yield return null;
        }

        // Contraer
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            float curveValue = curve.Evaluate(t);
            _rectTransform.localScale = Vector3.Lerp(expandedScale, originalScale, curveValue);
            yield return null;
        }

        _rectTransform.localScale = originalScale;
    }

    // ====================== Spawn Animation ======================
    private IEnumerator SpawnCoroutine(float duration)
    {
        _rectTransform.localScale = Vector3.zero;
        float elapsedTime = 0f;

        AnimationCurve curve = _spawnCurve != null ? _spawnCurve : _linearCurve;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float curveValue = curve.Evaluate(t);
            _rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, curveValue);
            yield return null;
        }

        _rectTransform.localScale = Vector3.one;
    }

    // ====================== Move Animation ======================
    private IEnumerator MoveToPositionCoroutine(Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = _rectTransform.anchoredPosition;
        float elapsedTime = 0f;

        AnimationCurve curve = _moveCurve != null ? _moveCurve : _linearCurve;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float curveValue = curve.Evaluate(t);
            _rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, curveValue);
            yield return null;
        }

        _rectTransform.anchoredPosition = targetPosition;
    }
}
