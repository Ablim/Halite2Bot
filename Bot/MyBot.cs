using Bot.hlt;
using System;
using System.Collections.Generic;

namespace Bot
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
                var planetLookup = new Dictionary<int, int>();

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

                    Planet closestPlanet = null;
                    double closestPlanetDistance = double.MaxValue;
                    Planet closestNeutralPlanet = null;
                    double closestNeutralPlanetDistance = double.MaxValue;

                    foreach (var planet in gameMap.GetAllPlanets().Values)
                    {
                        var distance = planet.GetDistanceTo(ship);
                        
                        if (planet.GetOwner() == gameMap.GetMyPlayerId() &&
                            planet.GetDockedShips().Count < planet.GetDockingSpots() &&
                            distance < closestPlanetDistance)
                        {
                            closestPlanetDistance = distance;
                            closestPlanet = planet;
                        }
                        else if (!planet.IsOwned() && 
                                 distance < closestNeutralPlanetDistance)
                        {
                            closestNeutralPlanetDistance = distance;
                            closestNeutralPlanet = planet;
                        }
                    }

                    Ship closestEnemy = null;
                    double closestEnemyDistance = double.MaxValue;
                    Ship closestDockedEnemy = null;
                    double closestDockedEnemyDistance = double.MaxValue;

                    foreach (var enemy in gameMap.GetAllShips())
                    {
                        if (enemy.GetOwner() != gameMap.GetMyPlayerId())
                        {
                            var distance = enemy.GetDistanceTo(ship);

                            if (enemy.GetDockingStatus() == Ship.DockingStatus.Undocked)
                            {
                                if (distance < closestEnemyDistance)
                                {
                                    closestEnemy = enemy;
                                    closestEnemyDistance = distance;
                                }
                            }
                            else
                            {
                                if (distance < closestDockedEnemyDistance)
                                {
                                    closestDockedEnemyDistance = distance;
                                    closestDockedEnemy = enemy;
                                }
                            }
                        }
                    }

                    Move move = null;

                    //Decide whether to colonise or fight
                    if (closestNeutralPlanet != null && !planetLookup.ContainsKey(closestNeutralPlanet.GetId()))
                    {
                        planetLookup.Add(closestNeutralPlanet.GetId(), ship.GetId());

                        if (ship.CanDock(closestNeutralPlanet))
                        {
                            move = new DockMove(ship, closestNeutralPlanet);
                        }
                        else
                        {
                            move = Navigation.NavigateShipToDock(gameMap, ship, closestNeutralPlanet, Constants.MAX_SPEED);
                        }
                    }
                    else if (closestPlanet != null && !planetLookup.ContainsKey(closestPlanet.GetId()))
                    {
                        planetLookup.Add(closestPlanet.GetId(), ship.GetId());

                        if (ship.CanDock(closestPlanet))
                        {
                            move = new DockMove(ship, closestPlanet);
                        }
                        else
                        {
                            move = Navigation.NavigateShipToDock(gameMap, ship, closestPlanet, Constants.MAX_SPEED);
                        }
                    }
                    else if (closestDockedEnemy != null)
                    {
                        move = Navigation.NavigateShipTowardsTarget(gameMap, ship, closestDockedEnemy, Constants.MAX_SPEED, true, Constants.MAX_NAVIGATION_CORRECTIONS, Math.PI / 180);
                    }
                    else if (closestEnemy != null)
                    {
                        move = Navigation.NavigateShipTowardsTarget(gameMap, ship, closestEnemy, Constants.MAX_SPEED, true, Constants.MAX_NAVIGATION_CORRECTIONS, Math.PI / 180);
                    }

                    if (move != null)
                    {
                        moveList.Add(move);
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
