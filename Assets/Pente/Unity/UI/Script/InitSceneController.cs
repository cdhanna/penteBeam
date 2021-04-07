using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Beamable.API.Instance.Then(_ =>
        {
            var load = SceneManager.LoadSceneAsync("Lobby");
            load.allowSceneActivation = true;
            // TODO: set up loading animation?
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
