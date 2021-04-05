using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Pente.Core
{
   public class GameProgress
   {

   }

   public class PlayerWon : GameProgress
   {

   }

   public class Capture : GameProgress
   {
      public Slot Origin { get; }
      public Slot Dest { get; }
      public Slot Captured1 { get; }
      public Slot Captured2 { get; }

      public Capture(Slot origin, Slot dest, Slot captured1, Slot captured2)
      {
         Origin = origin;
         Dest = dest;
         Captured1 = captured1;
         Captured2 = captured2;
      }
   }

   public class NewTurn : GameProgress
   {

   }

   public class GameManager
   {

      public Board board;
      public List<IPlayer> players;
      public int currentPlayerIndex = 0;
      public int seed;
      public Random random;


      public GameManager(int seed)
      {
         this.seed = seed;
         random = new Random(seed);
      }

      public IEnumerable<GameProgress> PlayGame()
      {
         // assign player codes...
         for (var i = 0; i < players.Count; i++)
         {
            players[i].PlayerCode = i;
         }

         while (true)
         {
            var activePlayer = players[currentPlayerIndex];
            yield return new NewTurn();
            var foundMove = false;
            foreach (var progress in activePlayer.MakeMove(board))
            {
               if (foundMove)
               {
                  break;
               }
               switch (progress)
               {
                  case PlayerMove move:
                     var isValid = ValidateMove(activePlayer, move);
                     if (!isValid)
                     {
                        throw new Exception($"Invalid move attempted {activePlayer} {move} ");
                     }

                     foundMove = true;
                     board.SetPiece(move.position, move.piece);

                     // check for captures, and emit those events if required...
                     yield return move;
                     if (CheckForCapture(move.position, out var capture))
                     {
                        board.RemovePiece(capture.Captured1.position);
                        board.RemovePiece(capture.Captured2.position);
                        yield return capture;

                        activePlayer.AwardedCaptures++;
                        if (activePlayer.AwardedCaptures >= 5)
                        {
                           yield return new PlayerWon();
                           yield break;
                        }
                     }

                     break;
                  default:
                     yield return progress;
                     break;
               }
            }

            if (CheckWin())
            {
               // the game is over!
               yield return new PlayerWon();
               yield break;
            }

            currentPlayerIndex++;
            currentPlayerIndex %= players.Count;
         }

      }

      public bool ValidateMove(IPlayer player, PlayerMove move)
      {
         return true; // TODO validate a move...
      }

      public bool CheckForCapture(Vector2Int position, out Capture capture)
      {
         var directions = new Vector2Int[] {Vector2Int.up, Vector2Int.right, Vector2Int.one, new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1), Vector2Int.down, Vector2Int.left,  };
         capture = null;
         foreach (var direction in directions)
         {
            if (board.IsStartOfCapture(position, direction))
            {
               capture = new Capture(
                  board.GetSlot(position),
                  board.GetSlot(position + direction * 3),
                  board.GetSlot(position + direction * 1),
                  board.GetSlot(position + direction * 2)
                  );
               return true;
            }
         }

         return false;
      }

      public bool CheckWin()
      {
         // check if there are N pieces in a row...

         var directions = new Vector2Int[] {Vector2Int.up, Vector2Int.right, Vector2Int.one, new Vector2Int(-1, 1), };
         foreach (var direction in directions)
         {
            foreach (var slot in board.AllSlots)
            {
               if (board.IsStartOfNRow(slot.position, direction, 5))
               {
                  return true;
               }
            }
         }

         return false; // TODO check if someone has won yet...
      }
   }
}