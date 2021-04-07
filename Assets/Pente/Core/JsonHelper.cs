using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beamable.Serialization.SmallerJSON;
using UnityEngine;

namespace Pente.Core
{
   public static class JsonHelper
   {

      public static string SerializeBoard(GameManager game) => Json.Serialize(GameToSmallerJson(game), new StringBuilder());

      public static ArrayDict GameToSmallerJson(GameManager game)
      {
         return new ArrayDict
         {
            {"size", game.board.size},
            {"seed", game.seed},
            {"slots", game.board.slots.Select(SlotToSmallerJson).ToArray()},
            {"players", game.players.Select(PlayerToSmallerJson).ToArray()}
         };
      }

      public static ArrayDict PlayerToSmallerJson(IPlayer player)
      {
         var type = player.PlayerType;
         return new ArrayDict
         {
            {"captures", player.AwardedCaptures},
            {"type", type},
            {"code", player.PlayerCode},
         };
      }

      public static ArrayDict SlotToSmallerJson(Slot slot)
      {
         return new ArrayDict
         {
            {"x", slot.position.x},
            {"y", slot.position.y},
            {"c", slot.piece?.PlayerCode ?? -1}
         };
      }

      public static GameManager DeserializeBoard(string json)
      {
         var dict = Json.Deserialize(json) as ArrayDict;
         return SmallerJsonToGame(dict);
      }

      public static GameManager SmallerJsonToGame(ArrayDict dict)
      {
         var size = int.Parse(dict["size"].ToString());
         var seed = int.Parse(dict["seed"].ToString());
         var board = new Board(size);
         var players = new List<IPlayer>();
         var slots = new List<Slot>();

         var game = new GameManager(seed, board);
         foreach (var obj in (IEnumerable<object>)dict["slots"])
         {
            var elem = obj as ArrayDict;

            var slot = SmallerJsonToSlot(elem);
            slots.Add(slot);
         }

         foreach (var obj in (IEnumerable<object>) dict["players"])
         {
            var elem = obj as ArrayDict;
            var type = elem["type"].ToString();
            var code = int.Parse(elem["code"].ToString());
            IPlayer player = null;
            switch (type)
            {
               case "cpu":
                  var cpuPlayer = new CpuPlayerBrain();
                  cpuPlayer.game = game;
                  player = cpuPlayer;
                  break;
               default:
                  player = new ProxyPlayer();
                  break;
            }

            player.AwardedCaptures = int.Parse(elem["captures"].ToString());
            player.PlayerCode = code;
            players.Add(player);
         }

         board.slots = slots;
         game.players = players;
         return game;
      }

      public static Slot SmallerJsonToSlot(ArrayDict dict)
      {
         var c = int.Parse(dict["c"].ToString());
         return new Slot
         {
            position = new Vector2Int(int.Parse(dict["x"].ToString()), int.Parse(dict["y"].ToString())),
            piece = c < 0 ? null : new Piece {PlayerCode = c}
         };
      }

   }
}