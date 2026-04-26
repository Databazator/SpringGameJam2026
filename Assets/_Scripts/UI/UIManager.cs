using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public bool SkipUI = true;

    public UIScreen TitleScreen;
    public UIScreen TutorialScreen;
    public UIScreen VictoryScreen;
    public UIScreen DefeatScreen;

    public UnityEvent OnPlayAgain;
    public UnityEvent OnStartGame;

    private UIScreen currentScreen;

    List<UIScreen> screens;

    public bool InputEnabled { get; private set; } = true;

    private void Awake()
    {
        screens = new List<UIScreen>{
            TitleScreen,
            TutorialScreen,
            VictoryScreen,
            DefeatScreen,
        };        
    }
    void Start()
    {
        foreach (UIScreen screen in screens)
        {
            screen?.HideScreenImmediate();
        }

        if (!SkipUI)
        {
            // start with opaque title background
            TitleScreen.GetComponent<CanvasGroup>().alpha = 1.0f;
            ShowScreen(TitleScreen);
        }
        else
        {
            currentScreen = null;
            OnStartGame.Invoke();
        }
    }

    void ShowScreen(UIScreen screen)
    {
        if (currentScreen == screen)
            return;
        currentScreen?.HideScreen(0f);
        currentScreen = screen;
        currentScreen?.ShowScreen(0.5f);
    }

    [SerializeField] UnityEvent OnShowVictoryScreen;

    public void ShowVictoryScreen()
    {
        Debug.Log("Showing VICTORY SCREEN!", this);
        InputEnabled = false;
        DOVirtual.DelayedCall(1f, () => InputEnabled = true);
        ShowScreen(VictoryScreen);
        OnShowVictoryScreen?.Invoke();
    }

    public void ShowDefeatScreen()
    {
        ShowScreen(DefeatScreen);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentScreen != null) // null is game/empty screen
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {//Debug.Log($"UIManager on any key down");
                if (InputEnabled)                
                    AnyKeyPressed();
            }            
        }        
    }

    void AnyKeyPressed() // Show next screen
    {
        if(currentScreen == TitleScreen)
        {
            ShowScreen(TutorialScreen);
        }
        else if(currentScreen == TutorialScreen)
        {
            ShowScreen(null);
            OnStartGame.Invoke();
        }
        else if (currentScreen == VictoryScreen)
        {
            currentScreen?.HideScreen();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //OnPlayAgain.Invoke();
        }
        else if (currentScreen == DefeatScreen)
        {
            currentScreen?.HideScreen();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //OnPlayAgain.Invoke();
        }
    }
}
