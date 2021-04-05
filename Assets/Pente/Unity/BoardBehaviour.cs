using System.Collections.Generic;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class BoardBehaviour : MonoBehaviour
   {
      public int boardSize = 6;

      public SlotBehaviour slotBehaviour;

      public SlotEvent OnSlotSelected;

      public Dictionary<Slot, SlotBehaviour> slotToBehaviour = new Dictionary<Slot, SlotBehaviour>();

      [ReadOnly]
      public Board board;

      public Board CreateBoard()
      {
         board = new Board(boardSize);

         foreach (var slot in board.AllSlots)
         {
            var instance = Instantiate(slotBehaviour, transform);
            instance.transform.localPosition = new Vector3(slot.position.x, 0, slot.position.y);

            instance.slot = slot;
            instance.board = this;

            instance.OnSelect.AddListener(HandleSlotSelection);
            slotToBehaviour[slot] = instance;
            instance.OnCreate();
         }

         return board;
      }

      public void HandleSlotSelection(SlotBehaviour slot)
      {
         OnSlotSelected?.Invoke(slot);
      }

      public SlotBehaviour GetSlotBehaviour(Slot slot)
      {
         return slotToBehaviour[slot];
      }
   }
}