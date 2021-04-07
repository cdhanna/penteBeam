using System.Collections;
using System.Collections.Generic;
using Pente.Core;
using Pente.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    public Button quitButton;
    public Button cancelQuitButton;

    public List<Button> lobbyButton;
    public List<Button> newGame;

    public GameObject warningPopup;
    public GameObject battleUI;
    public GameObject victoryScreen;
    public GameObject failureScreen;

    public bool wasGameOver;
    public GameBehaviour game;
    // Start is called before the first frame update
    void Start()
    {
        quitButton.onClick.AddListener(HandleWarning);
        cancelQuitButton.onClick.AddListener(HandleCancelQuit);
        lobbyButton.ForEach(b => b.onClick.AddListener(HandleStartQuit));
        newGame.ForEach(b => b.onClick.AddListener(HandleNewGame));
    }

    // Update is called once per frame
    void Update()
    {
        if ( (game.winningPlayer != null) && !wasGameOver)
        {
            battleUI.SetActive(false);

            var iswin = game.winningPlayer.PlayerType.Equals("human");
            if (iswin)
            {
                victoryScreen.SetActive(true);
            }
            else
            {
                failureScreen.SetActive(true);
            }

            wasGameOver = true;
        }
    }

    void HandleNewGame()
    {
        var load = SceneManager.LoadSceneAsync("BoardDemo");
        load.allowSceneActivation = true;
    }

    void HandleWarning()
    {
        game.paused = true;
        battleUI.SetActive(false);
        warningPopup.SetActive(true);
    }

    void HandleCancelQuit()
    {
        game.paused = false;
        battleUI.SetActive(true);
        warningPopup.SetActive(false);
    }

    void HandleStartQuit()
    {
        // TODO: add confirmation page
        var load = SceneManager.LoadSceneAsync("Lobby");
        load.allowSceneActivation = true;
    }
}
