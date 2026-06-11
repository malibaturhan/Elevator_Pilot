using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum EPatienceState
{
    WAITING_ELEVATOR,
    INSIDE_ELEVATOR
}

[RequireComponent(typeof(Passenger))]
public class PassengerPatience : MonoBehaviour
{
    private Passenger passenger;

    [Header("Settings")]
    [SerializeField] private float patienceMultiplier = 1.3f;
    [SerializeField] private float patienceDropByTime = 0.3f;
    private float maxPatience;
    private float currentPatience;

    [Header("Bonus system")]
    [SerializeField] private float patienceBonusMultiplier;
    [SerializeField] private float patienceNormalMultiplier;
    
    [Header("UI References")]
    [SerializeField] private Image patienceSliderImage;

    [Header("UI Settings")]
    [SerializeField] private Color32 goodColor;
    [SerializeField] private Color32 normalColor;
    [SerializeField] private Color32 weakColor;
    [SerializeField] private static float normalStep = 0.3f;
    [SerializeField] private static float goodStep = 0.7f;

    [Header("Runtime")]
    private bool canReducePatience;
    private bool isInitialized;
    private bool patienceOver;
    

    private void OnEnable()
    {
        Passenger.OnPassengerWaitingElevator += PassengerWaitingElevatorCallback;
        Passenger.OnPassengerExitElevator += PassengerExitElevatorCallback;
    }

    private void OnDisable()
    {
        Passenger.OnPassengerWaitingElevator -= PassengerWaitingElevatorCallback;
        Passenger.OnPassengerExitElevator -= PassengerExitElevatorCallback;
    }

    public void Init(Passenger passenger)
    {
        this.passenger = passenger;

        CalculateMaxPatience();

        isInitialized = true;
        UpdateUI();
    }

    private void PassengerWaitingElevatorCallback(Passenger passenger)
    {
        if (this.passenger == passenger)
        {
            canReducePatience = true;
        }
    }
    
    private void PassengerExitElevatorCallback(Passenger passenger, Floor initialFloor, Floor targetFloor, float satisfactionRatio)
    {
        if (this.passenger != passenger) return;

        canReducePatience = false;

        float currentPatienceRatio = currentPatience / maxPatience;

        if (currentPatienceRatio > goodStep)
        {
            float satisfactionIncrement = patienceBonusMultiplier * currentPatienceRatio;
            SatisfactionManager.Instance.IncreaseOverallSatisfaction(satisfactionIncrement);

            Debug.Log("Satisfaction increase: " + satisfactionIncrement + " by " + this.passenger.gameObject.name);
        }
        else if (currentPatienceRatio > normalStep)
        {
            float satisfactionIncrement = patienceNormalMultiplier * currentPatienceRatio;
            SatisfactionManager.Instance.IncreaseOverallSatisfaction(satisfactionIncrement);

            Debug.Log("Satisfaction increase: " + satisfactionIncrement + " by " + this.passenger.gameObject.name);
        }
    }

    private void CalculateMaxPatience()
    {
        int currentFloorCount = passenger.InitialFloorCount;
        int targetFloorCount = passenger.TargetFloorCount;

        int floorDifference = Mathf.Abs(currentFloorCount - targetFloorCount);

        // Debug.Log($"{gameObject.name} Initial: {currentFloorCount}, Target: {targetFloorCount}, Difference: {floorDifference}");

        maxPatience = floorDifference * patienceMultiplier;
        currentPatience = maxPatience;
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        ReducePatience();
        UpdateUI();
    }

    private void ReducePatience()
    {
        if (!canReducePatience)
            return;

        currentPatience -= patienceDropByTime * Time.deltaTime;
        currentPatience = Mathf.Clamp(currentPatience, 0f, maxPatience);

        if (currentPatience <= 0f && !patienceOver)
        {
            patienceOver = true;
            currentPatience = 0f;
            HitOverallSatisfaction();
        }
    }

    private void HitOverallSatisfaction()
    {
        StartCoroutine(ReduceOverallSatisfaction(4, maxPatience *  patienceMultiplier));
    }

    private IEnumerator ReduceOverallSatisfaction(float interval, float reduceAmount)
    {
        while (currentPatience <= 0f)
        {
            Debug.Log("hitting overall satisfaction" +reduceAmount);
            SatisfactionManager.Instance.DecreaseOverallSatisfaction(reduceAmount);
            yield return new WaitForSeconds(interval);
        }
    }

    private void UpdateUI()
    {
        if (patienceSliderImage == null)
            return;

        if (maxPatience <= 0f)
        {
            patienceSliderImage.fillAmount = 0f;
            return;
        }

        float patienceRatio = currentPatience / maxPatience;

        patienceSliderImage.fillAmount = patienceRatio;

        if (patienceRatio > goodStep)
        {
            patienceSliderImage.color = goodColor;
        }
        else if (patienceRatio > normalStep)
        {
            patienceSliderImage.color = normalColor;
        }
        else
        {
            patienceSliderImage.color = weakColor;
        }
    }
    
    public float SatisfactionRatio => currentPatience / maxPatience;

    public static float SatisfactionRatioForGoodSatisfaction => goodStep;
    public static float SatisfactionRatioForNormalSatisfaction => normalStep;
}