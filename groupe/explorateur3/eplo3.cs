/*

Bot qui se promène de façon aléatoire
Il possède une gestion minimale de son bouclier de protection

using System; 
using BattleIA;


namespace BotRandom
{
    public class explo3
    {

        // Pour faire des tirages de nombres aléatoires
        Random rnd = new Random();

        // Pour détecter si c'est le tout premier tour du jeu
        bool isFirstTime;

        // mémorisation du niveau du bouclier de protection
        UInt16 currentShieldLevel;
        // variable qui permet de savoir si le bot a été touché ou non
        bool hasBeenHit;
        //List<Point> path = new List<Point>();
        List<MoveDirection> chemin = new List<MoveDirection>();

        // ****************************************************************************************************
        // Ne s'exécute qu'une seule fois au tout début
        // C'est ici qu'il faut initialiser le bot
        public void DoInit()
        {
            isFirstTime = true;
            currentShieldLevel = 0;
            hasBeenHit = false;
        }

        // ****************************************************************************************************
        /// Réception de la mise à jour des informations du bot
        public void StatusReport(UInt16 turn, UInt16 energy, UInt16 shieldLevel, UInt16 cloakLevel)
        {
            // Si le niveau du bouclier a baissé, c'est que l'on a reçu un coup
            if (currentShieldLevel != shieldLevel)
            {
                currentShieldLevel = shieldLevel;
                hasBeenHit = true;
            }
        }

        // Trouve la case la plus proche contenant de l'énergie
        private  FindNearestEnergy(byte distance, byte[] informations,currentPosition)
        {
            // Recherche toutes les cases contenant de l'énergie
            List<Point> energyLocations = new List<Point>();
            for (byte x = 0; x < distance; x++)
            {
                for (byte y = 0; y < distance; y++)
                {
                    byte index = (byte)(x * distance + y);
                    if (informations[index] == (byte)CaseState.Energy)
                    {
                        energyLocations.Add(new Point(x, y));
                    }
                }
            }

            // Si aucune case ne contient d'énergie, retourne null
            if (energyLocations.Count == 0)
            {
                return null;
            }

            // Trouve le chemin le plus court vers chaque case contenant de l'énergie
            Dictionary<Point, List<Point>> shortestPaths = new Dictionary<Point, List<Point>>();
            foreach (Point energyLocation in energyLocations)
            {
                List<Point> shortestPath = FindShortestPath(distance, informations, currentPosition, energyLocation);
                if (shortestPath != null)
                {
                    shortestPaths[energyLocation] = shortestPath;
                }
            }

            // Trouve la case la plus proche contenant de l'énergie
            Point nearestEnergyLocation = energyLocations[0];
            int shortestDistance = int.MaxValue;
            foreach (KeyValuePair<Point, List<Point>> kvp in shortestPaths)
            {
                int distance = kvp.Value.Count;
                if (distance < shortestDistance)
                {
                    nearestEnergyLocation = kvp.Key;
                    shortestDistance = distance;
                }
            }

            // Génère une liste qui déplace le robot vers nearestEnergyLocation en évitant les obstacles

            
            Point previousPoint = currentPosition;

            foreach (Point point in shortestPaths[nearestEnergyLocation])
            {
                if (point.X > previousPoint.X)
                {
                    //path.Add(Point(1, 0));
                    chemin.Add(MoveDirection.South);
                }
                else if (point.X < previousPoint.X)
                {
                    //path.Add(Point(-1, 0));
                    chemin.Add(MoveDirection.North);
                }
                else if (point.Y > previousPoint.Y)
                {
                    //path.Add(Point(0, 1));
                    chemin.Add(MoveDirection.West);
                }
                else if (point.Y < previousPoint.Y)
                {
                    //path.Add(Point(0, -1));
                    chemin.Add(MoveDirection.East);
                }
                previousPoint = point;
            }


            

            // Retourne la liste
            return chemin;

        }




        // ****************************************************************************************************
        /// On nous demande la distance de scan que l'on veut effectuer
        public byte GetScanSurface()
        {
            if (isFirstTime)
            {
                isFirstTime = false;
                // La toute première fois, le bot fait un scan d'une surface de 20 cases autour de lui
                return 20;
            }
            // Toutes les autres fois, le bot n'effectue aucun scan...
            return 0;
        }

        // ****************************************************************************************************
        /// Résultat du scan
        public void AreaInformation(byte distance, byte[] informations)
        {
            // Ici, on ne fait rien avec cette information...
            // on l'affiche juste dans la console...
            // C'est dommage... ;)
            Console.WriteLine($"Area: {distance}");
            int index = 0;
            for (int i = 0; i < distance; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    //Console.Write(informations[index++]);
                    switch ((CaseState)informations[index++])
                    {
                        case CaseState.Empty: Console.Write("·"); break;
                        case CaseState.Energy: Console.Write(""); break;
                        case CaseState.Ennemy: Console.Write("E"); break;
                        case CaseState.Wall: Console.Write("█"); break;
                    }
                }
                Console.WriteLine();
            }

            // On recherche la case la plus proche contenant de l'énergie
            Point nearestEnergyLocation = FindNearestEnergy(distance, informations, new Point(distance / 2, distance / 2));

            
        }

        // ****************************************************************************************************
        /// On doit effectuer une action
        public byte[] GetAction()
        {
            // Si le bot vient d'être touché
            if (hasBeenHit)
            {
                // Le bot a-t-il encore du bouclier ?
                if (currentShieldLevel == 0)
                {
                    // NON ! On s'empresse d'en réactiver un de suite !
                    currentShieldLevel = (byte)rnd.Next(1, 9);
                    return BotHelper.ActionShield(currentShieldLevel);
                }
                // oui, il reste du bouclier actif

                // On réinitialise notre flag
                hasBeenHit = false;
                // Puis on déplace fissa le bot, au hazard...
                return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));
                
            }

            // S'il n'y a pas de bouclier actif, on en active un
            if (currentShieldLevel == 0)
            {
                currentShieldLevel = 1;
                return BotHelper.ActionShield(currentShieldLevel);
            }

            if (chemin.Count > 0)
            {
                MoveDirection direction = chemin[0];
                chemin.RemoveAt(0);
                return BotHelper.ActionMove(direction);
            }
            else{
                // On déplace le bot au hazard
                return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));
            }
            


        }

    }
}
 */
