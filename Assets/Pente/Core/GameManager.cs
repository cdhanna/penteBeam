using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pente.Core
{
   public class GameProgress
   {

   }

   public class PlayerWon : GameProgress
   {

   }

   public class NewTurn : GameProgress
   {

   }

   public class GameManager
   {

      public Board board;
      public List<Player> players;
      public int currentPlayerIndex = 0;

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
            foreach (var progress in activePlayer.MakeMove(board))
            {
               switch (progress)
               {
                  case PlayerMove move:
                     var isValid = ValidateMove(activePlayer, move);
                     if (!isValid)
                     {
                        throw new Exception($"Invalid move attempted {activePlayer} {move} ");
                     }
                     board.SetPiece(move.position, move.piece);
                     break;
               }
               yield return progress;
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

      public bool ValidateMove(Player player, PlayerMove move)
      {
         return true; // TODO validate a move...
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