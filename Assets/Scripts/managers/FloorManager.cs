using System.Collections.Generic;
using UnityEngine;

public class FloorManager : Singleton<FloorManager>
{
    [Header("Settings")]
    [SerializeField] private int numberOfFloorsToGenerate = 4;
    [SerializeField] private int maxNumberOfFloorsWillBeGenerated = 8;
    

    [Header("References")]
    private List<Floor> floors = new List<Floor>();
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private Transform leftBankRoot;
    [SerializeField] private Transform rightBankRoot;

    [Header("Runtime")]
    private float leftNextFloorY;
    private float rightNextFloorY;

    private int bottomFloorIndex = 0;
    private int topFloorIndex = -1;

    protected override void Awake()
    {
        base.Awake();

        leftNextFloorY = leftBankRoot.position.y;
        rightNextFloorY = rightBankRoot.position.y;

        InitFloors();
    }

    private void OnEnable()
    {
        SatisfactionManager.OnOverallSatisfactionChanged += SatisfactionChangedCallback;
    }

    private void OnDisable()
    {
        SatisfactionManager.OnOverallSatisfactionChanged -= SatisfactionChangedCallback;
    }

    private void SatisfactionChangedCallback(bool isIncreased)
    {
        if (isIncreased)
        {
            IncreaseFloorsToUp();
        }
    }

    private void InitFloors()
    {
        floors.Clear();

        topFloorIndex = -1;

        leftNextFloorY = leftBankRoot.position.y;
        rightNextFloorY = rightBankRoot.position.y;

        for (int i = 0; i < numberOfFloorsToGenerate; i++)
        {
            CreateFloorsOfBothSidesAtLevel(i);
        }
    }

    private void CreateFloorsOfBothSidesAtLevel(int floorIndex)
    {
        CreateFloorAt(floorIndex, EFloorSide.RIGHT);
        CreateFloorAt(floorIndex, EFloorSide.LEFT);

        // Bu level oluşturulduktan sonra en üst kat index'ini güncelliyoruz.
        if (floorIndex > topFloorIndex)
        {
            topFloorIndex = floorIndex;
        }
    }

    private void CreateFloorAt(int floorIndex, EFloorSide side)
    {
        Transform parentRoot = side == EFloorSide.LEFT ? leftBankRoot : rightBankRoot;

        GameObject newFloorInstance = Instantiate(floorPrefab, parentRoot);
        Floor newFloor = newFloorInstance.GetComponent<Floor>();

        newFloor.SetInitialValues(floorIndex, side);

        float xPos = parentRoot.position.x;
        float yPos = side == EFloorSide.LEFT ? leftNextFloorY : rightNextFloorY;

        newFloorInstance.transform.position = new Vector2(xPos, yPos);

        floors.Add(newFloor);

        if (side == EFloorSide.LEFT)
        {
            leftNextFloorY += newFloor.Height;
        }
        else
        {
            rightNextFloorY += newFloor.Height;
        }

        // Debug.Log($"Created floor => Index: {floorIndex}, Side: {side}");
    }

    public Floor GetFloorByStoreyAndSide(int floorIndex, EFloorSide side)
    {
        foreach (Floor floor in floors)
        {
            if (floor.FloorNumber == floorIndex && floor.Side == side)
            {
                return floor;
            }
        }

        Debug.LogError($"Floor bulunamadı. FloorIndex: {floorIndex}, Side: {side}");
        return null;
    }

    public int FloorCount => floors.Count;

    public int TopFloorIndex => topFloorIndex;
    public int BottomFloorIndex => bottomFloorIndex;
    
    [NaughtyAttributes.Button]
    public void IncreaseFloorsToUp()
    {
        if (topFloorIndex == maxNumberOfFloorsWillBeGenerated - 1)
        {
            Debug.LogError("max floor count reached " +topFloorIndex);
        }
        CreateFloorsOfBothSidesAtLevel(topFloorIndex + 1);
    }
}