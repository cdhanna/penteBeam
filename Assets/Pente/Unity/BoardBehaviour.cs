using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class BoardBehaviour : MonoBehaviour
   {
      public int boardSize = 6;

      public SlotBehaviour slotBehaviour;

      [ReadOnly]
      public Board board;

      public void Awake()
      {
         CreateBoard();
      }

      public void CreateBoard()
      {
         board = new Board(boardSize);

         foreach (var slot in board.AllSlots)
         {
            var instance = Instantiate(slotBehaviour, transform);
            instance.transform.localPosition = new Vector3(slot.position.x, 0, slot.position.y);

            instance.slot = slot;
            instance.board = board;

            instance.OnCreate();
         }
      }
   }
}