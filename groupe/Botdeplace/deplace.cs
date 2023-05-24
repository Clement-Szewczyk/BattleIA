/*

Bot qui se promène de façon aléatoire
Il possède une gestion minimale de son bouclier de protection

 */

using System;
using BattleIA;


namespace Botdeplace
{
    public class Deplace
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
                        case CaseState.Energy: Console.Write("L"); break;
                        case CaseState.Ennemy: Console.Write("E"); break;
                        case CaseState.Wall: Console.Write("█"); break;
                    }
                }
                Console.WriteLine();
            }
            //calculer la distacne entre l'énergie et le bot
            //si la distance est inférieur à 3, on se déplace vers l'énergie
            //sinon on se déplace aléatoirement
            
            if (distance == 3)
            {
                //on se déplace vers l'énergie
                //on regarde si il y a de l'énergie à gauche
                if (informations[0] == 1)
                {
                    //on se déplace vers la gauche
                    BotHelper.ActionMove(MoveDirection.West);
                }
                //on regarde si il y a de l'énergie à droite
                if (informations[2] == 1)
                {
                    //on se déplace vers la droite
                    BotHelper.ActionMove(MoveDirection.East);
                }
                //on regarde si il y a de l'énergie en haut
                if (informations[6] == 1)
                {
                    //on se déplace vers le haut
                    BotHelper.ActionMove(MoveDirection.North);
                }
                //on regarde si il y a de l'énergie en bas
                if (informations[8] == 1)
                {
                    //on se déplace vers le bas
                    BotHelper.ActionMove(MoveDirection.South);
                }
            }
            else
            {
                //on se déplace aléatoirement
                BotHelper.ActionMove((MoveDirection)rnd.Next(1, 5));
            }
        }

        // ****************************************************************************************************
        /// On doit effectuer une action
        public byte[] GetAction()
        {
            return BotHelper.ActionNone();

        }

    }
}