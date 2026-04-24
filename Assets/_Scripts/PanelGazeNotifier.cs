using UnityEngine;

public class PanelGazeNotifier : MonoBehaviour
{
    [SerializeField] private CharacterGreeter characterGreeter;

    private bool _isGazed = false;

    private void Update()
    {
        // Mientras la mira esté dentro del panel, resetea el timer cada frame
        if (_isGazed && characterGreeter != null)
            characterGreeter.NotifyPanelActivity();
    }

    private void OnPointerEnter()
    {
        _isGazed = true;
    }

    private void OnPointerExit()
    {
        _isGazed = false;
    }
}