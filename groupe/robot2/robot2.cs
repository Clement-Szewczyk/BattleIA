/*

Bot qui se promène de façon aléatoire
Il possède une gestion minimale de son bouclier de protection

 */

using System; 
using BattleIA;


namespace robot2
{
    public class robot2
    {

        // Pour faire des tirages de nombres aléatoires
        Random rnd = new Random();

        // Pour détecter si c'est le tout premier tour du jeu
        bool isFirstTime;

        // mémorisation du niveau du bouclier de protection
        UInt16 currentShieldLevel;

        // mémorisation du voile d'invisibilité
        UInt16 currentinvisibility;
        // variable qui permet de savoir si le bot a été touché ou non
        bool hasBeenHit;

        UInt16 nbtour;
        int niveau;
        UInt16 localenergy;
        UInt16 energie;
        UInt16 energiebase = 100;
        UInt16 critiquenergie = 30;
        UInt16 mur2;

        UInt16 murnord;
        UInt16 mursud;
        UInt16 murest;
        UInt16 murouest;
        UInt16 murpos;
        
        UInt16 nb = 0;

        int mur;
        int doublemur = 0;

        // ****************************************************************************************************
        // Ne s'exécute qu'une seule fois au tout début
        // C'est ici qu'il faut initialiser le bot
        public void DoInit()
        {
            isFirstTime = true;
            currentShieldLevel = 0;
            hasBeenHit = false;
            currentinvisibility = 0;
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
            if(currentinvisibility != cloakLevel)
            {
                currentinvisibility = cloakLevel;
                hasBeenHit = true ;
            }


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
                // La toute première fois, le bot fait un scan d'une surface de 20 cases autour de lui
                return 3;
            }
            if (nbtour % 2 == 0 && energie > critiquenergie)
            {
                return 5;
            }
            else if (energie < critiquenergie)
            {
                return 3;
            }
            else
            {
                return 3;
            }
        }

        // ****************************************************************************************************
        /// Résultat du scan
        public void AreaInformation(byte distance, byte[] informations)
        {
            UInt16[,] tab = new UInt16[distance, distance];
            UInt16[,] position = new UInt16[distance, distance];
            UInt16[,] mur = new UInt16[distance, distance];
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
            
            // donne le nombre de répétition de la valeur 3 (ennemi) dans le tableau tab
            for (int i = 0; i < distance; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    if (tab[i, j] == 3)
                    {
                        nb++;
                    }
                }
            }

            Console.WriteLine(nb);
            Console.WriteLine("--------------------------------");
            //affichage du tableau
            for (int i = 0; i < distance; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    Console.Write(tab[i, j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine("--------------------------------");

           
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
                murpos = 0;
            }
            if (posxMin != -1 && posyMin != -1) {
                
                //Console.WriteLine("La position la plus proche de (3, 3) est : (" + posxMin + ", " + posyMin + ")");
                // Faire en sorte que le robot se dirige vers les coordonnées (posxMin, posyMin)
                if (posxMin > posX) {
                    
                    if(posyMin > posY && murest == 0){
                            Console.WriteLine("Energie à l'est");
                            localenergy = 3;
                        }
                        if(posyMin < posY && murouest == 0){
                            Console.WriteLine("Energie à l'ouest");
                            localenergy = 4;
                        }
                    if (mursud == 1){
                        if (murest == 1){
                            Console.WriteLine("Energie à l'ouest");
                            localenergy = 4;
                        }
                        if (murouest == 1){
                            Console.WriteLine("Energie à l'est");
                            localenergy = 3;
                        }
                    }
                    else{
                        Console.WriteLine("Energie au sud");
                        localenergy = 2;
                    }                   
                }
                
                
                if (posxMin < posX) {
                    if (posyMin > posY) {
                        Console.WriteLine("Energie à l'est");
                        localenergy = 3;
                        
                    }
                    if (posyMin < posY) {
                        Console.WriteLine("Energie à l'ouest");
                        localenergy = 4;
                        
                    }
                    if (murnord == 1){
                        if (murest == 1){
                            Console.WriteLine("Energie à l'ouest");
                            localenergy = 4;
                        }
                        if (murouest == 1){
                            Console.WriteLine("Energie à l'est");
                            localenergy = 3;
                        }
                    }
                    else{
                        Console.WriteLine("Energie au nord");
                        localenergy = 1;
                    }
                    
                }
                if (posxMin == posX){
                    if (posyMin > posY) {
                        Console.WriteLine("Energie à l'est");
                        localenergy = 3;
                       
                    }
                    if (posyMin < posY) {
                        Console.WriteLine("Energie à l'ouest");
                        localenergy = 4;
                       
                    }
                }
            } else {
                Console.WriteLine("PAS D'ENERGIE");
            }
        }


        // ****************************************************************************************************
        /// On doit effectuer une action
        public byte[] GetAction()
        {
            // Si le bot vient d'être touché
            if (hasBeenHit==true)
            {
                // Le bot a-t-il encore du bouclier ?
                if (currentShieldLevel == 0)
                {
                    // NON ! On s'empresse d'en réactiver un de suite !
                    currentShieldLevel = (byte)rnd.Next(1, 9);
                    return BotHelper.ActionShield(currentShieldLevel);
                }
                // oui, il reste du bouclier actif
                if (nb)
                // On réinitialise notre flag
                hasBeenHit = false;
                // Puis on déplace fissa le bot, au hazard...
                return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));

                /* Le bot a-t-il encore l'invisibilité ?
                if (currentinvisibility == 0)
                {
                    // NON ! On s'empresse de le réactiver de suite !
                    currentinvisibility = (byte)rnd.Next(1, 9);
                    return BotHelper.ActionCloak(currentinvisibility);
                }
                // oui, il reste de l'invisibilité
                // On réinitialise notre flag
                hasBeenHit = false;
                // Puis on déplace fissa le bot, au hazard...
                return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));
    

            /*
                Explications :
                    rnd.Next(1, 5)   : tire un nombre aléatoire entre 1 (inclus) et 5 (exclu), donc 1, 2, 3 ou 4
                    (MoveDirection)x : converti 'x' en type MoveDirection
                    sachant que 1 = North, 2 = West, 3 = South et 4 = East
                 */
            }
            if (hasBeenHit == false && nb >= 2) 
            {
            // Le bot a-t-il encore de l'invisibilité ?
                if (currentinvisibility == 0)
                {
                    // NON ! On s'empresse de l'activer de suite !
                    currentinvisibility = (byte)rnd.Next(1, 9);
                    return BotHelper.ActionCloak(currentinvisibility);
                }
                // oui, il reste de l'invisibilité
                // On réinitialise notre flag
                // Puis on déplace fissa le bot, au hazard...
                return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));
              
            }
             // Tir dans la direction sud
            if (nb >= 2)
            {
                return BotHelper.ActionShoot((MoveDirection)rnd.Next(1, 5));
            }

            // 
            // On réinitialise notre flag
            hasBeenHit = false;
            // Puis on déplace fissa le bot, au hazard...
            return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));


            // S'il n'y a pas de bouclier actif, on en active un
            if (currentShieldLevel == 0)
            {
                currentShieldLevel = 1;
                return BotHelper.ActionShield(currentShieldLevel);
            }

            if (currentinvisibility == 0)
            {
                currentinvisibility = 1;
                return BotHelper.ActionCloak(1);
            }
            
            Console.WriteLine("localenergy = " + localenergy);
            
            if (localenergy == 1 && murnord == 0 ){
                localenergy = 0;
                Console.WriteLine(" direction nord");
                return BotHelper.ActionMove(MoveDirection.North);
                
            } 
            if (localenergy ==1 && murnord == 1){

               if (murest == 0){
                   localenergy = 0;
                   Console.WriteLine("direction est");
                   return BotHelper.ActionMove(MoveDirection.West);
               }
               if (murouest == 0){
                   localenergy = 0;
                   Console.WriteLine("direction ouest");
                   return BotHelper.ActionMove(MoveDirection.East);
               }
               else{
                    return BotHelper.ActionMove(MoveDirection.South);
               }
            }
            if (localenergy == 2 && mursud == 0 ){
                localenergy = 0;
                Console.WriteLine("direction sud");
                return BotHelper.ActionMove(MoveDirection.South);
            }
            if (localenergy == 2 && mursud == 1){
                if (murest == 0){
                     localenergy = 0;
                     Console.WriteLine("direction est");
                     return BotHelper.ActionMove(MoveDirection.West);
                }
                if (murouest == 0){
                     localenergy = 0;
                     Console.WriteLine("direction ouest");
                     return BotHelper.ActionMove(MoveDirection.East);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.North);
                }
            }
            if (localenergy == 4 && murouest == 0 ){
                localenergy = 0;
                Console.WriteLine(" direction ouest");
                return BotHelper.ActionMove(MoveDirection.East);
            }
            if (localenergy == 4 && murouest == 1){
                if (mursud == 0){
                     localenergy = 0;
                     Console.WriteLine(" direction sud");
                     return BotHelper.ActionMove(MoveDirection.South);
                }
                if (murnord == 0){
                     localenergy = 0;
                     Console.WriteLine("direction nord");
                     return BotHelper.ActionMove(MoveDirection.North);
                }
                else{
                      return BotHelper.ActionMove(MoveDirection.West);
                }
            }
            if (localenergy == 3 && murest == 0 ){
                localenergy = 0;
                Console.WriteLine("direction est");
                
                return BotHelper.ActionMove(MoveDirection.West);
            }
            if (localenergy == 3 && murest == 1){
                if (mursud == 0){
                     localenergy = 0;
                     Console.WriteLine("direction sud");
                     return BotHelper.ActionMove(MoveDirection.South);
                }
                if (murnord == 0){
                     localenergy = 0;
                     Console.WriteLine("direction nord");
                     return BotHelper.ActionMove(MoveDirection.North);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.East);
                }
            }
            if (murnord == 1){
                if (murest == 0){
                    return BotHelper.ActionMove(MoveDirection.West);
                }
                if (murouest == 0){
                    return BotHelper.ActionMove(MoveDirection.East);
                }
                if (murest == 1 || murouest == 1){
                    return BotHelper.ActionMove(MoveDirection.South);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.South);
                }
            }
            if (mursud == 1){
                if (murest == 0){
                    return BotHelper.ActionMove(MoveDirection.West);
                }
                if (murouest == 0){
                    return BotHelper.ActionMove(MoveDirection.East);
                }
                if (murest == 1 || murouest == 1){
                    return BotHelper.ActionMove(MoveDirection.North);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.North);
                }
            }
            if (murest == 1){
                if (mursud == 0){
                    return BotHelper.ActionMove(MoveDirection.South);
                }
                if (murnord == 0){
                    return BotHelper.ActionMove(MoveDirection.North);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.East);
                }
            }
            if (murouest == 1){
                if (mursud == 0){
                    return BotHelper.ActionMove(MoveDirection.South);
                }
                if (murnord == 0){
                    return BotHelper.ActionMove(MoveDirection.North);
                }
                else{
                    return BotHelper.ActionMove(MoveDirection.West);
                }
            }
            else{
               return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));
            
            }
            // Voici d'autres exemples d'actions possibles
            // -------------------------------------------

            // Si on ne veut rien faire, passer son tour
            // return BotHelper.ActionNone();

            // Déplacement du bot au nord
            // return BotHelper.ActionMove(MoveDirection.North);

            // Activation d'un bouclier de protection de niveau 10 (peut encaisser 10 points de dégats)
            // return BotHelper.ActionShield(10);

            // Activation d'un voile d'invisibilité sur une surface de 15
            // return BotHelper.ActionCloak(15);

            // Tir dans la direction sud
            // return BotHelper.ActionShoot(MoveDirection.South);

        }

    }
}