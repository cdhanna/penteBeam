using System;
using System.Collections.Generic;
using System.Linq;
using Beamable.Common;
using Beamable.Stats;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class HumanPlayerBehaviour : PlayerBehaviour, IPlayer
   {
      private SlotBehaviour _selectedSlot;
      public StatObject characterStat, skinStat;

      public PlayerPieceSetRef DefaultPieceSetRef;

      [ReadOnly]
      public string skinId;

      [ReadOnly]
      public PlayerPieceSet pieceSet;

      public override void OnCreated(GameBehaviour gameBehaviour)
      {
         base.OnCreated(gameBehaviour);
         game.boardBehaviour.OnSlotSelected.AddListener(HandleSlotSelection);

         Beamable.API.Instance.Then(async beamable =>
         {
            var view = await beamable.InventoryService.GetCurrent("items");

            var pieces = await beamable.InventoryService.GetItems<PlayerPieceSet>();

            if (pieces.Count == 0) // grant the first inventory item...
            {
               var defaultContent = await DefaultPieceSetRef.Resolve();

               await beamable.InventoryService.AddItem(DefaultPieceSetRef.Id, defaultContent.GetAllSkinsEnabledProperty());
               // auto select first skin...
               pieces = await beamable.InventoryService.GetItems<PlayerPieceSet>();
               characterStat.Write(defaultContent.Id);
               skinStat.Write(defaultContent.GetAllSkinsEnabledProperty().First().Key);
            }

            var stats = await beamable.User.GetStats(characterStat, skinStat);


            var selectedPieceSet = pieces.First(p => p.ItemContent.Id.Equals(stats[characterStat]));
            var selectedSkinKvp =
               selectedPieceSet.Properties.FirstOrDefault(kvp => kvp.Key.Equals(stats.Get(skinStat)));
            var hasSelectedSkin = selectedSkinKvp.Key != null && Boolean.TryParse(selectedSkinKvp.Value, out var val) &&
                                  val;

            if (!hasSelectedSkin)
            {
               skinId = selectedPieceSet.ItemContent.skins[0].AssetGUID;
            }
            else
            {
               skinId = selectedSkinKvp.Key;
            }

            pieceSet = selectedPieceSet.ItemContent;
         });


      }

      public void HandleSlotSelection(SlotBehaviour slot)
      {
         _selectedSlot = slot;
      }

      public override Promise<PieceBehaviour> SpawnPiece(SlotBehaviour slotBehaviour, GameBehaviour game)
      {
         return pieceSet.CreateRandomPiece(slotBehaviour, game.game, skinId);
      }

      public override string PlayerType { get; set; } = "human";

      public override IEnumerable<PlayerMoveProgress> MakeMove(Board board)
      {
         _selectedSlot = null;
         while (_selectedSlot == null)
         {
            yield return new PlayerMoveProgress();
         }
         yield return new PlayerMove(_selectedSlot.slot.position, CreateNewPiece());
      }

      public override IPlayer Clone()
      {
         return new ProxyPlayer()
         {
            AwardedCaptures = AwardedCaptures,
            PlayerCode = PlayerCode
         };
      }

   }
}