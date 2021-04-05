using System;
using System.Collections;
using System.Collections.Generic;
using Beamable;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class GameBehaviour : MonoBehaviour
   {
      public BoardBehaviour boardBehaviour;

      public int seed = 1;
      public GameManager game;

      public List<HumanPlayerBehaviour> HumanPrefabs;
      public CpuPlayerBehaviour CpuPrefab;

      private IEnumerable<GameProgress> _gameProgress;

      [ReadOnly]
      public List<PlayerBehaviour> players;

      private void Start()
      {

         players = new List<PlayerBehaviour>();
         game = new GameManager(seed);
         game.board = boardBehaviour.CreateBoard();
         game.players = new List<IPlayer>();
         AddHumanPlayer(); // 2 humans vs eachother, local style.
         AddHumanPlayer();
//         AddCpuPlayer();

         StartCoroutine(Play());
      }

      public void AddCpuPlayer() => AddPlayer(CpuPrefab);
      public void AddHumanPlayer() => AddPlayer(HumanPrefabs[game.players.Count % HumanPrefabs.Count]);

      public void AddPlayer<T>(T prefab) where T : PlayerBehaviour, IPlayer
      {
         var instance = Instantiate(prefab);
         instance.PlayerCode = game.players.Count;
         instance.OnCreated(this);
         players.Add(instance);
         game.players.Add(instance);
      }

      IEnumerator Play()
      {
         var beamable = Beamable.API.Instance;
         yield return beamable.ToYielder(); // spawn up some beams;
         _gameProgress = game.PlayGame();

         foreach (var progress in _gameProgress)
         {
            var player = players[game.currentPlayerIndex];
            switch (progress)
            {
               case PlayerMove move:

                  foreach (var waitable in boardBehaviour.SpawnPiece(this, player, move))
                  {
                     if (waitable is CustomYieldInstruction yielder)
                     {
                        yield return yielder;
                     }
                     yield return new WaitForEndOfFrame();
                  }
                  break;
               case Capture capture:
                  foreach (var waitable in boardBehaviour.DestroyPieces(this, player, capture))
                  {
                     if (waitable is CustomYieldInstruction yielder)
                     {
                        yield return yielder;
                     }
                     yield return new WaitForEndOfFrame();
                  }
                  break;
               case PlayerWon won:
                  Debug.Log("GAME OVER!!! ");
                  break;
            }

            yield return new WaitForEndOfFrame();

         }
      }

   }
}