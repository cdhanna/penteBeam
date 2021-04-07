using System.Collections;
using System.Collections.Generic;
using Beamable.Api.Payments;
using Beamable.Common.Shop;
using Beamable.UI.Scripts;
using Pente.Unity;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPreviewBehaviour : MonoBehaviour
{
    public string pieceId;
    public PlayerListingView listing;

    public InventoryController controller;
    public Image Icon;

    public Color SelectedColor;
    public Color UnselectedColor;
    public Button Button;

[ReadOnly]
    public PlayerPieceSet pieceSet;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void OnCreated()
    {
        var contentId = listing.ClientData["id"];
        var contentRef = new PlayerPieceSetRef {Id = contentId};

        pieceSet = await contentRef.Resolve();
        var sprite = await AddressableSpriteLoader.LoadSprite(pieceSet.icon);
        Icon.sprite = sprite;

        Button.onClick.AddListener(() => { controller.SetFor(listing, this); });
    }

    public void Unselect()
    {
        Button.targetGraphic.color = UnselectedColor;
    }

    public void Select()
    {
        Button.targetGraphic.color = SelectedColor;

    }
}
