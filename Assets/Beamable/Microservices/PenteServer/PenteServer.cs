using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Pente.Core;
using Beamable.Common;
using UnityEngine;
using Random = System.Random;

namespace Beamable.Server.PenteServer
{
    [Microservice("PenteServer")]
    public class PenteServer : Microservice
    {

        [ClientCallable]
        public async Task<StartGameResponse> NewGame()
        {
            var storage = new GameStorage(Services);

            var r = new Random();
            var seed = r.Next();
            var game = new GameManager(seed, 8);
            game.players = new List<IPlayer>();
            // add in some dud players.
            game.players.Add(new ProxyPlayer
            {
                AwardedCaptures = 0,
                PlayerCode = 0
            });
            game.players.Add(new CpuPlayerBrain
            {
                AwardedCaptures = 0,
                PlayerCode = 1
            });

            var gameJson = await storage.SaveGame(Context.UserId, seed, game);

            return new StartGameResponse
            {
                seed = seed,
                size = game.board.size,
                gameJson = gameJson
            };
        }

        [ClientCallable]
        public async Task<SubmitMoveResponse> SubmitMove(SubmitMoveRequest req)
        {
            var storage = new GameStorage(Services);

            // load up the stored game state,
            var data = await storage.LoadGame(Context.UserId);
            var game = data.Game;

            var r = new Random();
            var seed = r.Next();
            game.SetSeed(seed);

            // apply new random numbers to cpu players...
            foreach (var player in game.players)
            {
                if (player is CpuPlayerBrain cpu)
                {
                    cpu.game = game;
                }
            }

            BeamableLogger.Log("Simulating game...");

            foreach (var plr in game.players)
            {
                if (plr is ProxyPlayer proxyPlayer)
                {
                    BeamableLogger.Log("proxied move {move}", req.x);
                    proxyPlayer.move = new PlayerMove(new Vector2Int(req.x, req.y), proxyPlayer.CreateNewPiece());
                }
                BeamableLogger.Log("Player {plr} {type}", plr.PlayerCode, plr.GetType());
            }

            game.currentPlayerIndex = 0; // its always the human's turn...

            var moveCount = 0;
            var stop = false;
            // player two turns worth of data onto the board...
            foreach (var progress in game.PlayGame())
            {
                if (stop)
                {
                    break;
                }

                var currentPlayer = game.players[game.currentPlayerIndex];
                var isHuman = currentPlayer is ProxyPlayer;
                BeamableLogger.Log("progress {type}",progress.GetType());
                switch (progress)
                {
                    case NewTurn newTurn:
                        BeamableLogger.Log("new turn! {c} {activeIndex}", moveCount, game.currentPlayerIndex);
                        if (moveCount == 2)
                        {
                            stop = true;
                        }
                        break;
                    case Capture capture:
                        if (isHuman)
                        {
                            // the player got a capture!
                            Services.Inventory.AddCurrency("currency.gems", 5);
                        }
                        break;
                    case PlayerMove move:
                        moveCount++;
                        BeamableLogger.Log("Move made");

                        break;
                    case PlayerWon win:

                        if (isHuman)
                        {
                            // the player won!!!
                            Services.Inventory.AddCurrency("currency.gems", 50);
                        }

                        break;
                    default:
                        break;
                }


            }


            // award any points

            // save the board state


            var gameJson = await storage.SaveGame(Context.UserId, seed, game);

            // return the board state

            return new SubmitMoveResponse
            {
                seed = seed,
                gameJson = gameJson
            };
        }



    }
}