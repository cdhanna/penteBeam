using System;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class SlotBehaviour : MonoBehaviour
   {
      [ReadOnly]
      public Slot slot;

      [HideInInspector]
      public Board board;


      public bool ScatterHeight = true;

      private void OnEnable()
      {

      }

      public void OnCreate()
      {
         if (ScatterHeight)
         {
            var isOdd = (slot.position.x + slot.position.y) % 2 == 0; // 0, 1 -> false, 1, 1 -> true, 0, 2
            if (isOdd)
            {
               transform.localPosition += Vector3.up * .03f;
               transform.localScale *= .99f;
            }
         }
      }
   }
}