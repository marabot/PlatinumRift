using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platinum_rift
{
    static class Tools
    {

        public static int spawnQueryByOne(int achats, List<int>[] listSpawnNeutre, ref string retour)
        {
            int indexPlat = 0;

            for (indexPlat = 6; indexPlat >= 0; indexPlat--)
            {

                if (achats != 0)
                {
                    if (listSpawnNeutre[indexPlat].Count >= achats)
                    {
                        foreach (int zoneId in listSpawnNeutre[indexPlat])
                        {
                            retour = retour + "1 " + zoneId.ToString() + " ";
                        }
                        achats = 0;
                    }
                    else
                    {
                        int n = 0;
                        for (n = 0; n < listSpawnNeutre[indexPlat].Count; n++)
                        {
                            retour = retour + "1 " + listSpawnNeutre[indexPlat].ElementAt(n) + " ";
                        }
                        achats = achats - listSpawnNeutre[indexPlat].Count;
                    }
                }
            }
            return achats;
        }



        public static void initTab(ref List<int>[] tabLiens, int zoneCount)
        {
            int c;

            for (c = 0; c < zoneCount; c++)
            {
                tabLiens[c] = new List<int>();
            }
        }



        public static void initListspawn(List<int>[] list)
        {
            int l = list.Length;
            int i = 0;

            for (i = 0; i < l; i++)
            {
                list[i] = new List<int>();
            }
        }

        public static void buildTabsNextEnemy(int myId, int[,] listCarte, List<int>[] listIn, List<int>[] listOutFront, List<int>[] listOutBase)
        {
            int i = 0;

            for (i = 0; i < listIn.Length; i++)
            {

                foreach (int c in listIn[i])
                {
                    if (listCarte[c, 0] != myId && listCarte[c, 0] != -1)
                    {
                        listOutFront[i].Add(c);

                    }
                    else
                    {
                        listOutBase[i].Add(c);
                    }
                }
            }
        }

    }
}
