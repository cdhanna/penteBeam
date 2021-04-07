using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbySceneController : MonoBehaviour
{
    public Button startButton;


    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        // transition the scene to the boardDemo scene...
        var load = SceneManager.LoadSceneAsync("BoardDemo");
        startButton.enabled = false;
        load.allowSceneActivation = true;

        // do some sort of loading animation?
    }
}
