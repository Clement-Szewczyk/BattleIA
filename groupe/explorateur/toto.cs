/*

Bot qui se promène de façon aléatoire
Il possède une gestion minimale de son bouclier de protection
Est et OUest échangé
 */

using System;
using BattleIA;


namespace pathfinder
{   

    
    public class path
    {

        // Pour faire des tirages de nombres aléatoires
        Random rnd = new Random();

        // Pour détecter si c'est le tout premier tour du jeu
        bool isFirstTime;

        // mémorisation du niveau du bouclier de protection
        UInt16 currentShieldLevel;
        // variable qui permet de savoir si le bot a été touché ou non
        bool hasBeenHit;
        




        // ****************************************************************************************************
        // Tableau qui stocke les cases visitées
        bool[,] tabvisit;

        // Liste des déplacements à effectuer
        List<MoveDirection> deplacements = new List<MoveDirection>();
        // variable qui servira a caluler le niveau du scan 
        int niveau ;
        // Variable qui permetra de savoir s'il des énergie
        bool rien = false;
       
        List<(int, int) > positionEnnemie = new List<(int, int)>();
        
        int directennemie;

        // ****************************************************************************************************
        // Ne s'exécute qu'une seule fois au tout début
        // C'est ici qu'il faut initialiser le bot
        public void DoInit()
        {
            isFirstTime = true;
            currentShieldLevel = 0;
            hasBeenHit = false;
;
            
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


        // ****************************************************************************************************
        /// On nous demande la distance de scan que l'on veut effectuer
        public byte GetScanSurface()
        {
            if (isFirstTime)
            {
                isFirstTime = false;
                // La toute première fois, le bot fait un scan d'une surface de 20 cases autour de lui
                return 5;
            }
            // Scan de 5 s'il n'y a pas d'énergie de détecté
            if (rien == true){
                rien = false;
                return 5;
            }
            // Scan de 2 quand on a fini la liste
            if (deplacements.Count == 0){
                return 4;
            }
            else{
             // ne scanne rien
            return 0;
           }
            
        }
        

        // ****************************************************************************************************
        /// Résultat du scan

        public void AreaInformation(byte distance, byte[] informations)
        {       
            // On initialise le tableau des cases visitées
            bool[,] tabvisit = new bool[distance, distance];
            // On deux tableaux pour stocker les informations du scan
            UInt16[,] tab = new UInt16[distance, distance];
            UInt16[,] position = new UInt16[distance, distance];
            UInt16[,] ennemie = new UInt16[distance, distance];
            Console.WriteLine($"Area: {distance}");
            int index = 0;
            for (int i = 0; i < distance; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    //Console.Write(informations[index++]);
                    switch ((CaseState)informations[index++])
                    {
                        case CaseState.Empty: 
                            Console.Write("."); 
                            tab[i, j] = 1;
                            break;
                        case CaseState.Energy:
                            Console.Write("L"); 
                            tab[i, j] = 2;
                            break;
                        case CaseState.Ennemy:
                            Console.Write("E");
                            tab[i, j] = 3;
                            ennemie[i, j] = 3;
                            break;
                        case CaseState.Wall: 
                            Console.Write("|"); 
                            tab[i, j] = 4;
                            tabvisit[i, j] = true;
                            break;
                    }
                }
            Console.WriteLine();
            }
            // compte le nombre de 2 dans tab
            int nbenergie = 0;
            for (int i = 0; i < distance; i++){
                for (int j = 0; j < distance; j++){
                    if (tab[i, j] == 2){
                        nbenergie++;
                    }
                }
            }

            if (nbenergie >=1){
                Console.WriteLine("--------------------------------");
                // On calcul le niveau du scan
                niveau = (distance - 1) / 2;
    
                // On remplie le tableau position
                for (int i = 0; i < distance; i++)
                {
                    for (int j = 0; j < distance; j++)
                    {
                    if (tab[i,j] == 2){
                        position[i,j] = 2;
                        
                    }
                    else if (tab[i,j] == 4){
                        position[i,j] = 4;
                        tabvisit[i,j] = true;
                    }
                    else{
                            position[i,j] = 0;    
                        }
                    }
                }
                

                Console.WriteLine("--------------------------------");
                // ****************************************************************************************************
                // Recherche de l'énergie la plus proche
                int posX = niveau; // position du robot en x
                int posY = niveau; // position du robot en y
                int distanceMin = 100; // distance minimale entre le robot et le 2
                int posxMin = -1; // position x du 2 le plus proche
                int posyMin = -1; // position y du 2 le plus proche

                // on parcourt le tableau position
                for (int i = 0; i < position.GetLength(0); i++) {
                    for (int j = 0; j < position.GetLength(1); j++) {
                        if (position[i, j] == 2) { // si on trouve un 2
                            // on calcule la distance entre le 2 et le robot
                            int ecart = Math.Abs(i - posX) + Math.Abs(j - posY);
                        // Console.WriteLine("Distance entre (" + i + ", " + j + ") et (" + posX + ", " + posY + ") = " + ecart);
                            if (ecart < distanceMin) { // si la distance est plus petite que la distance minimale
                                distanceMin = ecart; // on met à jour la distance minimale
                                posxMin = i; // on met à jour la position x du 2 le plus proche
                                posyMin = j; // on met à jour la position y du 2 le plus proche
                            }   
                        }         
                    }
                }

                Console.WriteLine("Distance minimale = " + distanceMin);
                Console.WriteLine("Position du 2 le plus proche = (" + posxMin + ", " + posyMin + ")");
                Console.WriteLine("--------------------------------");

                // ****************************************************************************************************
                int limite = 0;
                
                // vide la liste de déplacement
                deplacements.Clear();

                while (limite<distanceMin){
                    // Analyse déplacement vers le SUD
                    if (posxMin > posX) {
                        // On vérifie s'il y a des murs autour de la position du robot
                        if (tabvisit[posX+1,posY] == true){
                            if (tabvisit[posX, posY+1 ] == true){
                                if(tabvisit[posX, posY-1 ] == true){
                                    Console.WriteLine("SUD-EST-OUEST_bloqué");
                                    posX = posX - 1;
                                    tabvisit[posX, posY] = true;
                                    // Déplacement vers le nord
                                    deplacements.Add(MoveDirection.North);
                                    break;
                                    
                                } 
                                else{
                                    Console.WriteLine("SUD-EST_bloqué");
                                    posY = posY - 1;
                                    tabvisit[posX, posY] = true;
                                    // Déplacement vers l'ouest
                                    deplacements.Add(MoveDirection.East);
                                    break;
                                    
                                }
                            }
                            else{
                                Console.WriteLine("SUD_bloqué");
                                posY = posY + 1;
                                tabvisit[posX, posY] = true;
                                // Déplacement vers l'est
                                deplacements.Add(MoveDirection.West);
                                break;
                                
                            }
                        }
                        else{
                            posX = posX + 1;
                            tabvisit[posX, posY] = true;
                            // Déplacement vers le sud
                            deplacements.Add(MoveDirection.South);
                            break;
                            
                        }
                        
                    }
                    // Analyse déplacement vers le nord
                    if (posxMin < posX) {
                        // On vérifie s'il y a des murs autour de la position du robot
                        if (tabvisit[posX-1,posY] == true){
                            if (tabvisit[posX, posY+1 ] == true){
                                if(tabvisit[posX, posY-1 ] == true){
                                    Console.WriteLine("NORD-EST-OUEST_bloqué");
                                    posX = posX - 1;
                                    tabvisit[posX, posY] = true;
                                    deplacements.Add(MoveDirection.South);
                                    break;
                                    
                                } 
                                else{
                                    Console.WriteLine("NORD-EST_bloqué");
                                    posY = posY - 1;
                                    tabvisit[posX, posY] = true;
                                    deplacements.Add(MoveDirection.East);
                                    break;
                                }
                            }
                            else{
                                Console.WriteLine("NORD_bloqué");
                                posY = posY + 1;
                                tabvisit[posX, posY] = true;
                                deplacements.Add(MoveDirection.West);
                                break;
                            }
                                
                                
                        }
                        else{
                            posX = posX - 1;
                            tabvisit[posX, posY] = true;
                            deplacements.Add(MoveDirection.North);
                            break;
                        }

                    }
                    // Analys déplacement vers l'Est
                    if (posyMin > posY) {
                        // On vérifie s'il y a des murs autour de la position du robot
                        if (tabvisit[posX,posY+1] == true){
                            if (tabvisit[posX-1, posY] == true){
                                if(tabvisit[posX+1, posY ] == true){
                                    Console.WriteLine("EST-NORD-SUD_bloqué");
                                    posY = posY - 1;
                                    tabvisit[posX, posY] = true;
                                    deplacements.Add(MoveDirection.East);
                                    break;
                                } 
                                else{
                                    Console.WriteLine("EST-NORD_bloqué");
                                    posX = posX + 1;
                                    tabvisit[posX, posY] = true;
                                    deplacements.Add(MoveDirection.South);
                                    break;
                                }
                            }
                            else{
                                Console.WriteLine("EST_bloqué");
                                posX = posX - 1;
                                tabvisit[posX, posY] = true;
                                deplacements.Add(MoveDirection.North);
                                break;
                            }
                        }
                        else{
                            posY = posY + 1;
                            tabvisit[posX, posY] = true;
                            deplacements.Add(MoveDirection.West);
                            break;
                        }

                        
                    }
                    // Analyse déplacement vers l'Ouest
                    if (posyMin < posY) {
                        // On vérifie s'il y a des murs autour de la position du robot
                        if (tabvisit[posX,posY-1] == true){
                            if (tabvisit[posX-1, posY] == true){
                                if(tabvisit[posX+1, posY ] == true){
                                    Console.WriteLine("OUEST-NORD-SUD_bloqué");
                                    posY = posY + 1;
                                    tabvisit[posX, posY] = true;
                                    deplacements.Add(MoveDirection.West);
                                    break;
                                } 
                                else{
                                    Console.WriteLine("OUEST-NORD_bloqué");
                                    posX = posX + 1;
                                    tabvisit[posX, posY] = true;
                                    deplacements.Add(MoveDirection.South);
                                    break;
                                }
                            }
                            else{
                                Console.WriteLine("OUEST_bloqué");
                                posX = posX - 1;
                                tabvisit[posX, posY] = true;
                                deplacements.Add(MoveDirection.North);
                                break;
                            }
                        }
                        else{
                            posY = posY - 1;
                            tabvisit[posX, posY] = true;
                            deplacements.Add(MoveDirection.East);
                            break;
                        }

                    }
                    limite = limite + 1;
                }

                Console.WriteLine("Liste des déplacements possibles :");
                foreach (MoveDirection deplacement in deplacements) {
                    Console.WriteLine(deplacement);
                }
                Console.WriteLine("--------------------------------");
                
               
            }
            else{
                rien = true;
            }

            //affichage du tableau des visites
            for (int i = 0; i < distance; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    Console.Write(tabvisit[i, j]);
                }
                Console.WriteLine();
            }
            
            // remettre à zéro le tableau tabvisit
            for (int i = 0; i < distance; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    tabvisit[i,j] = false;
                }      
        
            }

            // Recherche de l'ennemi
            positionEnnemie.Clear();
            int posXen = niveau; // position du robot en x
            int posYen = niveau; // position du robot en y
            int distanceMinen = 100; // distance minimale entre le robot et le 2
            int posxMinen = -1; // position x du 2 le plus proche
            int posyMinen = -1; // position y du 2 le plus proche

            // on parcourt le tableau ennemie
            for (int i = 0; i < ennemie.GetLength(0); i++) {
                for (int j = 0; j < ennemie.GetLength(1); j++) {
                    if (ennemie[i, j] == 3) { // si on trouve un 2
                       
                            positionEnnemie.Add((i,j));
                            Console.WriteLine("ennemie");
            
                                     
                    }         
                }
            }

            foreach (var pos in positionEnnemie)
            {
                Console.WriteLine($"i = {pos.Item1}, j = {pos.Item2}");
                // regarde si pos est au nord, sud, est ou ouest

                if (pos.Item1 < posXen & pos.Item2 == posYen) {
                    Console.WriteLine("NORD");
                    directennemie = 1;
                }
                if (pos.Item1 > posXen & pos.Item2 == posYen) {
                    Console.WriteLine("SUD");
                    directennemie = 2;
                }
                if (pos.Item1 == posXen & pos.Item2 < posYen) {
                    Console.WriteLine("EST");
                    directennemie = 3;
                }
                if (pos.Item1 == posXen & pos.Item2 > posYen) {
                    Console.WriteLine("OUEST");
                    directennemie = 4;
                }
                else{
                    Console.WriteLine("rien");
                }


            }

            

           






                      


        }
    


        // ****************************************************************************************************
        /// On doit effectuer une action
        public byte[] GetAction()
        {  
            Console.WriteLine("ENNEEEEEEEEEEEEEEEEEEEEEEEEMIE: "+directennemie);
            if (directennemie ==0){
                // Si la liste contient plus d'un élément
                if (deplacements.Count > 0) {
                    Console.WriteLine("direction de liste : ");
                    MoveDirection direction = deplacements[0];
                    deplacements.RemoveAt(0);
                    return BotHelper.ActionMove(direction);
                }
                if (rien == true){
                    // On ne fait rien s'il n'y a pas d'énergie
                    return BotHelper.ActionNone();
                }
                else{
                return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));
                }
            }
            if (directennemie == 1){
                directennemie = 0;
                return BotHelper.ActionShoot(MoveDirection.North);
            }
            if (directennemie == 2){
                directennemie = 0;
                return BotHelper.ActionShoot(MoveDirection.South);
            }
            if (directennemie == 3){
                directennemie = 0;
                return BotHelper.ActionShoot(MoveDirection.East);
            }
            if (directennemie == 4){
                directennemie = 0;
                return BotHelper.ActionShoot(MoveDirection.West);
            }
            else{
                directennemie = 0;
                return BotHelper.ActionNone();
            }
           
        }
   }    

}
    
