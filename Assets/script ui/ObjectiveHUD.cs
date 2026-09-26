using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ObjectiveHUD : MonoBehaviour
{
    public static ObjectiveHUD instance;

    [Header("Objective")]
    [SerializeField] private TMP_Text objectiveText;

    [Header("Balance")]
    [SerializeField] private TMP_Text balanceText;

    [Header("Database")]
    public DatabaseObjektif databaseObjektif;

    private int balance = 0;

    private const string BalanceKey = "CTAS_Balance";

    private void Start()
    {
        UpdateBalanceUI();
    }

    // =========================================================
    // OBJECTIVE
    // =========================================================

    public void SetObjective(string objective)
    {
        if (objectiveText != null)
        {
            objectiveText.text = objective;
        }
    }

    // =========================================================
    // BALANCE
    // =========================================================

    public void SetBalance(int amount)
    {
        balance = amount;

        UpdateBalanceUI();
        SaveBalance();
    }

    public void AddMoney(int amount)
    {
        balance += amount;

        UpdateBalanceUI();
        SaveBalance();
    }

    public void RemoveMoney(int amount)
    {
        balance -= amount;

        if (balance < 0)
        {
            balance = 0;
        }

        UpdateBalanceUI();
        SaveBalance();
    }

    public int GetBalance()
    {
        return balance;
    }

    private void UpdateBalanceUI()
    {
        if (balanceText != null)
        {
            balanceText.text = "Rp " + balance.ToString("N0");
        }
    }
    private void SaveBalance()
    {
        PlayerPrefs.SetInt(BalanceKey, balance);
        PlayerPrefs.Save();
    }



    public void UpdateHUD(JenisMotor jenisMotor, string namaMasalah, int indexLangkah)
    {
        if (databaseObjektif == null)
        {
            Debug.LogError("Database Objektif belum dimasukkan ke HUD!");
            return;
        }

        string teksBaru = databaseObjektif.AmbilTeksObjektif(jenisMotor, namaMasalah, indexLangkah);
        objectiveText.text = "- " + teksBaru;
    }
    public void SembunyikanHUD()
    {
        objectiveText.text = "Servis Selesai";
    }
}