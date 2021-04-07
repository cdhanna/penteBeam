using System;
using System.Collections.Generic;
using Beamable.Common;

namespace Pente.Core
{
   public class CpuPlayerBrain : IPlayer
   {
      public int AwardedCaptures { get; set; }
      public int PlayerCode { get; set; }
      public string PlayerType { get; set; } = "cpu";

      public GameManager game;

      public IEnumerable<PlayerMoveProgress> MakeMove(Board board)
      {
         var randomOffset = game.NextRandom(board.slots.Count);
         BeamableLogger.Log($"Random number picked [{randomOffset}]");
         var mm = new Minimax();
         var bestMove = mm.GetBestMove3(game, PlayerCode, 1);
         if (bestMove == null)
         {
            BeamableLogger.LogWarning("failed to create a best move...");
         }
         yield return bestMove;
//         for (var i = 0; i < board.slots.Count; i++)
//         {
//            yield return new PlayerMoveProgress();
//            var n = (i + randomOffset) % board.slots.Count;
//            if (board.slots[n].piece == null)
//            {
//               yield return new PlayerMove(board.slots[n].position, CreateNewPiece());
//            }
//         }
      }

      public Piece CreateNewPiece()
      {
         return new Piece {PlayerCode = PlayerCode};
      }

      public IPlayer Clone()
      {
         return new CpuPlayerBrain
         {
            AwardedCaptures = AwardedCaptures,
            PlayerCode = PlayerCode,
            game = game
         };
      }
   }
}