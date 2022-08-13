using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject winScreen;

    public static event Action OnMenuOpened;
    private void OnEnable()
    {
        GameManager.GameStateChanged += DisplayMenu;
    }

    private void OnDisable()
    {
        GameManager.GameStateChanged -= DisplayMenu;
    }

    private void DisplayMenu(GameState state)
    {
        // menuCanvas.SetActive(state == GameState.Lose || state == GameState.Win);
        // loseScreen.SetActive(state == GameState.Lose);
        // // winScreen.SetActive(state == GameState.Win);
        // if (menuCanvas.activeInHierarchy)
        // {
        //     OnMenuOpened?.Invoke();
        // }
    }
}