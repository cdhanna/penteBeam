using System.Collections.Generic;
using Beamable.Common;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public abstract class PlayerBehaviour : MonoBehaviour, IPlayer
   {
      public GameBehaviour game;

      public PlayerPieceSetRef setRef;

      public int AwardedCaptures { get; set; }
      public int PlayerCode { get; set; }

      public abstract string PlayerType { get; set; }

      public abstract IEnumerable<PlayerMoveProgress> MakeMove(Board board);

      public virtual Piece CreateNewPiece()
      {
         return new Piece
         {
            PlayerCode = PlayerCode
         };
      }

      public Promise<PieceBehaviour> SpawnPiece(SlotBehaviour slotBehaviour, GameBehaviour game)
      {
         return setRef.Resolve().FlatMap(r => r.CreateRandomPiece(slotBehaviour, game.game));
      }

      public virtual void OnCreated(GameBehaviour gameBehaviour)
      {
         game = gameBehaviour;
      }

      public abstract IPlayer Clone();
   }
}