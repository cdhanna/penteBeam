using System.Collections.Generic;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class HumanPlayerBehaviour : PlayerBehaviour, IPlayer
   {
      private SlotBehaviour _selectedSlot;

      public override void OnCreated(GameBehaviour gameBehaviour)
      {
         base.OnCreated(gameBehaviour);
         game.boardBehaviour.OnSlotSelected.AddListener(HandleSlotSelection);
      }

      public void HandleSlotSelection(SlotBehaviour slot)
      {
         _selectedSlot = slot;
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