using Pente.Core;
using UnityEngine;

namespace Pente.Unity
{
   public class HumanPlayerBehaviour : MonoBehaviour
   {
      public HumanPlayer player;
      public GameBehaviour game;

      public void OnCreated(GameBehaviour gameBehaviour, HumanPlayer humanPlayer)
      {
         player = humanPlayer;
         game = gameBehaviour;

         game.boardBehaviour.OnSlotSelected.AddListener(HandleSlotSelection);
      }

      public void HandleSlotSelection(SlotBehaviour slot)
      {
         player.SetNextMove(slot.slot.position);
      }

   }
}