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
   }

   [Serializable]
   public class Slot
   {
      public Vector2Int position;
      public Piece piece;
   }

   [Serializable]
   public class Piece
   {
      public int PlayerCode = -1;
   }
}