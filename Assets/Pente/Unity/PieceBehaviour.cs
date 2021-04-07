using System;
using System.Collections;
using Beamable.Common;
using Pente.Core;
using UnityEngine;
using UnityEngine.XR;
using Random = UnityEngine.Random;

namespace Pente.Unity
{
   public class PieceBehaviour : MonoBehaviour
   {
      public Promise<Unit> spawnComplete = new Promise<Unit>();

      public Color flashColor = Color.red;

      public void OnCreated(SlotBehaviour slot, GameManager game)
      {

      }

      public void StartNeighborPlacement()
      {
         if (Random.value > .5f)
         {
            GetComponentInChildren<Animator>().SetTrigger("Curious");
         }
      }

      public void StartWinAnimation()
      {
         GetComponentInChildren<Animator>().SetTrigger("Win");
      }

      public void StartAttackAnimation(Vector2 direction)
      {
         StartCoroutine(HandleAttack(direction.normalized));
      }

      public void StartCapturedAnimation()
      {
         StartCoroutine(HandleCapture());
      }

      public IEnumerator HandleAttack(Vector2 direction)
      {
         // face the attack vector...

         var angle = Mathf.Atan2(direction.y, direction.x);
         var targetAngle = (-angle * 180 / 3.14f) + 90;

         transform.localEulerAngles = new Vector3(0, targetAngle, 0); // TODO add a cool animation for this

         GetComponentInChildren<Animator>().SetTrigger("Attack");

         yield return new WaitForSecondsRealtime(1);
      }

      public IEnumerator HandleCapture()
      {
         GetComponentInChildren<Animator>().SetTrigger("Die");

         yield return new WaitForSecondsRealtime(1.5f);

         // flash the object out of existence...
         var renderer = GetComponentInChildren<SkinnedMeshRenderer>();

         for (var i = 0; i < 5; i++)
         {
            renderer.material.color = i % 2 == 0 ? flashColor : Color.white;
            yield return new WaitForSecondsRealtime(.2f);
         }

         renderer.material.color = Color.white;

         Destroy(gameObject);

      }

      public void ApplyMaterial(Material material)
      {
         var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
         renderer.material = material;
      }
   }
}