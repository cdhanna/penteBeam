using System.Collections.Generic;
using System.Threading.Tasks;
using Pente.Core;

namespace Beamable.Server.PenteServer
{

   public class BoardData
   {
      public GameManager Game { get; set; }
      public int Seed { get; set; }
   }
   public class GameStorage
   {
      private readonly IBeamableServices _services;

      public GameStorage(IBeamableServices services)
      {
         _services = services;
      }

      public async Task<string> SaveGame(long userId, int seed, GameManager game)
      {
         var json = JsonHelper.SerializeBoard(game);
         await SaveNewGame(userId, json);
         return json;
      }

      public async Task<BoardData> LoadGame(long userId)
      {
         var stats = await _services.Stats.GetProtectedPlayerStats(userId, new[] {"gameState"});
         var json = stats["gameState"];
         var game = JsonHelper.DeserializeBoard(json);

         return new BoardData
         {
            Game = game,
            Seed = game.seed
         };
      }

      public async Task SaveNewGame(long userId,string gameStateJson)
      {
         await _services.Stats.SetProtectedPlayerStats(userId, new Dictionary<string, string>
         {
            {"gameState", gameStateJson}
         });
      }
   }
}