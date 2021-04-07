using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pente.Core
{



   public class PlayerMoveProgress : GameProgress
   {

   }

   [Serializable]
   public class PlayerMove : PlayerMoveProgress
   {
      public Vector2Int position;
      public Piece piece;

      public PlayerMove(Vector2Int position, Piece piece)
      {
         this.position = position;
         this.piece = piece;
      }
   }


   public interface IPlayer
   {
      int AwardedCaptures { get; set; }
      int PlayerCode { get; set; }
      string PlayerType { get; set; }
      IEnumerable<PlayerMoveProgress> MakeMove(Board board);
      Piece CreateNewPiece();
      IPlayer Clone();
   }
//   public class Player
//   {
//      public int PlayerCode;
//
//      public virtual IEnumerable<PlayerMoveProgress> MakeMove(Board board)
//      {
//         yield return new PlayerMove(Vector2Int.zero, CreateNewPiece());
//         yield break; // dont do anything.
//      }
//
//      public virtual Piece CreateNewPiece()
//      {
//         return new Piece
//         {
//            PlayerCode = PlayerCode
//         };
//      }
//   }
}