using System.Collections;
using System.Collections.Generic;
using Beamable;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class BoardBehaviour : MonoBehaviour
   {
      public int boardSize = 6;

      public SlotBehaviour slotBehaviour;

      public SlotEvent OnSlotSelected;

      public Dictionary<Vector2Int, SlotBehaviour> slotToBehaviour = new Dictionary<Vector2Int, SlotBehaviour>();
      public Dictionary<Vector2Int, PieceBehaviour> slotToPiece = new Dictionary<Vector2Int, PieceBehaviour>();

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
            slotToBehaviour[slot.position] = instance;
            instance.OnCreate();
         }

         return board;
      }

      public IEnumerable SpawnPiece(GameBehaviour game, PlayerBehaviour current, PlayerMove move)
      {
         var slot = board.GetSlot(move.position);
         var slotBehaviour = GetSlotBehaviour(slot);
         var promise = current.SpawnPiece(slotBehaviour, game);
         yield return promise.ToYielder();

         slotToPiece[slot.position] = promise.GetResult();
      }

      public IEnumerable DestroyPieces(GameBehaviour game, PlayerBehaviour current, Capture capture)
      {
         var captured1 = slotToPiece[capture.Captured1.position];
         var captured2 = slotToPiece[capture.Captured2.position];

         slotToPiece.Remove(capture.Captured1.position);
         slotToPiece.Remove(capture.Captured2.position);

         Destroy(captured1.gameObject);
         Destroy(captured2.gameObject);

         yield break; // TODO do some cool death animations here
      }

      public void HandleSlotSelection(SlotBehaviour slot)
      {
         OnSlotSelected?.Invoke(slot);
      }

      public SlotBehaviour GetSlotBehaviour(Slot slot)
      {
         return slotToBehaviour[slot.position];
      }
   }
}