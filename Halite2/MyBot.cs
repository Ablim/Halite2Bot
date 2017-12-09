using Halite2.hlt;
using System.Collections.Generic;

namespace Halite2
{
    public class MyBot
    {
        public MyBot()
        {

        }

        public void Run()
        {
            string name = "AblimBot";

            Networking networking = new Networking();
            GameMap gameMap = networking.Initialize(name);
            List<Move> moveList = new List<Move>();

            while (true)
            {
                moveList.Clear();
                gameMap.UpdateMap(Networking.ReadLineIntoMetadata());
                var takenPlanets = new Dictionary<int, int>();

                foreach (var ship in gameMap.GetMyPlayer().GetShips().Values)
                {
                    /**
                     * Fly ships. Conquer planets to get more ships. Fight other ships. Repeat.
                     * Ultimate question: what should each ship do?
                     **/

                    //Greedy, take the best planet first

                    Planet bestPlanet = null;
                    double bestPlanetScore = 0;

                    foreach (var planet in gameMap.GetAllPlanets().Values)
                    {
                        if (planet.GetOwner() == gameMap.GetMyPlayerId())
                        {
                            continue;
                        }
                        else if (planet.GetDockedShips().Count == planet.GetDockingSpots())
                        {
                            continue;
                        }
                        else if (takenPlanets.ContainsKey(planet.GetId()))
                        {
                            continue;
                        }

                        var distance = planet.GetDistanceTo(ship);
                        var production = planet.GetCurrentProduction();
                        var currentPlanetScore = production + (1 / distance);

                        if (currentPlanetScore > bestPlanetScore)
                        {
                            bestPlanetScore = currentPlanetScore;
                            bestPlanet = planet;
                        }
                    }

                    if (bestPlanet != null)
                    {
                        takenPlanets.Add(bestPlanet.GetId(), ship.GetId());

                        if (ship.CanDock(bestPlanet))
                        {
                            var move = new DockMove(ship, bestPlanet);

                            if (move != null)
                            {
                                moveList.Add(move);
                            }

                            continue;

                        }
                        else
                        {
                            var move = Navigation.NavigateShipToDock(gameMap, ship, bestPlanet, Constants.MAX_SPEED);

                            if (move != null)
                            {
                                moveList.Add(move);
                            }

                            continue;
                        }
                    }
                    else
                    {
                        //Fight!

                    }
                }

                Networking.SendMoves(moveList);
            }
        }

        public static void Main(string[] args)
        {
            var bot = new MyBot();
            bot.Run();
        }
    }
}
