// GazeActivatedButton.cs
// Adjunta este script a cualquier botón/objeto interactivo del panel.
// Requisitos del GameObject:
//   1. Layer debe ser "Interactive"
//   2. Debe tener un BoxCollider (para que el reticle lo detecte)
//   3. Opcional: asignar un Image de tipo "Filled" como progressRing en el Inspector

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GazeActivatedButton : MonoBehaviour
{
    [Header("Tiempo de mirada para activar (segundos)")]
    [SerializeField] private float dwellTime = 2f;

    [Header("Opcional: imagen circular de progreso (fillAmount)")]
    [SerializeField] private Image progressRing;

    [Header("Color del botón al mirar")]
    [SerializeField] private Color hoverColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private Color normalColor = Color.white;

    [Header("¿Puede activarse varias veces?")]
    [SerializeField] private bool resetAfterUse = true;
    [SerializeField] private float resetDelay = 1.5f;

    [Header("Evento al activar — conecta desde el Inspector")]
    public UnityEvent OnActivated;

    [Header("Referencia para mantener el panel abierto")]
    [SerializeField] private CharacterGreeter characterGreeter;

    // ── estado interno ──────────────────────────────────────────
    private float _gazeTimer = 0f;
    private bool _isGazed = false;
    private bool _activated = false;
    private Image _bgImage;           // imagen de fondo del botón (tinting)

    // ─────────────────────────────────────────────────────────────
    private void Awake()
    {
        _bgImage = GetComponent<Image>();
    }

    private void Update()
    {
        // Mantiene el panel vivo mientras se mira el botón
        if (_isGazed && characterGreeter != null)
            characterGreeter.NotifyPanelActivity();

        if (!_isGazed || _activated) return;

        _gazeTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(_gazeTimer / dwellTime);

        if (progressRing != null)
            progressRing.fillAmount = progress;

        if (_gazeTimer >= dwellTime)
            Activate();
    }

    // ── llamado por CardboardReticlePointer vía SendMessage ──────
    private void OnPointerEnter()
    {
        if (_activated && !resetAfterUse) return;

        _isGazed = true;
        _gazeTimer = 0f;

        if (_bgImage != null)
            _bgImage.color = hoverColor;
    }

    private void OnPointerExit()
    {
        _isGazed = false;
        _gazeTimer = 0f;

        if (!_activated && _bgImage != null)
            _bgImage.color = normalColor;

        if (progressRing != null)
            progressRing.fillAmount = 0f;
    }
    // ─────────────────────────────────────────────────────────────

    private void Activate()
    {
        _activated = true;
        if (progressRing != null) progressRing.fillAmount = 0f;

        OnActivated?.Invoke();

        if (resetAfterUse)
            Invoke(nameof(ResetButton), resetDelay);
    }

    // Llama esto manualmente si necesitas re-habilitar el botón
    public void ResetButton()
    {
        _isGazed = false;
        _gazeTimer = 0f;
        _activated = false;

        if (_bgImage != null) _bgImage.color = normalColor;
        if (progressRing != null) progressRing.fillAmount = 0f;
    }
}