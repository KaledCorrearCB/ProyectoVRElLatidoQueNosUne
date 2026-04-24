// DonationManager.cs
// GameObject vacío en la escena — maneja el total global de donaciones
using UnityEngine;
using UnityEngine.Events;

public class DonationManager : MonoBehaviour
{
    public static DonationManager Instance { get; private set; }

    [Header("─── Objetivo global ────────────────────────────────────")]
    [SerializeField] private int donationGoal = 500000;

    [Header("─── Evento al completar el objetivo ─────────────────────")]
    public UnityEvent OnGoalReached;

    private int _totalDonated = 0;
    public int TotalDonated => _totalDonated;
    public int DonationGoal => donationGoal;
    public float Progress => Mathf.Clamp01((float)_totalDonated / donationGoal);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterDonation(int amount)
    {
        _totalDonated += amount;
        Debug.Log($"[DonationManager] Total: ${_totalDonated:N0} / ${donationGoal:N0} — {Progress * 100f:F1}%");

        if (_totalDonated >= donationGoal)
            OnGoalReached?.Invoke();
    }
}