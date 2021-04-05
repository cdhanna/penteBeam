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

      public HumanPlayerBehaviour HumanPrefab;
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
//         AddHumanPlayer(); // 2 humans vs eachother, local style.
         AddHumanPlayer();
         AddCpuPlayer();

         StartCoroutine(Play());
      }

      public void AddCpuPlayer() => AddPlayer(CpuPrefab);
      public void AddHumanPlayer() => AddPlayer(HumanPrefab);

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

            // do stuff ? visual stuff?
            switch (progress)
            {
               case PlayerMove move:

                  var player = players[game.currentPlayerIndex];
                  foreach (var _ in SpawnPiece(player, move))
                  {
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

      IEnumerable SpawnPiece(PlayerBehaviour current, PlayerMove move)
      {
         var slot = boardBehaviour.board.GetSlot(move.position);
         var slotBehaviour = boardBehaviour.GetSlotBehaviour(slot);
         yield return current.SpawnPiece(slotBehaviour, game).ToYielder();
      }
   }
}