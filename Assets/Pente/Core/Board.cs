using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pente.Core
{
   [Serializable]
   public class Board
   {
      public List<Slot> slots;
      public int size;

      public Board(Board other)
      {
         size = other.size;
         slots = other.slots.Select(s => s.Clone()).ToList();
      }
      public Board(int size)
      {
         slots = new List<Slot>();

         this.size = size;
         for (var y = 0; y < size; y++)
         {
            for (var x = 0; x < size; x++)
            {
               var position = new Vector2Int(x, y);
               var slot = new Slot
               {
                  position = position,
                  piece = null
               };
               slots.Add(slot);
            }
         }
      }

      public bool IsStartOfCapture(Vector2Int position, Vector2Int direction)
      {
         var start = GetSlot(position);
         if (start.piece == null || start.piece.PlayerCode == -1) return false;

         if (!TryGetSlot(position + direction * 3, out var end))
         {
            return false;
         }

         if (end.piece == null || end.piece.PlayerCode != start.piece.PlayerCode) return false;

         if (!TryGetSlot(position + direction * 1, out var midSlot1))
         {
            return false;
         }
         if (!TryGetSlot(position + direction * 2, out var midSlot2))
         {
            return false;
         }

         if (midSlot1 == null || midSlot2 == null) return false;
         if (midSlot1.piece == null || midSlot2.piece == null) return false;
         if (midSlot1.piece.PlayerCode == -1 || midSlot2.piece.PlayerCode == -1) return false;
         if (midSlot1.piece.PlayerCode != midSlot2.piece.PlayerCode) return false;
         if (midSlot1.piece.PlayerCode == start.piece.PlayerCode) return false;

         return true;
      }

      public bool IsStartOfNRow(Vector2Int position, Vector2Int direction, int n)
      {
         var start = GetSlot(position);
         if (start.piece == null || start.piece.PlayerCode == -1) return false;
         for (var i = 0; i < n; i++)
         {
            var currPosition = position + direction * i;


            if (!TryGetSlot(currPosition, out var slot))
            {
               return false;
            }

            if (slot.piece == null)
            {
               return false;
            }

            if (slot.piece.PlayerCode != start.piece.PlayerCode)
            {
               return false;
            }
         }

         return true;
      }


      public bool TryGetPiece(Vector2Int position, out Piece piece)
      {
         var slot = GetSlot(position);

         piece = slot.piece;
         return piece != null;
      }

      public void SetPiece(Vector2Int position, Piece piece)
      {
         var slot = GetSlot(position);
         slot.piece = piece;
      }

      public void RemovePiece(Vector2Int position)
      {
         var slot = GetSlot(position);
         slot.piece = null;
      }

      public void MovePiece(Vector2Int origin, Vector2Int destination)
      {
         var originSlot = GetSlot(origin);
         var destinationSlot = GetSlot(destination);

         destinationSlot.piece = originSlot.piece;
         originSlot.piece = null;
      }

      public Slot GetSlot(Vector2Int position)
      {
         if (position.x < 0 || position.y < 0 || position.x >= size || position.y >= size)
            throw new IndexOutOfRangeException("board size");

         // TODO: Refactor this with a dictionary lookup. but keep the list for serialization...
         return slots.FirstOrDefault(s => s.position.Equals(position));
      }

      public bool TryGetSlot(Vector2Int position, out Slot slot)
      {
          slot = slots.FirstOrDefault(s => s.position.Equals(position));
          return slot != null;
      }

      public IEnumerable<Slot> AllSlots => slots;

      public Board Clone()
      {
         return new Board(this);
      }

      public IEnumerable<Slot> GetNeighbors(Slot slot)
      {
         var directions = new Vector2Int[]
         {
            Vector2Int.down, Vector2Int.one, Vector2Int.left, Vector2Int.up, Vector2Int.right
         };
         foreach (var dir in directions)
         {
            if (TryGetSlot(slot.position + dir, out var neighbor))// && TryGetPiece(slot.position + dir, out var piece))
            {
               yield return neighbor;
            }
         }
      }
   }

   [Serializable]
   public class Slot
   {
      public Vector2Int position;
      public Piece piece;

      public Slot Clone()
      {
         return new Slot
         {
            position = position,
            piece = piece == null ? null : new Piece{PlayerCode = piece.PlayerCode}
         };

      }
   }

   [Serializable]
   public class Piece
   {
      public int PlayerCode = -1;
   }
}