using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Api.Inventory;
using Beamable.Stats;

namespace Pente.Unity
{
   public static class UtilMethods
   {
      public static async Task EnsureDefaultPieces(this IBeamableAPI beamable, PlayerPieceSetRef defaultPieceRef, List<InventoryObject<PlayerPieceSet>> pieces, StatObject characterStat, StatObject skinStat)
      {
         if (pieces.Count == 0) // grant the first inventory item...
         {
            var defaultContent = await defaultPieceRef.Resolve();

            await beamable.InventoryService.AddItem(defaultPieceRef.Id, defaultContent.GetAllSkinsEnabledProperty());
            // auto select first skin...
            pieces.Clear();
            pieces.AddRange(await beamable.InventoryService.GetItems<PlayerPieceSet>());
            await characterStat.Write(defaultContent.Id);
            await skinStat.Write(defaultContent.GetAllSkinsEnabledProperty().First().Key);
         }
      }

   }
}