using System;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject upgradeMenu;
    [SerializeField] private GameObject[] allMenus;

    public static Action<bool> OnPauseMenuToggle;
    public static Action<bool> OnUpgradeMenuToggle;

    private void Start()
    {
        CloseAllMenus();
    }

    private void CloseAllMenus()
    {
        if (allMenus.Length > 0)
        {
            foreach (var menu in allMenus)
            {
                menu.SetActive(false);
            }
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.activeSelf)
        {
            OnPauseMenuToggle?.Invoke(false);
            pauseMenu.SetActive(false);
        }
        else
        {
            OnPauseMenuToggle?.Invoke(true);
            pauseMenu.SetActive(true);
        }
    }


    public void ToggleUpgradeMenu()
    {
        if (upgradeMenu.activeSelf)
        {
            OnUpgradeMenuToggle?.Invoke(false);
            upgradeMenu.SetActive(false);
        }
        else
        {
            OnUpgradeMenuToggle?.Invoke(true);
            upgradeMenu.SetActive(true);
        }
    }
    
    public void RequestQuit()
    {
        GameManager.Instance.RequestQuit();
    }
}