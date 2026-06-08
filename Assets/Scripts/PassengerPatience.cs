using System;
using UnityEngine;
using UnityEngine.UI;

public enum EPatienceState
{
    WAITING_ELEVATOR,
    INSIDE_ELEVATOR
}

public class PassengerPatience : MonoBehaviour
{
    private Passenger passenger;
    
    [Header("Settings")]
    [SerializeField] private float patienceMultiplier = 1.3f;
    [SerializeField] private float patienceDropByTime = 0.3f;
    private float maxPatience;
    private float currentPatience;
    
    [Header("UI References")]
    [SerializeField] private Image patienceSliderImage;

    [Header("UI Settings")]
    [SerializeField] private Color32 goodColor;
    [SerializeField] private Color32 normalColor;
    [SerializeField] private Color32 weakColor;
    [SerializeField] private float normalStep = 0.3f;
    [SerializeField] private float goodStep = 0.7f;
    
    
    void Start()
    {
        passenger = GetComponent<Passenger>();
        CalculateMaxPatience();
    }

    private void CalculateMaxPatience()
    {
        int currentFloorCount = passenger.InitialFloorCount;
        int targetFloorCount = passenger.TargetFloorCount;
        maxPatience = Mathf.Abs(currentFloorCount - targetFloorCount) - patienceMultiplier;
        currentPatience = maxPatience;
    }

    private void Update()
    {
        ReducePatience();
        UpdateUI();
    }

    private void ReducePatience()
    {
        currentPatience-=patienceDropByTime * Time.deltaTime;
    }

    private void UpdateUI()
    {
        float patienceRatio = currentPatience / maxPatience;
        Debug.Log("patienceRatio: " + patienceRatio);
        if (patienceSliderImage != null)
        {
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
    }
}
