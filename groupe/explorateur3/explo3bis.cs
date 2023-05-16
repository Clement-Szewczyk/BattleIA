/*

Bot qui se promène de façon aléatoire
Il possède une gestion minimale de son bouclier de protection
Est et OUest échangé
 */

using System;
using BattleIA;


namespace explorateur3
{   

    public class ligne{
                
    }

    public class carte{

    }

    public class explo3bis
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
        // Tableau qui stocke les déplacements possibles
        bool[,] tabvisit;

        List<MoveDirection> deplacements = new List<MoveDirection>();

        bool rien = false;
        


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
                return 3;
            }
            if (rien == true){
                rien = false;
                return 5;
            }

            if (deplacements.Count == 0){
                return 3;
            }
            else{
             // ne scanne rien
             Console.WriteLine("RIEN");
            return 0;
           }
            
        }
        

        // ****************************************************************************************************
        /// Résultat du scan

        public void AreaInformation(byte distance, byte[] informations)
        {       
            save_distance = distance;
            bool[,] tabvisit = new bool[distance, distance];
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
                            tabvisit[i, j] = true;
                            break;
                        case CaseState.Wall: 
                            Console.Write("|"); 
                            tab[i, j] = 4;
                            
                            break;
                    }
                }
            Console.WriteLine();
            }
            // compte le nombre de 3 dans tab
            int nbennemie = 0;
            for (int i = 0; i < distance; i++){
                for (int j = 0; j < distance; j++){
                    if (tab[i, j] == 3){
                        nbennemie++;
                    }
                }
            }

            if (nbennemie >=1){
                Console.WriteLine("--------------------------------");
                
                niveau = (distance - 1) / 2;
                for (int i = 0; i < distance; i++)
                {
                    for (int j = 0; j < distance; j++)
                    {
                    if (tab[i,j] == 3){
                        position[i,j] = 3;
                        
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
                // ENERGIE PLUS PROCHE
                int posX = niveau; // position du robot en x
                int posY = niveau; // position du robot en y
                int distanceMin = 100; // distance minimale entre le robot et le 2
                int posenxMin = -1; // position x du 3 le plus proche
                int posenyMin = -1; // position y du 3 le plus proche

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

                // Génère une liste de déplacement possible

                Console.WriteLine("Distance minimale = " + distanceMin);
                Console.WriteLine("Position du 2 le plus proche = (" + posxMin + ", " + posyMin + ")");
                Console.WriteLine("--------------------------------");

               
                
                int limite = 0;
                
                // vide la liste de déplacement
                deplacements.Clear();

                while (limite<distanceMin){
                    // Analyse déplacement vers le SUD
                    if (posxMin > posX) {
                        //SUD
                        if (tabvisit[posX+1,posY] == true){
                            if (tabvisit[posX, posY+1 ] == true){
                                if(tabvisit[posX, posY-1 ] == true){
                                    Console.WriteLine("SUD-EST-OUEST_bloqué");
                                    posX = posX - 1;
                                    tabvisit[posX, posY] = true;
                                    deplacements.Add(MoveDirection.North);
                                    break;
                                    
                                } 
                                else{
                                    Console.WriteLine("SUD-EST_bloqué");
                                    posY = posY - 1;
                                    tabvisit[posX, posY] = true;
                                    deplacements.Add(MoveDirection.East);
                                    break;
                                    
                                }
                            }
                            else{
                                Console.WriteLine("SUD_bloqué");
                                posY = posY + 1;
                                tabvisit[posX, posY] = true;
                                deplacements.Add(MoveDirection.West);
                                break;
                                
                            }
                        }
                        else{
                            posX = posX + 1;
                            tabvisit[posX, posY] = true;
                            deplacements.Add(MoveDirection.South);
                            break;
                            
                        }
                        
                    }
                    // Analyse déplacement vers le nord
                    if (posxMin < posX) {
                        //NORD
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
                        //EST

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
                        //OUEST
        
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
        }


        // ****************************************************************************************************
        /// On doit effectuer une action
        public byte[] GetAction()
        {   
           // lire la liste chemin et effectuer l'action correspondante
            if (deplacements.Count > 0) {
                Console.WriteLine("direction de liste : ");
                MoveDirection direction = deplacements[0];
                deplacements.RemoveAt(0);
                return BotHelper.ActionMove(direction);
            }
            if (rien = true){
                return BotHelper.ActionNone();
            }
            else{
                return BotHelper.ActionMove(MoveDirection.East);
            }
            
            
     
            
        }
   }    

}
    
