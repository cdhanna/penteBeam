using System;
using System.Collections.Generic;

using UnityEngine;

namespace Pente.Core
{
   public class HeuristicMachine
   {


   }

   public class Minimax
   {


      public PlayerMove GetBestMove3(GameManager game, int playerCode, int allowedDepth)
      {

         var otherPlayerCode = playerCode == 0 ? 1 : 0;
         int AlphaBeta(GameManager node, int currentPlayer, int depth, int alpha, int beta)
         {
            var nextPlayer = currentPlayer == 0 ? 1 : 0;

            if (depth == 0)
            {
               // how good is this for the max player ?
               var playerScore = GetHeuristic(node, playerCode);
               return playerScore;
            }

            var moves = GetMovesFromState(node, currentPlayer);
            var isMaximizing = currentPlayer == playerCode;
            if (isMaximizing)
            {
               var value = int.MinValue;

               foreach (var move in moves)
               {
                  var child = node.Clone();
                  child.ApplyMove(move);

                  var childScore = AlphaBeta(child, nextPlayer, depth - 1, alpha, beta);
                  value = Math.Max(childScore, value); // pick the best score possible...

                  alpha = Math.Max(alpha, value);
                  if (alpha >= beta)
                  {
//                     Debug.Log("breaking alpha");

                     break;
                  }
               }

               return value;
            }
            else
            {
               var value = int.MaxValue;
               foreach (var move in moves)
               {
                  var child = node.Clone();
                  child.ApplyMove(move);

                  var childScore = AlphaBeta(child, nextPlayer, depth - 1, alpha, beta);
                  value = Math.Min(childScore, value); // the worst score possible, because this is the enemy.
                  beta = Math.Min(beta, value);
                  if (beta <= alpha)
                  {
//                     Debug.Log("breaking beta");
                     break;
                  }
               }

               return value;
            }

         }

         var topLevelMoves = GetMovesFromState(game, playerCode);
         var bestScore = int.MinValue;
         PlayerMove bestMove = null;
         var randomIndex = 0;//game.NextRandom(topLevelMoves.Count);
         var topAlpha = int.MinValue;
         var topBeta = int.MaxValue;
         for (var i = 0; i < topLevelMoves.Count; i++)
         {
            var move = topLevelMoves[(i + randomIndex) % topLevelMoves.Count];
            var option = game.Clone();
            option.ApplyMove(move);
            var score = AlphaBeta(option, otherPlayerCode, allowedDepth, topAlpha, topBeta);
            topAlpha = Math.Max(topAlpha, score);
            if (score > bestScore)
            {
               bestMove = move;
               bestScore = score;

            }
         }

         return bestMove;

      }


      public PlayerMove GetBestMove2(GameManager game, int playerCode, int allowedDepth)
      {

         var otherPlayerCode = playerCode == 0 ? 1 : 0;
         int Minimax(GameManager node, int currentPlayer, int depth)
         {
            var nextPlayer = currentPlayer == 0 ? 1 : 0;

            if (depth == 0)
            {
               // how good is this for the max player ?
               var playerScore = GetHeuristic(node, playerCode);
               return playerScore;
            }

            var moves = GetMovesFromState(node, currentPlayer);
            var isMaximizing = currentPlayer == playerCode;
            if (isMaximizing)
            {
               var value = int.MinValue;

               foreach (var move in moves)
               {
                  var child = node.Clone();
                  child.ApplyMove(move);

                  var childScore = Minimax(child, nextPlayer, depth - 1);
                  value = Math.Max(childScore, value); // pick the best score possible...

               }

               return value;
            }
            else
            {
               var value = int.MaxValue;
               foreach (var move in moves)
               {
                  var child = node.Clone();
                  child.ApplyMove(move);

                  var childScore = Minimax(child, nextPlayer, depth - 1);
                  value = Math.Min(childScore, value); // the worst score possible, because this is the enemy.
               }

               return value;
            }

         }

         var topLevelMoves = GetMovesFromState(game, playerCode);
         var bestScore = int.MinValue;
         PlayerMove bestMove = null;
         var randomIndex = game.NextRandom(topLevelMoves.Count);
         for (var i = 0; i < topLevelMoves.Count; i++)
         {
            var move = topLevelMoves[(i + randomIndex) % topLevelMoves.Count];
            var option = game.Clone();
            option.ApplyMove(move);
            var score = Minimax(option, otherPlayerCode, allowedDepth);
            if (score > bestScore)
            {
               bestMove = move;
               bestScore = score;

            }
         }

         return bestMove;

      }

      /// <summary>
      /// SCore the given board for the player given... Higher scores are better.
      /// </summary>
      /// <param name="game"></param>
      /// <param name="playerCode"></param>
      /// <returns></returns>
      public int GetHeuristic(GameManager game, int playerCode)
      {



         // number of captures pows up for target player, and pows away from other player...
         var score = 0;
         var captures = 0;
         var otherCaptures = 0;
         foreach (var player in game.players)
         {
            if (player.PlayerCode == playerCode)
            {
               captures = player.AwardedCaptures;
            }
            else
            {
               otherCaptures += player.AwardedCaptures;
            }
         }
         score += captures * 150;
         score -= otherCaptures * 150; // slightly defensive. Its worse to be captured than it is to get a capture...
         if (captures == 5)
         {
            score = int.MaxValue; // this is a winning state for the player.
            return score;
         }

         if (otherCaptures == 5)
         {
            score = int.MinValue; // this is a loosing state
            return score;
         }

         // consecutive pieces award points.
         int CountUntil(Slot start, Vector2Int direction)
         {
            if ((start.piece?.PlayerCode ?? -1) != playerCode) return 0; // not in a row...

            var count = 1;

            for (var i = 1; i < 5; i++)
            {
               var checkPos = start.position + (i * direction);
               if (!game.board.TryGetSlot(checkPos, out var curr))
               {
                  break;
               }

               var isSamePiece = (curr.piece?.PlayerCode ?? -1) == playerCode;
               if (!isSamePiece)
               {
                  break; // try the next slot...
               }

               count++;
            }

            return count;
         }

         var directions = new Vector2Int[]
         {
            Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.up,
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(1, -1),
         };


         foreach (var slot in game.board.slots)
         {
            foreach (var direction in directions)
            {

               // every slot will count for points. Positive points if its in the player's chain, negative if in someone else chain.
               var slotValue = slot.piece?.PlayerCode ?? -1;
               if (slotValue == -1) continue; // skip this slot.

               var pointSign = slotValue == playerCode ? 1 : -2;
               // count up long the chain is...

               var count = 1;
               for (var i = 1; i < 5; i++)
               {
                  var maybePos = slot.position + direction * i;


                  if (!game.board.TryGetSlot(maybePos, out var otherSlot))
                  {
                     count = 0; // no-op the edge the of board. You can't count it.
                     break;
                  }

                  var otherSlotValue = otherSlot.piece?.PlayerCode ?? -1;
                  if (otherSlotValue == -1)
                  {
                     break;
                  }
                  if (otherSlotValue != slotValue)
                  {
                     count = 0; // this streak is worthless if it doesn't amount
                     break; // the streak is over!
                  }

                  count++; // this piece is the same player as the starting piece.
               }

               var scoreMod = pointSign * (int)Math.Pow(count, 2);
               if (count == 5 && pointSign == 1)
               {
                  scoreMod = 100000;
               }

               score += scoreMod;

            }
         }

         return score;
      }

      public List<PlayerMove> GetMovesFromState(GameManager parent, int playerCode)
      {
         var moves = new List<PlayerMove>();
         foreach (var slot in parent.board.AllSlots) // can only generate moves with neighbors...
         {
            if ((slot.piece?.PlayerCode ?? -1) != -1) continue;

            Slot x;
            if (! (parent.board.TryGetSlot(slot.position + new Vector2Int(0, 1), out x) && (x.piece?.PlayerCode ?? -1) != -1))
               if (! (parent.board.TryGetSlot(slot.position + new Vector2Int(0, -1), out x) && (x.piece?.PlayerCode ?? -1) != -1))
                  if (! (parent.board.TryGetSlot(slot.position + new Vector2Int(-1, 0), out x) && (x.piece?.PlayerCode ?? -1) != -1))
                     if (! (parent.board.TryGetSlot(slot.position + new Vector2Int(1, 0), out x) && (x.piece?.PlayerCode ?? -1) != -1))
                        if (! (parent.board.TryGetSlot(slot.position + new Vector2Int(1, 1), out x) && (x.piece?.PlayerCode ?? -1) != -1))
                           if (! (parent.board.TryGetSlot(slot.position + new Vector2Int(1, -1), out x) && (x.piece?.PlayerCode ?? -1) != -1))
                              if (! (parent.board.TryGetSlot(slot.position + new Vector2Int(-1, -1), out x) && (x.piece?.PlayerCode ?? -1) != -1))
                                 if (! (parent.board.TryGetSlot(slot.position + new Vector2Int(-1, 1), out x) && (x.piece?.PlayerCode ?? -1) != -1))
                  continue;
//               if (!parent.board.TryGetSlot(slot.position + Vector2Int.down, out var _))
//                  if (!parent.board.TryGetSlot(slot.position + Vector2Int.left, out var _))
//                     if (!parent.board.TryGetSlot(slot.position + Vector2Int.right, out var _))
//                        if (!parent.board.TryGetSlot(slot.position + Vector2Int.one, out var _))
//                           if (!parent.board.TryGetSlot(slot.position + new Vector2Int(-1, 1), out var _))
//                              if (!parent.board.TryGetSlot(slot.position + new Vector2Int(1, -1), out var _))
//                                 if (!parent.board.TryGetSlot(slot.position + new Vector2Int(-1, -1), out var _))
//                                    continue;
//            var hasLeft = parent.board.TryGetSlot(slot.position + Vector2Int.left, out var _);
//            var hasRight = parent.board.TryGetSlot(slot.position + Vector2Int.right, out var _);
//            var hasTopLeft = parent.board.TryGetSlot(slot.position + Vector2Int.one, out var _);
//            var hasTopRight = parent.board.TryGetSlot(slot.position + new Vector2Int(-1, 1), out var _);
//            var hasLowRight = parent.board.TryGetSlot(slot.position + new Vector2Int(1, -1), out var _);
//            var hasLowLeft = parent.board.TryGetSlot(slot.position + new Vector2Int(-1, -1), out var _);

            // the player could move here...
            moves.Add(new PlayerMove(slot.position, new Piece{PlayerCode = playerCode}));
         }

         return moves;
      }
   }
}