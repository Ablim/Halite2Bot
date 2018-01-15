using Halite2.hlt;
using System;
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
            var random = new Random();

            while (true)
            {
                moveList.Clear();
                gameMap.UpdateMap(Networking.ReadLineIntoMetadata());

                foreach (var ship in gameMap.GetMyPlayer().GetShips().Values)
                {
                    /**
                     * Fly ships. Conquer planets to get more ships. Fight other ships. Repeat.
                     * Ultimate question: what should each ship do?
                     **/
                    if (ship.GetDockingStatus() != Ship.DockingStatus.Undocked)
                    {
                        continue;
                    }

                    Planet bestPlanet = null;
                    double bestPlanetScore = 0;
                    Planet closestEnemyPlanet = null;

                    foreach (var planet in gameMap.GetAllPlanets().Values)
                    {
                        if (planet.GetOwner() == gameMap.GetMyPlayerId() || !planet.IsOwned())
                        {
                            if (planet.GetDockedShips().Count == planet.GetDockingSpots())
                            {
                                continue;
                            }

                            var currentPlanetScore = GetPlanetScore(planet, ship);

                            if (currentPlanetScore > bestPlanetScore)
                            {
                                bestPlanetScore = currentPlanetScore;
                                bestPlanet = planet;
                            }
                        }
                        else
                        {
                            if (closestEnemyPlanet == null)
                            {
                                closestEnemyPlanet = planet;
                            }
                            else if (planet.GetDistanceTo(ship) < closestEnemyPlanet.GetDistanceTo(ship))
                            {
                                closestEnemyPlanet = planet;
                            }
                        }
                    }

                    Move move = null;

                    if (bestPlanet != null)
                    {
                        if (ship.CanDock(bestPlanet))
                        {
                            move = new DockMove(ship, bestPlanet);
                        }
                        else 
                        {
                            move = Navigation.NavigateShipToDock(gameMap, ship, bestPlanet, Constants.MAX_SPEED);
                        }
                    }
                    else if (closestEnemyPlanet != null)
                    {
                        move = Navigation.NavigateShipTowardsTarget(gameMap, ship, closestEnemyPlanet, Constants.MAX_SPEED, true, Constants.MAX_NAVIGATION_CORRECTIONS, Math.PI / 180);
                    }

                    if (move != null)
                    {
                        moveList.Add(move);
                    }
                }

                Networking.SendMoves(moveList);
            }
        }

        private double GetPlanetScore(Planet planet, Ship ship)
        {
            var distance = planet.GetDistanceTo(ship);
            var production = planet.GetCurrentProduction();
            return production + (1 / distance);
        }

        public static void Main(string[] args)
        {
            var bot = new MyBot();
            bot.Run();
        }
    }
}
