// IntroPanelController.cs
// Adjunta este script al GameObject raíz del Canvas introductorio.
// El canvas sigue la cámara constantemente hasta que el jugador
// mira el botón y lo activa con la mirada.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroPanelController : MonoBehaviour
{
    // ══════════════════════════════════════════════════════════════
    // REFERENCIAS
    // ══════════════════════════════════════════════════════════════
    [Header("─── Referencias de escena ──────────────────────────────")]
    [Tooltip("Arrastra aquí el Main Camera del Player")]
    [SerializeField] private Transform cameraTransform;

    [Header("─── Texto introductorio ──────────────────────────────────")]
    [SerializeField] private TMP_Text introText;

    [TextArea(4, 10)]
    [SerializeField]
    private string introMessage =
        "Bienvenido a la experiencia VR de la Cruz Roja Colombiana.\n\n" +
        "En esta experiencia podrás conocer cómo tu donación ayuda a " +
        "miles de personas en todo el país.\n\n" +
        "Mira a tu alrededor y acércate a los voluntarios para comenzar.";

    [Header("─── Botón de inicio ─────────────────────────────────────")]
    [SerializeField] private Image startButtonImage;
    [SerializeField] private TMP_Text startButtonLabel;

    [Header("─── Anillo de progreso del botón (opcional) ─────────────")]
    [SerializeField] private Image progressRing;

    // ══════════════════════════════════════════════════════════════
    // POSICIÓN DEL CANVAS FRENTE A LA CÁMARA
    // ══════════════════════════════════════════════════════════════
    [Header("─── Posición frente a la cámara ────────────────────────")]
    [Tooltip("Distancia en metros a la que flota el panel frente a la cámara")]
    [SerializeField] private float distanceFromCamera = 2f;

    [Tooltip("Desplazamiento vertical (negativo = más abajo)")]
    [SerializeField] private float verticalOffset = 0f;

    [Tooltip("Qué tan rápido sigue la cámara (0 = fijo, 1 = instantáneo)")]
    [Range(0.01f, 1f)]
    [SerializeField] private float followSpeed = 0.08f;

    // ══════════════════════════════════════════════════════════════
    // GAZE / DWELL DEL BOTÓN
    // ══════════════════════════════════════════════════════════════
    [Header("─── Interacción con la mirada ────────────────────────────")]
    [SerializeField] private float dwellTime = 2f;

    [Tooltip("Radio en viewport (0–1) para considerar que la mirada está sobre el botón")]
    [SerializeField] private float gazeViewportRadius = 0.12f;

    [Tooltip("Radio en viewport para que el panel deje de seguir la cámara")]
    [SerializeField] private float panelFreezeViewportRadius = 0.35f;

    [Header("─── Colores del botón ──────────────────────────────────")]
    [SerializeField] private Color normalColor = new Color(0.90f, 0.15f, 0.15f, 1f);
    [SerializeField] private Color hoverColor = new Color(0.70f, 0.10f, 0.10f, 1f);
    [SerializeField] private Color confirmedColor = new Color(0.18f, 0.77f, 0.38f, 1f);

    // ══════════════════════════════════════════════════════════════
    // FADE OUT
    // ══════════════════════════════════════════════════════════════
    [Header("─── Fade al cerrar ─────────────────────────────────────")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.6f;

    // ── estado interno ──────────────────────────────────────────
    private bool _gazeOnButton = false;
    private float _gazeTimer = 0f;
    private bool _closing = false;

    // ─────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main?.transform;

        if (introText != null)
            introText.text = introMessage;

        if (startButtonImage != null)
            startButtonImage.color = normalColor;

        if (startButtonLabel != null)
            startButtonLabel.text = "Comenzar";

        if (progressRing != null)
            progressRing.fillAmount = 0f;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        SnapToCamera();
    }

    private void Update()
    {
        if (_closing) return;

        FollowCamera();
        DetectGaze();
    }

    // ─────────────────────────────────────────────────────────────
    // SEGUIMIENTO DE CÁMARA
    // Se pausa cuando el panel ya está frente al jugador para que
    // el botón quede quieto y sea posible apuntarle con la mirada.
    // ─────────────────────────────────────────────────────────────
    private void FollowCamera()
    {
        if (cameraTransform == null) return;

        // Si el panel ya está centrado en la vista, no lo muevas
        if (IsPanelCenteredInView(panelFreezeViewportRadius)) return;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 targetPos = cameraTransform.position
                             + forward * distanceFromCamera
                             + Vector3.up * verticalOffset;

        Quaternion targetRot = Quaternion.LookRotation(
            transform.position - cameraTransform.position);

        transform.position = Vector3.Lerp(
            transform.position, targetPos, followSpeed);

        transform.rotation = Quaternion.Slerp(
            transform.rotation, targetRot, followSpeed);
    }

    // Posiciona el panel instantáneamente al iniciar (sin Lerp)
    private void SnapToCamera()
    {
        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        transform.position = cameraTransform.position
                           + forward * distanceFromCamera
                           + Vector3.up * verticalOffset;

        transform.rotation = Quaternion.LookRotation(
            transform.position - cameraTransform.position);
    }

    // ─────────────────────────────────────────────────────────────
    // DETECCIÓN DE MIRADA
    // Usa WorldToViewportPoint: no necesita colliders en la UI.
    // ─────────────────────────────────────────────────────────────
    private void DetectGaze()
    {
        if (startButtonImage == null || cameraTransform == null) return;

        Camera cam = cameraTransform.GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        // Posición del botón en viewport
        Vector3 viewportPos = cam.WorldToViewportPoint(
            startButtonImage.transform.position);

        // Botón detrás de la cámara → ignorar
        if (viewportPos.z < 0f)
        {
            ResetGaze();
            return;
        }

        // Distancia desde el centro del viewport (0.5, 0.5)
        float dx = viewportPos.x - 0.5f;
        float dy = viewportPos.y - 0.5f;
        bool hit = Mathf.Sqrt(dx * dx + dy * dy) <= gazeViewportRadius;

        if (hit)
        {
            if (!_gazeOnButton)
            {
                _gazeOnButton = true;
                startButtonImage.color = hoverColor;
            }

            _gazeTimer += Time.deltaTime;

            if (progressRing != null)
                progressRing.fillAmount = Mathf.Clamp01(_gazeTimer / dwellTime);

            if (_gazeTimer >= dwellTime)
                StartClosing();
        }
        else
        {
            ResetGaze();
        }
    }

    // Devuelve true si el centro del panel está dentro del radio dado en viewport
    private bool IsPanelCenteredInView(float radius)
    {
        Camera cam = cameraTransform.GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        if (cam == null) return false;

        Vector3 vp = cam.WorldToViewportPoint(transform.position);
        if (vp.z < 0f) return false;

        float dx = vp.x - 0.5f;
        float dy = vp.y - 0.5f;
        return Mathf.Sqrt(dx * dx + dy * dy) <= radius;
    }

    // Resetea el estado visual y el temporizador del gaze
    private void ResetGaze()
    {
        if (_gazeOnButton)
        {
            _gazeOnButton = false;
            if (startButtonImage != null)
                startButtonImage.color = normalColor;
        }

        _gazeTimer = 0f;

        if (progressRing != null)
            progressRing.fillAmount = 0f;
    }

    // ─────────────────────────────────────────────────────────────
    // CIERRE CON FADE
    // ─────────────────────────────────────────────────────────────
    private void StartClosing()
    {
        if (_closing) return;
        _closing = true;

        if (startButtonImage != null)
            startButtonImage.color = confirmedColor;

        if (startButtonLabel != null)
            startButtonLabel.text = "¡Vamos!";

        if (progressRing != null)
            progressRing.fillAmount = 0f;

        StartCoroutine(FadeAndClose());
    }

    private IEnumerator FadeAndClose()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}