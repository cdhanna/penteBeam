using System;
using System.Collections.Generic;
using Beamable;
using Beamable.Common;
using Beamable.Common.Content;
using Pente.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Pente.Unity
{
   [CreateAssetMenu]
   [ContentType("pieceSet")]
   public class PlayerPieceSet : ContentObject
   {
      public List<AddressablePieceBehaviour> pieceReferences;

      public Promise<PieceBehaviour> CreateRandomPiece(SlotBehaviour slot, GameManager game)
      {
         var index = 0;
         var reference = pieceReferences[index];

         var taskHandle = !reference.OperationHandle.IsValid() ? reference.LoadAssetAsync() : reference.OperationHandle.Convert<GameObject>();
         var loading = taskHandle.Task.ToPromise();
         return loading.FlatMap(template =>
         {
            var gob = Instantiate(template, slot.transform);
            var piece = gob.GetComponent<PieceBehaviour>();
            piece.OnCreated(slot, game);

            // play the spawn animation...

            return Promise<PieceBehaviour>.Successful(piece).WaitForSeconds(1);
         });
      }
   }

   [System.Serializable]
   public class PlayerPieceSetRef : ContentRef<PlayerPieceSet>
   {

   }

   [Serializable]
   public class AddressablePieceBehaviour : AssetReferenceGameObject
   {
      public AddressablePieceBehaviour(string guid) : base(guid)
      {
      }

      public override bool ValidateAsset(string path)
      {
         var piece = AssetDatabase.LoadAssetAtPath<PieceBehaviour>(path);
         return piece != null;
      }
   }
}