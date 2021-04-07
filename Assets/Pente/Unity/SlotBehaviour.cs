using System;
using System.Collections;
using Pente.Core;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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

      public Material oddMaterial, evenMaterial;


      public bool isHover = false;
      public bool ScatterHeight = true;
      public bool ApplyMaterial = true;
      public bool MakeMessy = true;

      public bool AnimateIn = true;

      public SlotEvent OnSelect;

      private void OnEnable()
      {

      }

      public void OnCreate()
      {
         var isOdd = (slot.position.x + slot.position.y) % 2 == 0; // 0, 1 -> false, 1, 1 -> true, 0, 2
         if (ApplyMaterial)
         {
            GetComponentInChildren<MeshRenderer>().material = isOdd ? oddMaterial : evenMaterial;
         }

         if (ScatterHeight)
         {
            if (isOdd)
            {
               transform.localPosition += Vector3.up * .03f;
               transform.localScale *= .95f;
            }
         }

         if (MakeMessy)
         {
            transform.localScale *= Random.Range(.92f, .97f);
            transform.localEulerAngles = new Vector3(0, Random.Range(-6, 6), 0);
         }

         if (AnimateIn)
         {
            var distanceFromCenter = (transform.localPosition).magnitude;// - new Vector3(board.boardSize, 0, board.boardSize)*.5f).magnitude;
            StartCoroutine(AnimateInShow(distanceFromCenter));
         }
      }

      IEnumerator AnimateInShow(float delay)
      {
         var targetPos = transform.localPosition;
         var targetScale = transform.localScale;

         transform.localPosition = targetPos + Vector3.up * -2;
         transform.localScale = Vector3.zero;

         var scaleVel = Vector3.zero;
         var posVel = Vector3.zero;


         yield return new WaitForEndOfFrame();
         yield return new WaitForSecondsRealtime(delay * .09f);

         bool IsPositionBack() => (targetPos - transform.localPosition).sqrMagnitude < .05f;
         bool IsScaleBack() => (targetScale - transform.localScale).sqrMagnitude < .05f;

         while (!IsPositionBack() || !IsScaleBack())
         {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref scaleVel, .1f, 5, Time.deltaTime);
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref posVel, .05f, 15, Time.deltaTime);
            yield return new WaitForEndOfFrame();
         }

         transform.localPosition = targetPos;
         transform.localScale = targetScale;

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