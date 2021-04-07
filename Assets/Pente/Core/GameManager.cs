using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Beamable.Common;
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

   [Serializable]
   public class GameManager
   {

      public Board board;
      public List<IPlayer> players;
      public int currentPlayerIndex = 0;
      public int seed;
      public Random random;


      public GameManager(GameManager other)
      {
         this.seed = other.seed;
         this.board = new Board(other.board);
         this.currentPlayerIndex = other.currentPlayerIndex;
         this.random = new Random(this.seed); // TODO: how do we make sure the same number of randoms have been called?
         this.players = other.players.Select(p => p.Clone()).ToList();
      }

      public GameManager(int seed, int size)
      {
         this.seed = seed;
         BeamableLogger.Log("Setting log seed to " + seed);
         random = new Random(seed);
         board = new Board(size);
      }

      public GameManager(int seed, Board board)
      {
         this.seed = seed;
         BeamableLogger.Log("Setting log seed to " + seed);
         random = new Random(seed);
         this.board = board;
      }

      public IEnumerable<GameProgress> PlayGame()
      {
         // assign player codes...
//         for (var i = 0; i < players.Count; i++)
//         {
//            players[i].PlayerCode = i;
//         }

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
                     while (CheckForCapture(move.position, out var capture))
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

      public void ApplyMove(PlayerMove move)
      {
         board.SetPiece(move.position, move.piece);
         if (CheckForCapture(move.position, out var cap))
         {
            var player = players.FirstOrDefault(p => p.PlayerCode == cap.Origin.piece.PlayerCode);
            player.AwardedCaptures += 1;
         }
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

      public void SetSeed(int i)
      {
         BeamableLogger.Log("Setting log seed to " + seed);
         seed = i;
         random = new Random(seed);
      }

      public int NextRandom()
      {
         return random.Next();
      }

      public int NextRandom(int max)
      {
         return random.Next(max);
      }

      public GameManager Clone()
      {
         return new GameManager(this);
      }
   }
}