/*

Bot qui se promène de façon aléatoire
Il possède une gestion minimale de son bouclier de protection

 */

using System; 
using BattleIA;


namespace explorateur
{
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

        
    
            int ennemyX = -1;
    int ennemyY = -1;

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
            // Toutes les autres fois, le bot n'effectue aucun scan...
            return 2;
        }

        // ****************************************************************************************************
        /// Résultat du scan
        public void AreaInformation(byte distance, byte[] informations)
{
    Console.WriteLine($"Area: {distance}");
    byte[] infoArray = new byte[informations.Length];
    Array.Copy(informations, infoArray, informations.Length);
    int index = 0;
    for (int i = 0; i < distance; i++)
    {
        for (int j = 0; j < distance; j++)
        {
            switch ((CaseState)infoArray[index++])
            {
                case CaseState.Empty: Console.Write("·"); break;
                case CaseState.Energy: Console.Write(""); break;
                case CaseState.Ennemy: Console.Write("E"); break;
                case CaseState.Wall: Console.Write("█"); break;
            }
        }
        Console.WriteLine();
    }


for (int i = 0; i < distance; i++)
{
    for (int j = 0; j < distance; j++)
    {
        switch ((CaseState)informations[index++])
        {
            case CaseState.Ennemy:
                ennemyX = j;
                ennemyY = i;
                break;
            case CaseState.Wall:
                Console.Write("█"); 
                break;
            case CaseState.Empty:
                Console.Write("·"); 
                break;
            case CaseState.Energy:
                Console.Write(""); 
                break;
        }
    }
    Console.WriteLine();
}
}
        /*public void AreaInformation(byte distance, byte[] informations)
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
        }*/

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
                /*// Tir dans la direction aléatoire
                if (ennemi==1){
                    return BotHelper.ActionShoot(MoveDirection.North);
                }
                if (ennemi==2){
                    return BotHelper.ActionShoot(MoveDirection.South);
                }*/
                if (ennemyX == -1 || ennemyY == -1)
                {
                    // Pas d'ennemi dans la zone, on ne tire pas
                    return BotHelper.ActionNone();
                }
                if (ennemyY < distance / 2)
                {
                    // L'ennemi est au nord
                    return BotHelper.ActionShoot(MoveDirection.North);
                }

            // S'il n'y a pas de bouclier actif, on en active un
            if (currentShieldLevel == 0)
            {
                currentShieldLevel = 1;
                return BotHelper.ActionShield(currentShieldLevel);
            }

            // On déplace le bot au hazard
            return BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));


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
}