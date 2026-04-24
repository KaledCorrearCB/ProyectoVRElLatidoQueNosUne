// BillboardCanvas.cs
// Ponlo en el GameObject raíz del Canvas.
// El panel siempre rotará para quedar de frente a la cámara.

using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    private Transform _camera;

    private void Start()
    {
        _camera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        // Rota el canvas para mirar hacia la cámara
        transform.LookAt(
            transform.position + _camera.forward,
            _camera.up
        );
    }
}