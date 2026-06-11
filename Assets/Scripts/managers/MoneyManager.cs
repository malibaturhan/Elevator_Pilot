using System;
using TMPro;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{

    [Header("Runtime")]
    private float currentMoney = 0;

    [Header("Settings")]
    [SerializeField] private float paymentPerFloorPassed = 0.2f;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private void OnEnable()
    {
        Passenger.OnPassengerExitElevator += PassengerDeliveredCallback;
    }
    private void OnDisable()
    {
        Passenger.OnPassengerExitElevator -= PassengerDeliveredCallback;
    }

    private void PassengerDeliveredCallback(Passenger passenger, Floor initialFloor, Floor targetFloor, float satisfactionRatio)
    {
        var floorPassed = Mathf.Abs(initialFloor.FloorNumber - targetFloor.FloorNumber);
        var payment = floorPassed * paymentPerFloorPassed;
        GainMoney(payment);
    }

    public void GainMoney(float amount)
    {
        currentMoney += amount;
        UpdateUI();

    }

    public bool SpendMoney(float amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        if (moneyText == null) return;
         moneyText.text = currentMoney.ToString("F2");
    }
}
