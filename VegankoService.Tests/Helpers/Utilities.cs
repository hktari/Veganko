using System.Collections.Generic;
using VegankoService.Data;

namespace VegankoService.Tests
{
    public static class Utilities
    {
        #region snippet1
        public static void InitializeDbForTests(VegankoContext db)
        {
            //db.Messages.AddRange(GetSeedingMessages());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(VegankoContext db)
        {
            //db.Messages.RemoveRange(db.Messages);
            InitializeDbForTests(db);
        }

        //public static List<Message> GetSeedingMessages()
        //{
        //    return new List<Message>()
        //    {
        //        new Message(){ Text = "TEST RECORD: You're standing on my scarf." },
        //        new Message(){ Text = "TEST RECORD: Would you like a jelly baby?" },
        //        new Message(){ Text = "TEST RECORD: To the rational mind, " +
        //            "nothing is inexplicable; only unexplained." }
        //    };
        //}
        #endregion
    }
}
