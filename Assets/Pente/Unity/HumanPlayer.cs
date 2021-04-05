using System.Collections.Generic;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class HumanPlayer : Player
   {

      public bool isMoveReady = false;
      public Vector2Int movePosition;

      public void SetNextMove(Vector2Int position)
      {
         movePosition = position;
         isMoveReady = true;
      }

      public override IEnumerable<PlayerMoveProgress> MakeMove(Board board)
      {
         isMoveReady = false; // don't allow queued up moves... Always require that someone calls SetNextMove after MakeMove is called.
         while (!isMoveReady)
         {
            yield return new PlayerMoveProgress(); // idle...
         }

         isMoveReady = false; // use up the move...
         yield return new PlayerMove(movePosition, CreateNewPiece());
      }

   }
}