/*

Bot qui se promène de façon aléatoire
Il possède une gestion minimale de son bouclier de protection
Est et OUest échangé
 */

using System;
using BattleIA;


namespace discretbot
{   

    public class discret
    {

        // Pour faire des tirages de nombres aléatoires
        Random rnd = new Random();

        // Pour détecter si c'est le tout premier tour du jeu
        bool isFirstTime;

        // mémorisation du niveau du bouclier de protection
        UInt16 currentShieldLevel;
        // variable qui permet de savoir si le bot a été touché ou non
        bool hasBeenHit;
        // mémorisation du voile d'invisibilité
        UInt16 currentinvisibility;
        




        // ****************************************************************************************************
        // Tableau qui stocke les cases visitées
        bool[,] tabvisit;

        // Liste des déplacements à effectuer
        List<MoveDirection> deplacements = new List<MoveDirection>();
        // variable qui servira a caluler le niveau du scan 
        int niveau ;
        // Variable qui permetra de savoir s'il des énergie
        bool rien = false;
     
        // ****************************************************************************************************
        // Ne s'exécute qu'une seule fois au tout début
        // C'est ici qu'il faut initialiser le bot
        public void DoInit()
        {
            isFirstTime = true;
            currentShieldLevel = 0;
            hasBeenHit = false;
            currentinvisibility = 0;
            
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
                //Console.Writeline("Je me suis pris un dégât et j'ai perdu 1 niveau de bouclier!");
            }
            if(currentinvisibility != cloakLevel)
            {
                currentinvisibility = cloakLevel;
                hasBeenHit = true ;
                //Console.Writeline("Je me suis pris un dégât et je n'ai plus d'invisibilité!");
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
                return 3;
            }
            // Scan de 5 s'il n'y a pas d'énergie de détecté
            if (rien == true){
                rien = false;
                return 5;
            }
            // Scan de 2 quand on a fini la liste
            if (deplacements.Count == 0){
                return 2;
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

            
            // remettre à zéro le tableau tabvisit
            for (int i = 0; i < distance; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    tabvisit[i,j] = false;
                }      
        
            }
        }
    


        // ****************************************************************************************************
        
            public byte[] GetAction()
            {   // S'il n'a pas pris de dégat 
                if (hasBeenHit==false)
                    {   
                        // On remet le mode invisible
                        if (currentinvisibility == 0)
                        {
                            currentinvisibility = 1;
                            return BotHelper.ActionCloak(currentinvisibility);
                        }
                        // On remet le mode bouclier
                        else if (currentShieldLevel == 0)
                        {
                            currentShieldLevel = 5;
                            return BotHelper.ActionShield(currentShieldLevel);
                        }
                    }
                // S'il y a des déplacements à effectuer
                else if (deplacements.Count > 0)
                {
                    Console.WriteLine("Direction de liste : ");
                    MoveDirection direction = deplacements[0];
                    deplacements.RemoveAt(0);
                    return BotHelper.ActionMove(direction);
                }
                else if (rien)
                {
                    return BotHelper.ActionNone();
                }
                

                return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));
            }

        }    
    }
    
