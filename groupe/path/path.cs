/*

Bot qui se promène de façon aléatoire
Il possède une gestion minimale de son bouclier de protection
Est et OUest échangé
 */

using System;
using BattleIA;


namespace explorateur
{
    public class explo2
    {

        // Pour faire des tirages de nombres aléatoires
        Random rnd = new Random();

        // Pour détecter si c'est le tout premier tour du jeu
        bool isFirstTime;

        // mémorisation du niveau du bouclier de protection
        UInt16 currentShieldLevel;
        // variable qui permet de savoir si le bot a été touché ou non
        bool hasBeenHit;
        UInt16 nbtour;
        int niveau;
        UInt16 localenergy;
        UInt16 energie;
        UInt16 energiebase = 100;
        UInt16 critiquenergie = 30;

        UInt16 murnord;
        UInt16 mursud;
        UInt16 murest;
        UInt16 murouest;
       
        
        
        List<MoveDirection> chemin = new List<MoveDirection>();

        // ****************************************************************************************************
        // Ne s'exécute qu'une seule fois au tout début
        // C'est ici qu'il faut initialiser le bot
        public void DoInit()
        {
            isFirstTime = true;
            currentShieldLevel = 0;
            hasBeenHit = false;
            nbtour = 0;
            
        }

        // ****************************************************************************************************
        /// Réception de la mise à jour des informations du bot
        public void StatusReport(UInt16 turn, UInt16 energy, UInt16 shieldLevel, UInt16 cloakLevel)
        {   
            nbtour = turn;
            energie = energy;
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
            // variable qui contient le taille de chemin 
            if (chemin.Count == 0)
            {
                return 5;
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
                                break;
                        }
                    }
                Console.WriteLine();
                }
                Console.WriteLine("--------------------------------");
                niveau = (distance - 1) / 2;
                murnord = 0;
                mursud = 0;
                murest = 0;
                murouest = 0;
                if (tab[niveau-1,niveau ] == 4){
                    murnord = 1;
                    Console.WriteLine("MUR au nord");
                }
                if (tab[niveau+1,niveau ] == 4){
                    mursud = 1;
                    Console.WriteLine("MUR au sud");
                }
                if (tab[niveau,niveau+1 ] == 4){
                    murest = 1;
                    Console.WriteLine("MUR à l'est");
                }
                if (tab[niveau,niveau-1 ] == 4){
                    murouest = 1;
                    Console.WriteLine("MUR à l'ouest");
                }
                else{
                    Console.WriteLine("Evaluation des mur fini");
                }
            /* //affichage du tableau
                for (int i = 0; i < distance; i++)
                {
                    for (int j = 0; j < distance; j++)
                    {
                        Console.Write(tab[i, j]);
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("--------------------------------");*/

            
                niveau = (distance - 1) / 2;
                for (int i = 0; i < distance; i++)
                {
                    for (int j = 0; j < distance; j++)
                    {
                    if (tab[i,j] == 2){
                        position[i,j] = 2;
                        
                    }
                        else if (tab[i,j] == 4){
                            position[i,j] = 4;
                        }
                    else{
                            position[i,j] = 0;    
                    }
                    }
                }

                // ENERGIE PLUS PROCHE
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

               
                    
           
        }

            


              
            
           
            






        // ****************************************************************************************************
        /// On doit effectuer une action
        public byte[] GetAction()
        {   
          return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));
            
            
     
            
        }
   }    

}
    
