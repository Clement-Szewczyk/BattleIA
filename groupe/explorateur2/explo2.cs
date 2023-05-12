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
        int nbtour;
        int niveau;
        int localenergy;
        int energie;
        int critiquenergie = 30;

        

        int murnord;
        int mursud;
        int murest;
        int murouest;

        byte save_distance;
        byte[] save_informations;
        bool aleatoire = false;

        bool detectenemy = false;

        
        
        
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

            //savoir le type de déplace précédent



           
            

            if (energy < critiquenergie)
            {
                Console.WriteLine("Je vais chercher de l'énergie");
            }
        }


        // ****************************************************************************************************
        /// On nous demande la distance de scan que l'on veut effectuer
        public byte GetScanSurface()
        {
            if (isFirstTime)
            {
                isFirstTime = false;
                
                return 3;
            }
            // variable qui contient le taille de chemin 
            if (chemin.Count == 0)
            {
                
                return 2;
            }
            if (aleatoire == true){
                
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
            int[,] tab = new int[distance, distance];
            int[,] position = new int[distance, distance];
            
            // Ici, on ne fait rien avec cette information...
            // on l'affiche juste dans la console...
            // C'est dommage... ;)
            
            // si c'est un scan de plus  1 case

            
            save_distance = distance;
            
            save_informations = informations;
           

            if (distance > 1){
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
                                detectenemy = true;
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

                //Regarde ce qui a autour de l'énergie
                if (posxMin != -1 && posyMin != -1) {
                    if (position[posxMin + 1, posyMin] == 4) {
                        mursud = 1;
                    }
                    if (position[posxMin - 1, posyMin] == 4) {
                        murnord = 1;
                    }
                    if (position[posxMin, posyMin + 1] == 4) {
                        murest = 1;
                    }
                    if (position[posxMin, posyMin - 1] == 4) {
                        murouest = 1;
                    }
                }
                else{
                    Console.WriteLine("Pas d'énergie");
                }

                if (mursud == 1 && murest == 1 && murouest == 1 && murnord == 1){
                    Console.WriteLine("MUR PARTOUT");
                    // surprime l'énergie du tableau position et refait une recherche 
                    position[posxMin, posyMin] = 0;
                    AreaInformation(save_distance, save_informations);
                }
                else{
                    Console.WriteLine("Pas de mur partout");
                }

                // Génération d'une liste de déplacements pour aller vers le 2 le plus proche 
                // (on suppose que le robot est au centre du tableau)
                chemin.Clear();
                if (posxMin != -1 && posyMin != -1) {
                    // on se déplace en x
                    if (posxMin > posX) {
                        if (mursud == 0){
                            for (int i = 0; i < posxMin - posX; i++) {
                                chemin.Add(MoveDirection.South);
                            }
                        }
                        else if(mursud == 1){
                            if (murest == 0){
                                for (int i = 0; i < posxMin - posX; i++) {
                                    chemin.Add(MoveDirection.West);
                                }
                            }
                            else if (murouest == 1){
                                for (int i = 0; i < posxMin - posX; i++) {
                                    chemin.Add(MoveDirection.East);
                                }
                            }
                            else{
                                for (int i = 0; i < posxMin - posX; i++) {
                                    chemin.Add(MoveDirection.North);
                                }
                            }
                            
                        }
                        
                    }
                    else if (posxMin < posX) {
                        
                        if (murnord == 0){
                            for (int i = 0; i < posX - posxMin; i++) {
                                chemin.Add(MoveDirection.North);
                            }
                        }
                        
                        else if (murnord == 1){
                            if (murest == 0){
                                for (int i = 0; i < posX - posxMin; i++) {
                                    chemin.Add(MoveDirection.East);
                                }
                            }
                            else if (murouest == 0){
                                for (int i = 0; i < posX - posxMin; i++) {
                                    chemin.Add(MoveDirection.West);
                                }
                            }
                            else{
                                for (int i = 0; i < posX - posxMin; i++) {
                                    chemin.Add(MoveDirection.South);
                                }
                            }
                        }
                    }
                    // on se déplace en y
                    else if (posyMin > posY) {
                        
                        if (murest == 0)
                        {
                            for (int i = 0; i < posyMin - posY; i++) {
                                chemin.Add(MoveDirection.West);
                            }
                        }
                        else if (murest == 1){
                            if (mursud == 0){
                                for (int i = 0; i < posyMin - posY; i++) {
                                    chemin.Add(MoveDirection.South);
                                    
                                }
                            }
                            else if (murnord == 1){
                                for (int i = 0; i < posyMin - posY; i++) {
                                    chemin.Add(MoveDirection.North);
                                }
                            }
                            else{
                                for (int i = 0; i < posyMin - posY; i++) {
                                    chemin.Add(MoveDirection.East);
                                }
                            }
                        }
                        
                            
                        
                    }
                    else if (posyMin < posY) {
                        if (murouest == 0){
                            for (int i = 0; i < posY - posyMin; i++) {
                                chemin.Add(MoveDirection.East);
                            }
                        }
                        else if (murouest == 1){
                            if (mursud == 0){
                                for (int i = 0; i < posY - posyMin; i++) {
                                    chemin.Add(MoveDirection.South);
                                }
                            }
                            else if (murnord == 0){
                                for (int i = 0; i < posY - posyMin; i++) {
                                    chemin.Add(MoveDirection.North);
                                }
                            }
                            else{
                                for (int i = 0; i < posY - posyMin; i++) {
                                    chemin.Add(MoveDirection.West);
                                }
                            }
                        }
                    }
                }
                else {
                    Console.WriteLine("Pas de 2 à proximité");

                }




            }
            else{

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
                
            }
        }
        
        // ****************************************************************************************************
        /// On doit effectuer une action
        public byte[] GetAction()
        {   
           
           /*if (detectenemy){
                
                if (currentShieldLevel == 0)
                {
                    // NON ! On s'empresse d'en réactiver un de suite !
                    currentShieldLevel = 1;
                    return BotHelper.ActionShield(currentShieldLevel);
                }
           }*/
            
            // aficher la liste Chemin
            Console.WriteLine("Chemin : ");
            foreach (MoveDirection direction in chemin) {
                Console.Write(direction + " ");
            }
            // lire la liste chemin et effectuer l'action correspondante
            if (chemin.Count > 0) {
                Console.WriteLine("direction de liste : ");
                MoveDirection direction = chemin[0];
                chemin.RemoveAt(0);
                return BotHelper.ActionMove(direction);
            }
            else {
                Console.WriteLine("direction aléatoire : ");
                aleatoire = true;
                return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));
            }
            
     
            
        }
   }    

}
    
