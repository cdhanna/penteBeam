using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Beamable;
using Beamable.Server.Clients;
using Cinemachine;
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

      public GameObject CameraParent;
      public CinemachineVirtualCamera VCamPrefab;
      public CinemachineMixingCamera MixingCamera;
      public CinemachineTargetGroup TargetGroup;

      private IEnumerable<GameProgress> _gameProgress;

      [ReadOnly]
      public List<PlayerBehaviour> players;

      private PenteServerClient _client = new PenteServerClient();
      private void Start()
      {

         players = new List<PlayerBehaviour>();

         _client.NewGame().Then(res =>
         {
            var serverGame = JsonHelper.DeserializeBoard(res.gameJson);
            seed = serverGame.seed;
            game = serverGame;

//            game = new GameManager(seed, res.size);
//            boardBehaviour.boardSize = res.size;
            boardBehaviour.board = game.board;
            boardBehaviour.InstantiateBoardSlots(game.board);

            var clientPlayers = new List<IPlayer>();
            foreach (var player in game.players)
            {
               switch (player)
               {
                  case CpuPlayerBrain cpu:
                     clientPlayers.Add(AddCpuPlayer(cpu));
                     break;
                  case ProxyPlayer human:
                     clientPlayers.Add(AddHumanPlayer(human));
                     break;
               }
            }

            game.players = clientPlayers;
            players = game.players.Cast<PlayerBehaviour>().ToList();

            StartCoroutine(Play());
         });

      }

      public IPlayer AddCpuPlayer(IPlayer template) => AddPlayer(CpuPrefab, template);
      public IPlayer AddHumanPlayer(IPlayer template) => AddPlayer(HumanPrefabs[game.players.Count % HumanPrefabs.Count], template);

      public IPlayer AddPlayer<T>(T prefab, IPlayer template) where T : PlayerBehaviour, IPlayer
      {
         var instance = Instantiate(prefab);
         instance.PlayerCode = template.PlayerCode;
         instance.AwardedCaptures = template.AwardedCaptures;
         instance.OnCreated(this);
         return instance;
//         players.Add(instance);
//         game.players.Add(instance);
      }

      IEnumerator Play()
      {
         var beamable = Beamable.API.Instance;
         yield return beamable.ToYielder(); // spawn up some beams;
         _gameProgress = game.PlayGame();


         GameManager lastServerGameState = null;

         var checkBoardSate = false;

         foreach (var progress in _gameProgress)
         {
            var player = players[game.currentPlayerIndex];
            switch (progress)
            {
               case NewTurn newTurn when checkBoardSate:

                  checkBoardSate = false;

//                  // the move was an AI move. At this point, we should be in sync with the server...
                  var currBoard = JsonHelper.SerializeBoard(game);
                  var lastBoard = JsonHelper.SerializeBoard(lastServerGameState);

                  if (!string.Equals(currBoard, lastBoard))
                  {
                     Debug.Log(currBoard);
                     Debug.Log(lastBoard);
                     Debug.LogError("Server state is not in sync! HAAACKS!");
                  }


                  break;

               case PlayerMove move:

                  if (player is HumanPlayerBehaviour)
                  {
                     // we should update the server state of the board...
                     var validationPromise = _client.SubmitMove(new SubmitMoveRequest
                     {
                        x = move.position.x,
                        y = move.position.y,
                     }).Then(res =>
                     {
                        var serverGame = JsonHelper.DeserializeBoard(res.gameJson);
                        // the board state is going to include the next AI move in it already... we can just use that, can't we ?

                        lastServerGameState = serverGame;
                        game.seed = serverGame.seed; // sync the seed.
                        game.random = new System.Random(game.seed);

                        Debug.Log("Got server response...");
                     });
                     yield return validationPromise.ToYielder(); // wait for server result...


                  }
                  else
                  {
                     checkBoardSate = true;

                  }

                  Debug.Log("carrying on with move stuff.");


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

                  foreach (var waitable in boardBehaviour.GiveWinningTeam(this, player))
                  {
                     if (waitable is CustomYieldInstruction yielder)
                     {
                        yield return yielder;
                     }
                     yield return new WaitForEndOfFrame();
                  }
                  break;
            }

            yield return new WaitForEndOfFrame();

         }
      }

   }
}