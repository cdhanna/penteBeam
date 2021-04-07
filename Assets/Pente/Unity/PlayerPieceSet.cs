using System;
using System.Collections.Generic;
using System.Linq;
using Beamable;
using Beamable.Common;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Pente.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Pente.Unity
{
   [CreateAssetMenu]
   [ContentType("pieceSet")]
   public class PlayerPieceSet : ItemContent
   {
      public AddressablePieceBehaviour pieceReference;
      public List<AddressableMaterial> skins = new List<AddressableMaterial>();


      public Dictionary<string, string> GetAllSkinsEnabledProperty()
      {
         return skins.ToDictionary(s => s.AssetGUID, s => "true");
      }

      private Promise<GameObject> _loadPromise;

      public Promise<GameObject> LoadPromise => _loadPromise ?? (_loadPromise =
                                                       (!pieceReference.OperationHandle.IsValid()
                                                          ? pieceReference.LoadAssetAsync()
                                                          : pieceReference.OperationHandle.Convert<GameObject>()).Task
                                                       .ToPromise());

      public Promise<PieceBehaviour> CreateRandomPiece(SlotBehaviour slot, GameManager game, string skinId=null)
      {
         var index = 0;

//         var taskHandle = !pieceReference.OperationHandle.IsValid() ? pieceReference.LoadAssetAsync() : pieceReference.OperationHandle.Convert<GameObject>();
//         var loading = taskHandle.Task.ToPromise();


         var loading = LoadPromise;
         var skin = skins.FirstOrDefault(s => s.AssetGUID.Equals(skinId));
         var materialPromise = new Promise<Material>();
         if (!string.IsNullOrEmpty(skinId) && skin != null)
         {
            var skinTaskHandle = !skin.OperationHandle.IsValid()
               ? skin.LoadAssetAsync()
               : skin.OperationHandle.Convert<Material>();
            materialPromise = skinTaskHandle.Task.ToPromise();
         }
         else
         {
            materialPromise.CompleteSuccess(null);
         }

         return materialPromise.FlatMap(material => loading.FlatMap(template =>
         {
            var gob = Instantiate(template, slot.transform);
            var piece = gob.GetComponent<PieceBehaviour>();

            if (material != null)
            {
               piece.ApplyMaterial(material);
            }

            piece.OnCreated(slot, game);

            // play the spawn animation...
            return Promise<PieceBehaviour>.Successful(piece).WaitForSeconds(1);
         }));
      }
   }

   [System.Serializable]
   public class PlayerPieceSetRef : ItemRef<PlayerPieceSet>
   {

   }

   [Serializable]
   public class AddressableMaterial : AssetReferenceT<Material>
   {
      public AddressableMaterial(string guid) : base(guid)
      {
      }
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