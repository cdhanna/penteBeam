using System.Collections.Generic;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class CpuPlayerBehaviour : PlayerBehaviour, IPlayer
   {

      public CpuPlayerBrain brain = new CpuPlayerBrain();
      public override string PlayerType { get; set; } = "cpu";

      public override IEnumerable<PlayerMoveProgress> MakeMove(Board board)
      {
         brain.game = game.game;
         brain.PlayerCode = PlayerCode;
         brain.AwardedCaptures = AwardedCaptures;

         foreach (var p in brain.MakeMove(board))
         {
            yield return p;
         }
      }

      public override IPlayer Clone()
      {
         return new CpuPlayerBrain()
         {
            AwardedCaptures = AwardedCaptures,
            PlayerCode = PlayerCode
         };
      }
   }
}