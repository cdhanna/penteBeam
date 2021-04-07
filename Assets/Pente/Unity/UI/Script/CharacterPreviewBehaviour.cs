using System.Collections;
using System.Collections.Generic;
using Beamable.Api.Payments;
using Beamable.Common.Api.Inventory;
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

    public InventoryObject<PlayerPieceSet> item;

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
        if (listing != null)
        {
            var contentId = listing.ClientData["id"];
            var contentRef = new PlayerPieceSetRef {Id = contentId};
            pieceSet = await contentRef.Resolve();
        }

        if (item != null && pieceSet == null)
        {
            pieceSet = item.ItemContent;
        }

        var sprite = await AddressableSpriteLoader.LoadSprite(pieceSet.icon);
        if (Icon)
        {
            Icon.sprite = sprite;

        }

        if (Button)
        {
            Button.onClick.AddListener(() => { controller.SetFor(listing, item, this); });

        }
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
