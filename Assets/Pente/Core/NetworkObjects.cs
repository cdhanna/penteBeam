using System;

namespace Pente.Core
{
   [Serializable]
   public class StartGameResponse
   {
      public int seed;
      public int size;
      public string gameJson;
   }

   [Serializable]
   public class SubmitMoveRequest
   {
      public int x, y;
   }

   [Serializable]
   public class SubmitMoveResponse
   {
      public int seed;
      public string gameJson;
   }
}