using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class SatisfactionManager : Singleton<SatisfactionManager>
{
    [Header("UI References")]
    [SerializeField] private Slider overallSatisfactionSlider;
    
    [Header("Events")]
    public static Action<bool> OnOverallSatisfactionChanged; // 1 for increase, -1 for decrease

    [Header("Runtime")]
    private int satisfactionLevel = 0;
    private float currentSatisfaction = 100;
    private float maxSatisfactionForLevel;

    private void Start()
    {
        CalculateMaxSatisfactionForLevel();
    }

    private void CalculateMaxSatisfactionForLevel()
    {
        maxSatisfactionForLevel = 75f + Mathf.Pow(satisfactionLevel, 1.25f) * 20f;
    }

    public void IncreaseOverallSatisfaction(float increment)
    {
        currentSatisfaction += increment;
        if (currentSatisfaction > maxSatisfactionForLevel)
        {
            satisfactionLevel++;
            OnOverallSatisfactionChanged?.Invoke(true);
            CalculateMaxSatisfactionForLevel();
            Debug.Log("Satisfaction level increased");
        }

        UpdateUI();
    }
    public void DecreaseOverallSatisfaction(float decrement)
    {
        currentSatisfaction -= decrement;
        if (currentSatisfaction < 0)
        {
            satisfactionLevel--;
            if (satisfactionLevel < 0)
            {
                Debug.Log("GAME OVER - OR SHOULD IT BE OVER?");
            }
            OnOverallSatisfactionChanged?.Invoke(false);
            CalculateMaxSatisfactionForLevel();
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        overallSatisfactionSlider.value = currentSatisfaction / maxSatisfactionForLevel;  
    }
    
}
