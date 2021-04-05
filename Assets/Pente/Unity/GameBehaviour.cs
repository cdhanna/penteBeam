using System;
using System.Collections;
using System.Collections.Generic;
using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class GameBehaviour : MonoBehaviour
   {
      public BoardBehaviour boardBehaviour;

      public GameManager game;

      public HumanPlayerBehaviour HumanPrefab;

      private IEnumerable<GameProgress> _gameProgress;

      private void Start()
      {

         game = new GameManager();
         game.board = boardBehaviour.CreateBoard();

         game.players = new List<Player>();
         AddHumanPlayer(); // 2 humans vs eachother, local style.
         AddHumanPlayer();

         StartCoroutine(Play());
      }

      public void AddHumanPlayer()
      {
         var player = new HumanPlayer();
         player.PlayerCode = game.players.Count; // TODO: is this too dirty? the player index will get reassigned by the server, I think?

         var humanInstance = Instantiate(HumanPrefab);
         humanInstance.OnCreated(this, player);
         game.players.Add(player);
      }

      IEnumerator Play()
      {
         _gameProgress = game.PlayGame();

         foreach (var progress in _gameProgress)
         {

            // do stuff ? visual stuff?
            switch (progress)
            {
               case PlayerMove move:
                  foreach (var _ in SpawnPiece(move))
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

      IEnumerable SpawnPiece(PlayerMove move)
      {
         // do something? TODO create game asset
         yield return new WaitForEndOfFrame();
      }
   }
}