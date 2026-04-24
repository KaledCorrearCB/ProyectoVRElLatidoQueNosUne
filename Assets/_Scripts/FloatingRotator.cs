// FloatingRotator.cs
// Adjunta a la esfera/corazón para que flote y gire suavemente
using UnityEngine;

public class FloatingRotator : MonoBehaviour
{
    [Header("─── Rotación ────────────────────────────────────────────")]
    [SerializeField] private float rotationSpeed = 30f; // grados por segundo

    [Header("─── Flotación ───────────────────────────────────────────")]
    [SerializeField] private float floatAmplitude = 0.15f; // qué tanto sube y baja
    [SerializeField] private float floatFrequency = 1f;    // qué tan rápido oscila

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        // Rotación continua en Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Flotación senoidal en Y
        float newY = _startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
    }
}