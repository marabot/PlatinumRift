using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platinum_rift
{
    class Player
    {
        static void Main(String[] args)
        {

            //////////////////////////// declarations //////////////////////////////////
            string[] inputs;
            int turn = 0;  // numéro du tour, ne sert pour l'instant que pour le 1er tour
            int nbrePodsMe = 0; // nombre de mes pods sur la carte
            inputs = Console.ReadLine().Split(' ');
            int playerCount = int.Parse(inputs[0]); // the amount of players (2 to 4)
            int myId = int.Parse(inputs[1]); // my player ID (0, 1, 2 or 3)
            int zoneCount = int.Parse(inputs[2]); // the amount of zones on the map
            List<int>[] liensCase = new List<int>[zoneCount]; // tableau des liens des cases
            Tools.initTab(ref liensCase, zoneCount);
            int nbreIles = 0; // nombre d'îles de la carte
            List<int> listToMove = new List<int>(); // Liste des cases avec pod à bouger

            /*  tableau qui va contenir toutes les infos pour chaque case
               0: propriétaire de la case
               1: nombre de pods du joueur 1 
               2: nbre pods j2 
               3: nbre pods j3
               4: nbre pods j4
               5:nombre de mes pods
               6: productivité de la case
               7: île de la case
             */
            int[,] listCarte = new int[zoneCount, 8];



            int linkCount = int.Parse(inputs[3]); // the amount of links between all zones

            //////////// tableau des productivité des cases /////
            for (int i = 0; i < zoneCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int zoneId = int.Parse(inputs[0]); // this zone's ID (between 0 and zoneCount-1)
                int platinumSource = int.Parse(inputs[1]); // the amount of Platinum this zone can provide per game turn
                listCarte[zoneId, 6] = platinumSource;
            }

            //////////// tableau des liens //////////
            for (int i = 0; i < linkCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int zone1 = int.Parse(inputs[0]);
                int zone2 = int.Parse(inputs[1]);

                liensCase[zone1].Add(zone2);
                liensCase[zone2].Add(zone1);
            }

            nbreIles=findIsle(listCarte, liensCase);

            /////////////////////////////////////// GAME LOOP    /////////////////////////////

            while (true)
            {
                int wTabCarte;
                int hTabCarte;
                int iCarte;
                List<int> listAdjEnnemy = new List<int>();
                List<int> listAdjNeutre = new List<int>();
                nbrePodsMe = 0;

                listToMove = new List<int>();

                int platinum = int.Parse(Console.ReadLine()); // my available Platinum
                Console.Error.WriteLine(platinum);
                int achatsPotentiels = platinum / 20; // nombre d'achats possible
                Console.Error.WriteLine(achatsPotentiels);
                List<int>[] listSpawnNeutre = new List<int>[7]; // liste des possibilitées de spawns
                Tools.initListspawn(listSpawnNeutre);

                List<int>[] listSpawnBase = new List<int>[7];
                Tools.initListspawn(listSpawnBase);

                string move = ""; // commande de mouvement

                /////////////////////////////// TRAITEMENT CASE PAR CASE ///////////////////////        
                for (int i = 0; i < zoneCount; i++)
                {

                    int myPods = 0;
                    int ownerPods = 0;

                    inputs = Console.ReadLine().Split(' ');
                    int zId = int.Parse(inputs[0]); // this zone's ID
                    int ownerId = int.Parse(inputs[1]); // the player who owns this zone (-1 otherwise)
                    int podsP0 = int.Parse(inputs[2]); // player 0's PODs on this zone
                    int podsP1 = int.Parse(inputs[3]); // player 1's PODs on this zone
                    int podsP2 = int.Parse(inputs[4]); // player 2's PODs on this zone (always 0 for a two player game)
                    int podsP3 = int.Parse(inputs[5]); // player 3's PODs on this zone (always 0 for a two or three player game)

                    /// affecte myPods nombre de mes pods sur la case
                    switch (myId)
                    {
                        case 0:
                            myPods = podsP0;
                            break;

                        case 1:
                            myPods = podsP1;
                            break;

                        case 2:
                            myPods = podsP2;
                            break;

                        case 3:
                            myPods = podsP3;
                            break;
                    }


                    /// affect ownberId, nolbre de pod du propriétaire de la case
                    switch (ownerId)
                    {
                        case 0:
                            ownerPods = podsP0;
                            break;

                        case 1:
                            ownerPods = podsP1;
                            break;

                        case 2:
                            ownerPods = podsP2;
                            break;

                        case 3:
                            ownerPods = podsP3;
                            break;
                    }


                    listCarte[zId, 0] = ownerId;
                    listCarte[zId, 1] = podsP0;
                    listCarte[zId, 2] = podsP1;
                    listCarte[zId, 3] = podsP2;
                    listCarte[zId, 4] = podsP3;
                    listCarte[zId, 5] = myPods;
                    nbrePodsMe = nbrePodsMe + listCarte[zId, 5];

                    ////////// si la case est neutre /////////
                    if (ownerId == -1)
                    {
                        bool meAdj = false;
                        // Console.Error.WriteLine("avant point");
                        int rang = listCarte[zId, 6];

                        // Console.Error.WriteLine("après point");
                        //Ajoute à la liste des cases qui produisent comme elles 
                        listSpawnNeutre[rang].Add(zId);

                        foreach (int l in liensCase[zId])
                        {
                            if (listCarte[l, 0] == myId)
                            {
                                meAdj = true;
                            }
                        }

                        if (meAdj)
                        {
                            listAdjNeutre.Add(zId);
                        }
                    }

                    ///////// si la case est  à moi , la range dans la liste des case à moi
                    if (ownerId == myId)
                    {
                        int rang = listCarte[zId, 6];
                        listSpawnBase[rang].Add(zId);
                    }

                    // si la case est à l'ennemi et adjacente; la range dans la liste des cases neutres, test si il est adjacente à une case à moi, et la range dans la liste des cases adjacentes neutre si c'est le cas

                    if (ownerId != -1 && ownerId != myId)
                    {
                        bool isGoal = false;
                        foreach (int l in liensCase[zId])
                        {
                            if (listCarte[l, 0] == myId)
                            {
                                isGoal = true;
                            }
                        }

                        if (isGoal == true)
                        {
                            listAdjEnnemy.Add(zId);
                        }
                    }


                }
                //////////////////////////////////////////////////////////////////////////////////

                Console.Error.WriteLine("list Adj ennemy" + listAdjEnnemy.Count());
                Console.Error.WriteLine("list Adj neutre" + listAdjNeutre.Count());

                Console.Error.WriteLine("list spawn neutre:"
                                            + listSpawnNeutre[0].Count
                                            + listSpawnNeutre[1].Count
                                            + listSpawnNeutre[2].Count
                                            + listSpawnNeutre[3].Count
                                            + listSpawnNeutre[4].Count
                                            + listSpawnNeutre[5].Count
                                            + listSpawnNeutre[6].Count);

                Console.Error.WriteLine("list spawn base :"
                                                + listSpawnBase[0].Count
                                               + listSpawnBase[1].Count
                                               + listSpawnBase[2].Count
                                               + listSpawnBase[3].Count
                                               + listSpawnBase[4].Count
                                               + listSpawnBase[5].Count
                                               + listSpawnBase[6].Count);

                /////////////////////////////////// TRAITEMENT AVEC RETOUR /////////////////////

                // creation des listes de Cases cibles potentielles
                List<int>[,] listAdjIsles = new List<int>[nbreIles, 2];
                
                int iList=0;
                for(iList=0;iList<nbreIles;iList++)
                {
                    foreach(int c in listAdjEnnemy)
                        {
                            if (listCarte[c,7]==iList+1) listAdjIsles[iList,0].Add(c);                        
                        }

                    foreach (int c in listAdjNeutre)
                        {
                            if (listCarte[c, 7] == iList + 1) listAdjIsles[iList, 1].Add(c);
                        }        
                }

                
                wTabCarte = listCarte.GetLength(0);
                hTabCarte = listCarte.GetLength(1);

                iCarte = 0;
               
                List<int>[] tabNewCase = new List<int>[3]; // tableau quib range les nextCase en 3 cat : 0:ennemi 1: neutre 2: ami

                Tools.initTab(ref tabNewCase, 3);


                Console.Error.WriteLine("Pods : " + nbrePodsMe);
                ///////////////////// DEPLACEMENT DES PODS /////////////////////////////////////////////    
                /////////////////   pour chaque case de la carte
                for (iCarte = 0; iCarte < wTabCarte; iCarte++)
                {
                    int nextMove = 0;

                    /// si la case m'appartient et qu'il ya  des PODS
                    if (listCarte[iCarte, 0] == myId && listCarte[iCarte, 5] != 0)
                    {
                        Console.Error.WriteLine("traitement de la case : " + iCarte);
                        int pluscourt = 256;
                        int[] tabFrom = getTabFrom(iCarte, listCarte, liensCase);
                        Console.Error.WriteLine("taille tabFrom: " + tabFrom.Length);

                        //
                        //

                        if (listAdjEnnemy.Count() > 0)
                        {
                            pluscourt = 256;

                             
                            foreach (int c in listAdjEnnemy)
                            {

                                Console.Error.WriteLine("pluscourt ennemy: " + pluscourt);
                                if (getTaillePath(iCarte, c, tabFrom) < pluscourt && getTaillePath(iCarte, c, tabFrom) > 0)
                                {
                                    pluscourt = getTaillePath(iCarte, c, tabFrom);
                                    nextMove = getMove(iCarte, c, tabFrom);
                                }
                            }
                            Console.Error.WriteLine("nextMove :" + nextMove);
                            if (nextMove != 0) move = move + listCarte[iCarte, 5] + " " + iCarte + " " + nextMove + " ";
                        }
                        else
                        {
                            foreach (int c in listAdjNeutre)
                            {
                                pluscourt = 256;

                                Console.Error.WriteLine("pluscourt neutre: " + pluscourt);
                                if (getTaillePath(iCarte, c, tabFrom) < pluscourt && getTaillePath(iCarte, c, tabFrom) > 0)
                                {
                                    pluscourt = getTaillePath(iCarte, c, tabFrom);
                                    nextMove = getMove(iCarte, c, tabFrom);
                                }
                            }
                            if (nextMove != 0) move = move + listCarte[iCarte, 5] + " " + iCarte + " " + nextMove + " ";
                        }
                    }
                }


                string achatsDuTour = "";
                string achatsFinaux = "";
                List<int>[] listSpawnNE = new List<int>[7];  // list spawn Neutre next to  Ennemy
                Tools.initListspawn(listSpawnNE);
                List<int>[] listSpawnNB = new List<int>[7];  // list spawn Neutre  next to Base
                Tools.initListspawn(listSpawnNB);
                List<int>[] listSpawnBE = new List<int>[7];  // list spawn Base   next to Ennemy
                Tools.initListspawn(listSpawnBE);
                List<int>[] listSpawnBB = new List<int>[7];  // list spawn Neutre next to  Base
                Tools.initListspawn(listSpawnBB);


                Tools.buildTabsNextEnemy(myId, listCarte, listSpawnNeutre, listSpawnNE, listSpawnNB);
                Tools.buildTabsNextEnemy(myId, listCarte, listSpawnBase, listSpawnBE, listSpawnBB);

                achatsPotentiels = Tools.spawnQueryByOne(achatsPotentiels, listSpawnNE, ref achatsDuTour);
                achatsFinaux = achatsDuTour;
                // Console.Error.WriteLine(achatsPotentiels);
                achatsPotentiels = Tools.spawnQueryByOne(achatsPotentiels, listSpawnNB, ref achatsDuTour);
                achatsFinaux = achatsFinaux + achatsDuTour;

                achatsPotentiels = Tools.spawnQueryByOne(achatsPotentiels, listSpawnBE, ref achatsDuTour);
                achatsFinaux = achatsFinaux + achatsDuTour;

                achatsPotentiels = Tools.spawnQueryByOne(achatsPotentiels, listSpawnBB, ref achatsDuTour);
                achatsFinaux = achatsFinaux + achatsDuTour;
                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");
                //    Console.Error.WriteLine(move);
                //    Console.Error.WriteLine(achatsFinaux);
                if (move == "") move = "WAIT";

                if (turn == 2)
                {
                    achatsFinaux = getFirstSpawns(listCarte, listSpawnNeutre, liensCase);
                    Console.Error.WriteLine("first turn !!");
                }


                Console.WriteLine(move); // first line for movement commands, second line for POD purchase (see the protocol in the statement for details)
                Console.WriteLine(achatsFinaux);
            }
        }

        /// <summary>
        /// répertorié les cases par île. le résultat est classé dans la colonne d'index 7 de ListCarte
        /// </summary>
        /// <param name="listCarte"></param>
        /// <param name="liensCase"></param>
        private static int findIsle(int[,] listCarte, List<int>[] liensCase)
        {
            int start = isNoVisisted(listCarte);
            List<int> tas=new List<int>();
            int numIsle = 0;

            if (start != 0)
            {              
               // tant que des cases n'ont pas été répertorié dans une île
                while (start!=0)
                {
                    numIsle++;
                    tas.Add(start);
                    listCarte[start, 7] = numIsle;

                    // parcours de l'île
                    while (tas.Count > 0) {
                        int current = tas.ElementAt(tas.Count - 1);
                       
                        tas.RemoveAt(tas.Count - 1);

                        foreach (int c in liensCase[current])
                        { 
                            if (listCarte[current, 7]==0)
                            {
                                tas.Add(c);
                                listCarte[c, 7] = numIsle;
                            }                        
                        }
                    }
                    start=isNoVisisted(listCarte);
                }            
            }
            return numIsle;
        }


        /// <summary>
        /// renvoi le numéro de la denrière case à 0 (non répertoriée sur une île) lu, ou 0 si toutes sont répertortiées
        /// </summary>
        /// <param name="listCarte"></param>
        /// <returns></returns> 
        private static int isNoVisisted(int[,] listCarte)
        {
            int i=0;
            int result = 0;

            for (i = 0; i < listCarte.GetLength(0); i++)
            {
                if (listCarte[i, 7] == 0) {
                    result = i;
                }           
            }
            return result;
        }



        /// retourne un spawn du meilleur hexagone 6 + ses adjacents pour 10 pods
        private static string getFirstSpawns(int[,] listCarte, List<int>[] listSpawn, List<int>[] liensCase)
        {

            string result = "";
            int[,] tabPlatEmplacement = new int[listSpawn[5].Count(), 2];
            int indexCase = 0;

            foreach (int c6 in listSpawn[5])
            {
                indexCase++;             

                foreach (int c in liensCase[c6])
                {
                    tabPlatEmplacement[indexCase, 0] = tabPlatEmplacement[indexCase, 1] + listCarte[c, 6];
                    tabPlatEmplacement[indexCase, 1] = c6;
                }
            }
            /*
               int bestCase=0;
                  i=0;
                  int bestPlat=0;
                  for(i=0;i<tabPlatEmplacement.Length;i++)
                  {
                      if (tabPlatEmplacement[i]>bestPlat) result=tabPlatEmplacement[i, 1];
                  }
              */
            return result;

        }

        /// <summary>
        /// retourne le prochain mouvement pour aller à Goal depuis start
        /// </summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <param name="tabFrom"></param>
        /// <returns></returns>
        private static int getMove(int start, int goal, int[] tabFrom)
        {
            int result;
            int current = goal;

            do
            {
                result = current;
                current = tabFrom[current];
            } while (current != start);

            return result;
        }

        /// <summary>
        /// retourne la taille d'un chemin entre start et goal, renvoi -1 si pas de chemin possible
        /// </summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <param name="tabFrom"></param>
        /// <returns></returns>
        private static int getTaillePath(int start, int goal, int[] tabFrom)
        {
            int lChemin = 0;
            int current = goal;

            do
            {

                if (tabFrom[current] == 0)
                {
                    current = start;
                    lChemin = -1;
                }
                else
                {
                    current = tabFrom[current];
                    lChemin++;
                }

            } while (current != start);

            return lChemin;
        }


        /// <summary>
        ///  retourne la map des "FROM" pour le pathfinding
        /// </summary>
        /// <param name="start"></param>
        /// <param name="listcarte"></param>
        /// <param name="tabLiens"></param>
        /// <returns></returns>
        private static int[] getTabFrom(int start, int[,] listcarte, List<int>[] tabLiens)
        {
            List<int> listVisited = new List<int>();
            List<int> tas = new List<int>();
            int[] tabFrom = new int[listcarte.GetLength(0)];
            int current;


            tas.Add(start);

            // tant qu'il ya  des cases à visiter
            while (tas.Count != 0)
            {
                current = tas.Last();
                tas.RemoveAt(tas.Count() - 1);

                // pour chaque case en lien
                foreach (int next in tabLiens[current])
                {
                    //si pas visitée
                    if (!(listVisited.Exists(val => val == next)))
                    {
                        tas.Add(next); // ajoute aux cases à visiter
                        tabFrom[next] = current; // garde la case d'origine du chemin
                        listVisited.Add(next); // ajoute à la liste des déjà visitées
                    }
                }

            }
            Console.Error.WriteLine("mark getTabFrom");
            return tabFrom;
        }
    }
}
