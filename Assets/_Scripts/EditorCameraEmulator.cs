// EditorCameraEmulator.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorCameraEmulator : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2f;

    private float _rotX = 0f;
    private float _rotY = 0f;

    private void Update()
    {
#if UNITY_EDITOR
        // Mantén click derecho + mueve el mouse para rotar
        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();

            _rotY += delta.x * sensitivity * Time.deltaTime * 10f;
            _rotX -= delta.y * sensitivity * Time.deltaTime * 10f;
            _rotX = Mathf.Clamp(_rotX, -80f, 80f);

            transform.localRotation = Quaternion.Euler(_rotX, _rotY, 0f);
        }
#endif
    }
}