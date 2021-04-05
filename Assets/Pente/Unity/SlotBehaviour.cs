using System;
using Pente.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Pente.Unity
{

   [Serializable]
   public class SlotEvent : UnityEvent<SlotBehaviour> {}

   public class SlotBehaviour : MonoBehaviour
   {
      [ReadOnly]
      public Slot slot;

      [HideInInspector]
      public BoardBehaviour board;


      public bool isHover = false;
      public bool ScatterHeight = true;

      public SlotEvent OnSelect;

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
               transform.localScale *= .95f;
            }
         }
      }

      private void OnDrawGizmos()
      {
         if (isHover)
         {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position, Vector3.one * .7f);
         }

         if (slot.piece != null)
         {
            var debugColors = new Color[] {Color.blue, Color.green, Color.cyan};
            var color = debugColors[slot.piece.PlayerCode % debugColors.Length];
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position + Vector3.up * .3f, .5f);
         }

      }

      private void OnMouseUp()
      {
         // clicked!
         OnSelect.Invoke(this);
      }

      private void OnMouseEnter()
      {
         isHover = true;
      }

      private void OnMouseExit()
      {
         isHover = false;
      }
   }
}