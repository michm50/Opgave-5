using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sport;

namespace ServerTCP
{
    class ObjectStore
    {
        public static List<FootballPlayer> Players = new List<FootballPlayer>()
        {
            new FootballPlayer(1, "Martin Hansen", 149.00m, 22),
            new FootballPlayer(2, "Foliak Chunkyboi", 243.88m, 99),
            new FootballPlayer(3, "Richard Fawkes", 399.00m, 48),
            new FootballPlayer(4, "Helle etellerandet", 2315782345.00m, 1)
        };

        public static List<FootballPlayer> Get()
        {
            return Players;
        }

        public static FootballPlayer Get(int id)
        {
            return Players.Find(p => p.Id == id);
        }
    }
}
