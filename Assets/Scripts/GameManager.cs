using UnityEngine;

public enum EGameState
{
    GAMEPLAY,
    PAUSED,
    MAINMENU
}

public class GameManager : Singleton<GameManager>
{
    private void OnEnable()
    {
        MenuManager.OnPauseMenuToggle += PauseMenuToggleCallback;
        MenuManager.OnUpgradeMenuToggle += UpgradeMenuToggleCallback;
    }

    private void OnDisable()
    {
        MenuManager.OnPauseMenuToggle   -= PauseMenuToggleCallback;
        MenuManager.OnUpgradeMenuToggle -= UpgradeMenuToggleCallback;
    }

    private void PauseMenuToggleCallback(bool isOpen)
    {
        if (isOpen)
        {
            ChangeGameState(EGameState.PAUSED);
        }
        else
        {
            ChangeGameState(EGameState.GAMEPLAY);
        }
    }

    private void UpgradeMenuToggleCallback(bool isOpen)
    {
        if (isOpen)
        {
            ChangeGameState(EGameState.PAUSED);
        }
        else
        {
            ChangeGameState(EGameState.GAMEPLAY);
        }
    }

    private void ChangeGameState(EGameState newState)
    {
        switch (newState)
        {
            case EGameState.PAUSED:
                Time.timeScale = 0f;
                break;

            case EGameState.GAMEPLAY:
                Time.timeScale = 1f;
                break;
        }
    }

    public void RequestQuit()
    {
        // save data if necessary - noop
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}