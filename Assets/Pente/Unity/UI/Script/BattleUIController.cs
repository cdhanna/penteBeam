using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    public Button quitButton;
    // Start is called before the first frame update
    void Start()
    {
        quitButton.onClick.AddListener(HandleStartQuit);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void HandleStartQuit()
    {
        // TODO: add confirmation page
        var load = SceneManager.LoadSceneAsync("Lobby");
        load.allowSceneActivation = true;
    }
}
