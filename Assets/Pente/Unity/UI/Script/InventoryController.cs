using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Beamable;
using Beamable.Api.Payments;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.Common.Shop;
using Beamable.Stats;
using Beamable.UI.Scripts;
using Pente.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public Canvas MainCanvas;
    public Button TriggerButton;

    public Button CloseButton;

    public Preview3dBehaviour preview3d;
    public Transform characterPrefabContainer;
    public CharacterPreviewBehaviour characterPrefab;

    public StoreRef characterStoreRef;

    public Button BuyButton;
    public TextMeshProUGUI PriceText;
    public Image CurrencyIcon;

    public PlayerPieceSetRef defaultPieceRef;
    public StatObject characterStat, skinStat;
    [ReadOnly]
    public CharacterPreviewBehaviour selectedCharacter;
    [ReadOnly]
    public PlayerListingView selectedListing;

    [ReadOnly]
    public InventoryView inventoryView;
    // Start is called before the first frame update
    async void Start()
    {
        TriggerButton.onClick.AddListener(Toggle);
        CloseButton.onClick.AddListener(Close);

        var beamable = await Beamable.API.Instance;
//        beamable.CommerceService.ForceRefresh(characterStoreRef.Id);
//        Debug.Log("getting store data... " + characterStoreRef.Id);
//        var store = await beamable.CommerceService.GetCurrent().Error(ex =>
//        {
//            Debug.LogError(ex);
//        });
//        Debug.Log("Have store data... " + characterStoreRef.Id);
//

//

        characterStat.OnValueChanged += OnCharacterSelectionChanged;

        beamable.InventoryService.Subscribe("currency", CurrencyUpdated);

        beamable.InventoryService.Subscribe(ContentRegistry.GetContentTypeName(typeof(PlayerPieceSet)),
            PieceSetUpdated);
    }

    private void OnCharacterSelectionChanged(StatObjectChangeEvent obj)
    {

    }

    private void PieceSetUpdated(InventoryView obj)
    {
        UpdateItems();
    }

    private void CurrencyUpdated(InventoryView obj)
    {
        inventoryView = obj;
        CheckBuyable();
    }

    async void UpdateItems()
    {
        var beamable = await API.Instance;
        var items = await beamable.InventoryService.GetItems<PlayerPieceSet>();

        await beamable.EnsureDefaultPieces(defaultPieceRef, items, characterStat, skinStat);
        items = await beamable.InventoryService.GetItems<PlayerPieceSet>();
//                var characterStore = await characterStoreRef.Resolve();
        for (var i = 0; i < characterPrefabContainer.childCount; i++)
        {
            Destroy(characterPrefabContainer.GetChild(i).gameObject);
        }

        var instances = new List<CharacterPreviewBehaviour>();
        foreach (var item in items)
        {
            var instance = Instantiate(characterPrefab, characterPrefabContainer);
            instance.listing = null;
            instance.item = item;
            instance.controller = this;
            instances.Add(instance);
            instance.OnCreated();
        }

        var selectedCharacterStat = await beamable.User.GetStat(characterStat);
        var selectedCharacter = instances.First(c => c.item?.ItemContent.Id.Equals(selectedCharacterStat) ?? false);
        SetFor(selectedCharacter.listing, selectedCharacter.item, selectedCharacter);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void HandleBuy()
    {
        if (!IsBuyable()) return;

        var beamable = await Beamable.API.Instance;
        await beamable.CommerceService.Purchase(characterStoreRef.Id, selectedListing.symbol);


    }

    public void Close()
    {
        MainCanvas.gameObject.SetActive(false);
    }
    public void Toggle()
    {
        MainCanvas.gameObject.SetActive(!MainCanvas.gameObject.activeInHierarchy);
    }

    public async void SetFor(PlayerListingView listing, InventoryObject<PlayerPieceSet> item,
        CharacterPreviewBehaviour characterPreviewBehaviour)
    {
        if (selectedCharacter)
        {
            selectedCharacter.Unselect();
        }

        if (item == null)
        {
            BuyButton.gameObject.SetActive(true);
        }
        else
        {
            characterStat.Write(item.ItemContent.Id);
            BuyButton.gameObject.SetActive(false);
        }

        selectedCharacter = characterPreviewBehaviour;
        selectedCharacter.Select();

        selectedListing = listing;

        if (listing != null)
        {
            PriceText.text = listing.offer.price.amount.ToString();
            CheckBuyable();
            if (listing.offer.price.type == "currency")
            {
                var currencyRef = new CurrencyRef(listing.offer.price.symbol);
                var currency = await currencyRef.Resolve();
                CurrencyIcon.sprite = await currency.icon.LoadSprite();
            }
        }

        characterPreviewBehaviour.pieceSet.LoadPromise.Then(gob => { preview3d.SetPreview(gob); });
    }

    void CheckBuyable()
    {
        if (IsBuyable())
        {
            BuyButton.interactable = true;
        }
        else
        {
            BuyButton.interactable = false;
        }
    }

    bool IsBuyable()
    {
        if (selectedListing == null) return false;

        if (selectedListing.offer.price.type == "currency")
        {
            if (!inventoryView.currencies.TryGetValue(selectedListing.offer.price.symbol, out var wallet))
            {
                return false;
            }

            return wallet >= selectedListing.offer.price.amount;
        }

        return false;
    }


}
