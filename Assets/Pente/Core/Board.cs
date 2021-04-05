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
                  position = position
               };
               slots.Add(slot);
            }
         }
      }

      public Slot GetSlot(Vector2Int position)
      {
         if (position.x < 0 || position.y < 0 || position.x >= size || position.y >= size)
            throw new IndexOutOfRangeException("board size");

         // TODO: Refactor this with a dictionary lookup. but keep the list for serialization...
         return slots.FirstOrDefault(s => s.position.Equals(position));
      }

      public IEnumerable<Slot> AllSlots => slots;
   }

   [Serializable]
   public class Slot
   {
      public Vector2Int position;
   }
}