using System.Collections.Generic;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class CpuPlayerBehaviour : PlayerBehaviour, IPlayer
   {

      public override IEnumerable<PlayerMoveProgress> MakeMove(Board board)
      {

         // TODO: simulate some random thinking time...

         // find a random slot...

         var randomOffset = game.game.random.Next(board.slots.Count);
         for (var i = 0; i < board.slots.Count; i++)
         {
            yield return new PlayerMoveProgress();
            var n = (i + randomOffset) % board.slots.Count;
            if (board.slots[n].piece == null)
            {
               yield return new PlayerMove(board.slots[n].position, CreateNewPiece());

            }
         }


      }

   }
}