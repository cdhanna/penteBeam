using System.Collections;
using System.Collections.Generic;
using Pente.Unity;
using TMPro;
using UnityEngine;

public class CaptureLabelBehaviour : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public GameBehaviour Game;

    public int PlayerIndex;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Game.players?.Count > PlayerIndex)
        {
            Text.text = Game.players[PlayerIndex].AwardedCaptures.ToString();
        }
    }
}
