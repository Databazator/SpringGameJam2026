using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
            ShowScreen(TitleScreen);
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

    public void ShowVictoryScreen()
    {
        ShowScreen(VictoryScreen);
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
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                Debug.Log($"UIManager on any key down");
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
        else if (currentScreen == TitleScreen)
        {
            currentScreen?.HideScreen();
            OnPlayAgain.Invoke();
        }
        else if (currentScreen == TitleScreen)
        {
            currentScreen?.HideScreen();
            OnPlayAgain.Invoke();
        }
    }
}
