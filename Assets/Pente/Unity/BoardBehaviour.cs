using System;
using System.Collections;
using System.Collections.Generic;
using Beamable;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class BoardBehaviour : MonoBehaviour
   {
      public int boardSize = 6;

      public SlotBehaviour slotBehaviour;

      public SlotEvent OnSlotSelected;

      public Dictionary<Vector2Int, SlotBehaviour> slotToBehaviour = new Dictionary<Vector2Int, SlotBehaviour>();
      public Dictionary<Vector2Int, PieceBehaviour> slotToPiece = new Dictionary<Vector2Int, PieceBehaviour>();

      [ReadOnly]
      public Board board;

      public Board CreateBoard()
      {
         board = new Board(boardSize);
         InstantiateBoardSlots(board);
         return board;
      }

      private void OnDrawGizmos()
      {
         var halfSize = (int)(boardSize * .5f);

         for (var y = 0; y < boardSize; y++)
         {
            for (var x = 0; x < boardSize; x++)
            {
               var isOdd = (x + y) % 2 == 0;
               Gizmos.color = isOdd ? Color.blue : Color.cyan;

               var position = transform.InverseTransformPoint(new Vector3(y-halfSize, 0, x-halfSize));
               Gizmos.DrawWireCube(position, new Vector3(.8f, .2f, .8f));
            }
         }
      }

      public void InstantiateBoardSlots(Board board)
      {
         foreach (var slot in board.AllSlots)
         {
            var instance = Instantiate(slotBehaviour, transform);
            var halfSize = (int)(boardSize * .5f);
            instance.transform.localPosition = new Vector3(slot.position.x - halfSize, 0, slot.position.y - halfSize);

            instance.slot = slot;
            instance.board = this;

            instance.OnSelect.AddListener(HandleSlotSelection);
            slotToBehaviour[slot.position] = instance;
            instance.OnCreate();
         }
      }

      public int PieceSpawnIndex = 1;
      public IEnumerable SpawnPiece(GameBehaviour game, PlayerBehaviour current, PlayerMove move)
      {
         var slot = board.GetSlot(move.position);
         var slotBehaviour = GetSlotBehaviour(slot);
         var promise = current.SpawnPiece(slotBehaviour, game);
         game.TargetGroup.AddMember(slotBehaviour.transform, .5f * PieceSpawnIndex++, .1f);

         promise.Then(pieceB =>
         {
//            var newCam = Instantiate(game.VCamPrefab, game.CameraParent.transform);
//            newCam.LookAt = pieceB.transform;
         });
         yield return promise.ToYielder();

         slotToPiece[slot.position] = promise.GetResult();

         // are there any adjacent neighbors?
         foreach (var neighbor in board.GetNeighbors(slot))
         {
            if (board.TryGetPiece(neighbor.position, out var piece) && slotToPiece.TryGetValue(neighbor.position, out var neighborPiece) && piece.PlayerCode != current.PlayerCode)
            {
               neighborPiece.StartNeighborPlacement();
            }

         }

      }

      public IEnumerable GiveWinningTeam(GameBehaviour game, PlayerBehaviour current)
      {
         foreach (var slot in board.AllSlots)
         {
            if (slotToPiece.TryGetValue(slot.position, out var piece))
            {
               var isWinner = slot.piece.PlayerCode == current.PlayerCode;
               if (isWinner)
               {
                  piece.StartWinAnimation();
               }
            }
         }
         yield return new WaitForSecondsRealtime(1);
      }

      public IEnumerable DestroyPieces(GameBehaviour game, PlayerBehaviour current, Capture capture)
      {
         var captured1 = slotToPiece[capture.Captured1.position];
         var captured2 = slotToPiece[capture.Captured2.position];

         slotToPiece.Remove(capture.Captured1.position);
         slotToPiece.Remove(capture.Captured2.position);


         var killer1 = slotToPiece[capture.Dest.position];
         var killer1AttackDir = (capture.Origin.position - capture.Dest.position);
         killer1.StartAttackAnimation(killer1AttackDir);
         var killer2 = slotToPiece[capture.Origin.position];
         killer2.StartAttackAnimation(-killer1AttackDir);

         yield return new WaitForSecondsRealtime(1);

         captured1.StartCapturedAnimation();
         captured2.StartCapturedAnimation();


         yield return new WaitForSecondsRealtime(3);
//         yield return capture1Routine;
//         yield return capture2Routine;
//         Destroy(captured1.gameObject);
//         Destroy(captured2.gameObject);
         yield break; // TODO do some cool death animations here
      }

      public void HandleSlotSelection(SlotBehaviour slot)
      {
         OnSlotSelected?.Invoke(slot);
      }

      public SlotBehaviour GetSlotBehaviour(Slot slot)
      {
         return slotToBehaviour[slot.position];
      }
   }
}