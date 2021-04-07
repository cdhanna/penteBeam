using System.Collections.Generic;
using Pente.Core;

namespace Pente.Core
{
   public class ProxyPlayer : IPlayer
   {
      public int AwardedCaptures { get; set; }
      public int PlayerCode { get; set; }

      public string PlayerType { get; set; } = "human";

      public PlayerMove move;

      public IEnumerable<PlayerMoveProgress> MakeMove(Board board)
      {
         yield return move;
      }

      public Piece CreateNewPiece()
      {
         return new Piece {PlayerCode = PlayerCode};
      }
      public IPlayer Clone()
      {
         return new ProxyPlayer()
         {
            AwardedCaptures = AwardedCaptures,
            PlayerCode = PlayerCode
         };
      }
   }
}