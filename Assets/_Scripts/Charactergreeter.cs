using System.Collections;
using UnityEngine;

public class CharacterGreeter : MonoBehaviour
{
    [Header("Tiempo mirando al personaje para activar (segundos)")]
    [SerializeField] private float gazeTimeToGreet = 2f;

    [Header("Nombre exacto del Trigger en el Animator")]
    [SerializeField] private string greetTrigger = "Greet";
    [SerializeField] private string idleTrigger = "Idle"; // trigger para volver al idle

    [Header("Panel de donación que aparece tras el saludo")]
    [SerializeField] private GameObject donationPanel;

    [Header("Segundos de espera antes de mostrar el panel")]
    [SerializeField] private float delayBeforePanel = 1.5f;

    [Header("¿Puede saludar de nuevo si el panel se cierra?")]
    [SerializeField] private bool canRepeat = true; // true para que pueda repetir

    [Header("Referencia al Animator del personaje")]
    [SerializeField] private Animator _animator;

    [Header("─── Auto-cierre del panel ───────────────────────────────")]
    [SerializeField] private float panelAutoCloseTime = 10f; // segundos sin interacción

    private bool _isGazed = false;
    private float _gazeTimer = 0f;
    private bool _hasGreeted = false;
    private bool _panelVisible = false;
    private float _panelInactivityTimer = 0f;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        if (_animator == null)
            Debug.LogError("❌ No se encontró Animator — asígnalo en el Inspector");

        if (donationPanel != null)
            donationPanel.SetActive(false);
        else
            Debug.LogError("❌ donationPanel NO está asignado");
    }

    private void Update()
    {
        // ── Lógica de gaze para saludar ──────────────────────────
        if (_isGazed && (!_hasGreeted || canRepeat))
        {
            _gazeTimer += Time.deltaTime;
            if (_gazeTimer >= gazeTimeToGreet)
                Greet();
        }

        // ── Lógica de auto-cierre del panel ──────────────────────
        if (_panelVisible)
        {
            if (_isGazed)
            {
                // El jugador está mirando el panel — resetea el timer
                _panelInactivityTimer = 0f;
            }
            else
            {
                // El jugador no está mirando — cuenta el tiempo
                _panelInactivityTimer += Time.deltaTime;

                if (_panelInactivityTimer >= panelAutoCloseTime)
                    ClosePanel();
            }
        }
    }

    private void OnPointerEnter()
    {
        _isGazed = true;
        if (canRepeat && !_panelVisible) _hasGreeted = false;

        // Si el panel está abierto, resetea el timer de inactividad
        if (_panelVisible)
            _panelInactivityTimer = 0f;
    }

    private void OnPointerExit()
    {
        _isGazed = false;
        if (!_hasGreeted)
            _gazeTimer = 0f;
    }

    private void Greet()
    {
        _hasGreeted = true;
        _gazeTimer = 0f;

        if (_animator != null)
            _animator.SetTrigger(greetTrigger);

        StartCoroutine(ShowPanelDelayed());
    }

    private IEnumerator ShowPanelDelayed()
    {
        yield return new WaitForSeconds(delayBeforePanel);

        if (donationPanel != null)
        {
            donationPanel.SetActive(true);
            _panelVisible = true;
            _panelInactivityTimer = 0f;
            Debug.Log("✅ Panel de donación activado");
        }
    }

    private void ClosePanel()
    {
        Debug.Log("⏱ Panel cerrado por inactividad");

        _panelVisible = false;
        _panelInactivityTimer = 0f;
        _hasGreeted = false;
        _gazeTimer = 0f;

        if (donationPanel != null)
            donationPanel.SetActive(false);

        // Vuelve a la animación idle
        if (_animator != null)
            _animator.SetTrigger(idleTrigger);
    }

    public void NotifyPanelActivity()
    {
        _panelInactivityTimer = 0f;
    }

    public void ResetGreeter()
    {
        ClosePanel();
    }
}