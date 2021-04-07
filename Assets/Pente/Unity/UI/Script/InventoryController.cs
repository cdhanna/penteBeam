using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public Canvas MainCanvas;
    public Button TriggerButton;

    public Button CloseButton;
    // Start is called before the first frame update
    void Start()
    {
        TriggerButton.onClick.AddListener(Toggle);
        CloseButton.onClick.AddListener(Close);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Close()
    {
        MainCanvas.gameObject.SetActive(false);
    }
    public void Toggle()
    {
        MainCanvas.gameObject.SetActive(!MainCanvas.gameObject.activeInHierarchy);
    }
}
