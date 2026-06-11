using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EUpgradeType
{
    ELEVATOR_SPEED,
    ELEVATOR_SLOT,
    PASSENGER_PATIENCE,
    PROFIT
}

public class UpgradeManager : Singleton<UpgradeManager>
{
    public static UpgradeManager Instance;

    [Header("Buttons Texts")]
    [SerializeField] private TextMeshProUGUI elevatorSpeedUpgradeButtonText;
    [SerializeField] private TextMeshProUGUI passengerSlotUpgradeButtonText;
    [SerializeField] private TextMeshProUGUI passengerPatienceUpgradeButtonText;
    [SerializeField] private TextMeshProUGUI profitUpgradeButtonText;

    [Header("Runtime Button References")]
    private Button elevatorSpeedUpgradeButton;
    private Button passengerSlotUpgradeButton;
    private Button passengerPatienceUpgradeButton;
    private Button profitUpgradeButton;

    [Header("Money / Test")]
    [SerializeField] private bool ignoreMoneyCostForNow = true;

    [Header("Speed Upgrade")]
    [SerializeField] private float speedUpgradeBasePrice = 25f;
    [SerializeField][Range(0.1f, 1.5f)] private float speedUpgradePriceIncreasePercent = 0.35f;
    [SerializeField] private float speedUpgradeAmount = 0.15f;
    private int speedUpgradeLevel;
    private float lastPriceToUpgradeSpeed;
    private bool speedUpgradePossible = true;

    [Header("Passenger Slot Upgrade")]
    [SerializeField] private float passengerSlotUpgradeBasePrice = 60f;
    [SerializeField][Range(0.1f, 1.5f)] private float passengerSlotUpgradePriceIncreasePercent = 0.55f;
    [SerializeField] private int passengerSlotUpgradeAmount = 1;
    private int slotUpgradeLevel;
    private float lastPriceToUpgradeSlots;
    private bool slotUpgradePossible = true;

    [Header("Passenger Patience Upgrade")]
    [SerializeField] private float patienceUpgradeBasePrice = 40f;
    [SerializeField][Range(0.1f, 1.5f)] private float patienceUpgradePriceIncreasePercent = 0.4f;
    [SerializeField] private float patienceUpgradeAmount = 0.15f;
    private int patienceUpgradeLevel;
    private float lastPriceToUpgradePatience;
    private bool patienceUpgradePossible = true;

    [Header("Profit Upgrade")]
    [SerializeField] private float profitUpgradeBasePrice = 35f;
    [SerializeField][Range(0.1f, 1.5f)] private float profitUpgradePriceIncreasePercent = 0.45f;
    [SerializeField] private float profitUpgradeAmount = 0.2f;
    private int profitUpgradeLevel;
    private float lastPriceToUpgradeProfit;
    private bool profitUpgradePossible = true;

    [Header("Events")]
    public static Action<float> OnElevatorSpeedUpgraded;
    public static Action<int> OnElevatorSlotUpgraded;
    public static Action<float> OnPassengerPatienceUpgraded;
    public static Action<float> OnProfitUpgraded;

    public static Action<EUpgradeType, int, float> OnAnyUpgradePurchased; // type, level, amount

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CacheButtonReferences();
        InitPrices();
        UpdateAllUI();
    }

    private void CacheButtonReferences()
    {
        elevatorSpeedUpgradeButton = GetButtonFromText(elevatorSpeedUpgradeButtonText);
        passengerSlotUpgradeButton = GetButtonFromText(passengerSlotUpgradeButtonText);
        passengerPatienceUpgradeButton = GetButtonFromText(passengerPatienceUpgradeButtonText);
        profitUpgradeButton = GetButtonFromText(profitUpgradeButtonText);
    }

    private Button GetButtonFromText(TextMeshProUGUI text)
    {
        if (text == null)
            return null;

        return text.GetComponentInParent<Button>();
    }

    private void InitPrices()
    {
        lastPriceToUpgradeSpeed = speedUpgradeBasePrice;
        lastPriceToUpgradeSlots = passengerSlotUpgradeBasePrice;
        lastPriceToUpgradePatience = patienceUpgradeBasePrice;
        lastPriceToUpgradeProfit = profitUpgradeBasePrice;
    }

    public void UpgradeElevatorSpeedButtonCallback()
    {
        TryBuyUpgrade(
            EUpgradeType.ELEVATOR_SPEED,
            lastPriceToUpgradeSpeed,
            speedUpgradeAmount,
            ref speedUpgradeLevel,
            ref lastPriceToUpgradeSpeed,
            speedUpgradePriceIncreasePercent
        );
    }

    public void UpgradeElevatorSpaceButtonCallback()
    {
        TryBuyUpgrade(
            EUpgradeType.ELEVATOR_SLOT,
            lastPriceToUpgradeSlots,
            passengerSlotUpgradeAmount,
            ref slotUpgradeLevel,
            ref lastPriceToUpgradeSlots,
            passengerSlotUpgradePriceIncreasePercent
        );
    }

    public void UpgradePassengerPatienceButtonCallback()
    {
        TryBuyUpgrade(
            EUpgradeType.PASSENGER_PATIENCE,
            lastPriceToUpgradePatience,
            patienceUpgradeAmount,
            ref patienceUpgradeLevel,
            ref lastPriceToUpgradePatience,
            patienceUpgradePriceIncreasePercent
        );
    }

    public void UpgradeProfitButtonCallback()
    {
        TryBuyUpgrade(
            EUpgradeType.PROFIT,
            lastPriceToUpgradeProfit,
            profitUpgradeAmount,
            ref profitUpgradeLevel,
            ref lastPriceToUpgradeProfit,
            profitUpgradePriceIncreasePercent
        );
    }

    private void TryBuyUpgrade(
        EUpgradeType upgradeType,
        float currentPrice,
        float upgradeAmount,
        ref int upgradeLevel,
        ref float lastPrice,
        float priceIncreasePercent)
    {
        if (!IsUpgradePossible(upgradeType))
        {
            Debug.Log($"{upgradeType} upgrade is not possible anymore.");
            return;
        }

        if (!TrySpendMoney(currentPrice))
        {
            Debug.Log($"Not enough money for {upgradeType}. Price: {currentPrice}");
            return;
        }

        upgradeLevel++;

        ApplyUpgrade(upgradeType, upgradeAmount);

        OnAnyUpgradePurchased?.Invoke(upgradeType, upgradeLevel, upgradeAmount);

        lastPrice = CalculateNextPrice(lastPrice, priceIncreasePercent);

        UpdateAllUI();

        Debug.Log($"{upgradeType} upgraded. Level: {upgradeLevel}, Next Price: {lastPrice}");
    }

    private bool IsUpgradePossible(EUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case EUpgradeType.ELEVATOR_SPEED:
                return speedUpgradePossible;

            case EUpgradeType.ELEVATOR_SLOT:
                return slotUpgradePossible;

            case EUpgradeType.PASSENGER_PATIENCE:
                return patienceUpgradePossible;

            case EUpgradeType.PROFIT:
                return profitUpgradePossible;

            default:
                return false;
        }
    }

    private void ApplyUpgrade(EUpgradeType upgradeType, float upgradeAmount)
    {
        switch (upgradeType)
        {
            case EUpgradeType.ELEVATOR_SPEED:
                OnElevatorSpeedUpgraded?.Invoke(upgradeAmount);
                break;

            case EUpgradeType.ELEVATOR_SLOT:
                OnElevatorSlotUpgraded?.Invoke(Mathf.RoundToInt(upgradeAmount));
                break;

            case EUpgradeType.PASSENGER_PATIENCE:
                OnPassengerPatienceUpgraded?.Invoke(upgradeAmount);
                break;

            case EUpgradeType.PROFIT:
                OnProfitUpgraded?.Invoke(upgradeAmount);
                break;
        }
    }

    private float CalculateNextPrice(float currentPrice, float increasePercent)
    {
        return Mathf.Ceil(currentPrice + currentPrice * increasePercent);
    }

    private bool TrySpendMoney(float price)
    {
        if (ignoreMoneyCostForNow)
            return true;

        return MoneyManager.Instance.SpendMoney(price);
    }

    private void UpdateAllUI()
    {
        UpdateUpgradeText(
            elevatorSpeedUpgradeButtonText,
            "Speed",
            speedUpgradeLevel,
            lastPriceToUpgradeSpeed,
            speedUpgradePossible
        );

        UpdateUpgradeText(
            passengerSlotUpgradeButtonText,
            "Slot",
            slotUpgradeLevel,
            lastPriceToUpgradeSlots,
            slotUpgradePossible
        );

        UpdateUpgradeText(
            passengerPatienceUpgradeButtonText,
            "Patience",
            patienceUpgradeLevel,
            lastPriceToUpgradePatience,
            patienceUpgradePossible
        );

        UpdateUpgradeText(
            profitUpgradeButtonText,
            "Profit",
            profitUpgradeLevel,
            lastPriceToUpgradeProfit,
            profitUpgradePossible
        );

        UpdateButtonInteractable(elevatorSpeedUpgradeButton, speedUpgradePossible);
        UpdateButtonInteractable(passengerSlotUpgradeButton, slotUpgradePossible);
        UpdateButtonInteractable(passengerPatienceUpgradeButton, patienceUpgradePossible);
        UpdateButtonInteractable(profitUpgradeButton, profitUpgradePossible);
    }

    private void UpdateUpgradeText(TextMeshProUGUI text, string upgradeName, int level, float price, bool isPossible)
    {
        if (text == null)
            return;

        if (!isPossible)
        {
            text.text = $"{upgradeName}\nLv.{level}\nMAX";
            return;
        }

        text.text = $"{upgradeName}\nLv.{level}\n${price:0}";
    }

    private void UpdateButtonInteractable(Button button, bool isPossible)
    {
        if (button == null)
            return;

        button.interactable = isPossible;
    }

    public void DeactivateUpgrade(EUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case EUpgradeType.ELEVATOR_SLOT:
                slotUpgradePossible = false;
                break;

            case EUpgradeType.ELEVATOR_SPEED:
                speedUpgradePossible = false;
                break;

            case EUpgradeType.PASSENGER_PATIENCE:
                patienceUpgradePossible = false;
                break;

            case EUpgradeType.PROFIT:
                profitUpgradePossible = false;
                break;
        }

        UpdateAllUI();
    }

    public int SpeedUpgradeLevel => speedUpgradeLevel;
    public int SlotUpgradeLevel => slotUpgradeLevel;
    public int PatienceUpgradeLevel => patienceUpgradeLevel;
    public int ProfitUpgradeLevel => profitUpgradeLevel;

    public float CurrentSpeedUpgradePrice => lastPriceToUpgradeSpeed;
    public float CurrentSlotUpgradePrice => lastPriceToUpgradeSlots;
    public float CurrentPatienceUpgradePrice => lastPriceToUpgradePatience;
    public float CurrentProfitUpgradePrice => lastPriceToUpgradeProfit;
}