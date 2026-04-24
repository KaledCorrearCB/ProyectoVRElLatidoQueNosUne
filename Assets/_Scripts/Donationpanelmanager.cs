// DonationPanelManager.cs
// Adjunta este script al GameObject raíz del Canvas de donación.
// Conecta todas las referencias desde el Inspector.

using UnityEngine;
using UnityEngine.UI;
using TMPro;          // TextMeshPro — asegúrate de tenerlo en el proyecto

public class DonationPanelManager : MonoBehaviour
{
    // ══════════════════════════════════════════════════════════════
    // PERSONALIZACIÓN — edítala directo en el Inspector de Unity
    // ══════════════════════════════════════════════════════════════
    [Header("─── Personalización del puesto ───────────────────────")]
    [Tooltip("Nombre del puesto que aparece en el mensaje de bienvenida")]
    [SerializeField] private string locationName = "Bogotá Centro";

    // ══════════════════════════════════════════════════════════════
    // REFERENCIAS DE UI — arrastra los objetos desde el Hierarchy
    // ══════════════════════════════════════════════════════════════
    [Header("─── Referencias de UI ─────────────────────────────────")]
    [SerializeField] private TMP_Text welcomeText;    // Texto largo de bienvenida
    [SerializeField] private TMP_Text amountText;     // Muestra el monto actual

    [SerializeField] private Image donateButtonImage;  // Imagen del botón "Donar" (para cambiar a verde)
    [SerializeField] private TMP_Text donateButtonLabel;  // Texto del botón "Donar" (opcional)

    // ══════════════════════════════════════════════════════════════
    // CONFIGURACIÓN DE MONTO
    // ══════════════════════════════════════════════════════════════
    [Header("─── Monto de donación ──────────────────────────────────")]
    [SerializeField] private int startingAmount = 1000;
    [SerializeField] private int stepAmount = 1000;
    [SerializeField] private int minAmount = 1000;
    [SerializeField] private int maxAmount = 500000;

    // ══════════════════════════════════════════════════════════════
    // AUDIO Y COLORES
    // ══════════════════════════════════════════════════════════════
    [Header("─── Audio ───────────────────────────────────────────────")]
    [SerializeField] private AudioSource donationSound;

    [Header("─── Colores del botón Donar ────────────────────────────")]
    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color donatedButtonColor = new Color(0.18f, 0.77f, 0.38f, 1f); // verde

    // ── estado interno ──────────────────────────────────────────
    private int _currentAmount;
    private bool _donated = false;

    // ─────────────────────────────────────────────────────────────
    // Se llama cada vez que el panel se activa (SetActive(true))
    private void OnEnable()
    {
        _donated = false;
        _currentAmount = startingAmount;

        if (donateButtonImage != null)
            donateButtonImage.color = normalButtonColor;

        if (donateButtonLabel != null)
            donateButtonLabel.text = "Donar";

        RefreshUI();
    }

    // ─────────────────────────────────────────────────────────────
    // MÉTODOS PÚBLICOS — conéctalos al evento OnActivated de cada GazeActivatedButton

    /// <summary> Suma 1.000 al monto. Conectar al botón "+" </summary>
    public void IncreaseAmount()
    {
        if (_donated) return;
        _currentAmount = Mathf.Min(_currentAmount + stepAmount, maxAmount);
        RefreshUI();
    }

    /// <summary> Resta 1.000 al monto. Conectar al botón "−" </summary>
    public void DecreaseAmount()
    {
        if (_donated) return;
        _currentAmount = Mathf.Max(_currentAmount - stepAmount, minAmount);
        RefreshUI();
    }

    /// <summary> Registra la donación. Conectar al botón "Donar" </summary>
    public void Donate()
    {
        if (_donated) return;
        _donated = true;

        // Sonido
        if (donationSound != null)
            donationSound.Play();

        // Botón se pone verde
        if (donateButtonImage != null)
            donateButtonImage.color = donatedButtonColor;

        if (donateButtonLabel != null)
            donateButtonLabel.text = "¡Gracias!";

        DonationManager.Instance?.RegisterDonation(_currentAmount);

        Debug.Log($"[Cruz Roja] Donación registrada: ${_currentAmount:N0} COP — Puesto: {locationName}");
    }

    // ─────────────────────────────────────────────────────────────
    private void RefreshUI()
    {
        // Texto de bienvenida con el nombre del puesto interpolado
        if (welcomeText != null)
        {
            welcomeText.text =
                $"Buenos días compañero de la Cruz Roja Colombiana, " +
                $"este es el puesto de <b>{locationName}</b> donde podrás donar " +
                $"para ayudar a más personas y hacer que el corazón de Colombia " +
                $"late aún más fuerte.";
        }

        // Monto formateado en pesos colombianos
        if (amountText != null)
            amountText.text = $"$ {_currentAmount:N0} COP";
    }
}