// HeartProgressIndicator.cs
// Adjunta al objeto 3D del corazón
using UnityEngine;

public class HeartProgressIndicator : MonoBehaviour
{
    [Header("─── Escala ──────────────────────────────────────────────")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;

    [Header("─── Color (blanco → rojo) ──────────────────────────────")]
    [SerializeField] private Color colorStart = Color.white;
    [SerializeField] private Color colorEnd = Color.red;

    [Header("─── Suavizado ───────────────────────────────────────────")]
    [SerializeField] private float lerpSpeed = 2f;

    private Renderer _renderer;
    private float _targetProgress = 0f;
    private float _currentProgress = 0f;

    private void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();

        if (_renderer == null)
            Debug.LogError("❌ No se encontró Renderer en el corazón");

        // Comienza en blanco y pequeño
        ApplyProgress(0f);
    }

    private void Update()
    {
        if (DonationManager.Instance == null) return;

        _targetProgress = DonationManager.Instance.Progress;

        // Suaviza la transición
        _currentProgress = Mathf.Lerp(_currentProgress, _targetProgress, Time.deltaTime * lerpSpeed);

        ApplyProgress(_currentProgress);
    }

    private void ApplyProgress(float t)
    {
        // Escala
        float scale = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = Vector3.one * scale;

        // Color
        if (_renderer != null)
            _renderer.material.color = Color.Lerp(colorStart, colorEnd, t);
    }
}