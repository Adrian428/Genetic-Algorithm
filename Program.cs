using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WDTIGS
{
    class Program
    {
        static void Main(string[] args)
        {
            int iloscMiast;
            string file = "pr76.txt";
            const string folder = "C:\\Users\\aa\\Desktop\\WDTIGS\\";
            string path = folder + file;
            string[] files =
            {
                "pr76",
                "pr107",
                "eil51",
                "att48",
                "a280",
                "berlin52",

                "pr136"
            };

            double prawdopodobinstwoMutacji = 0.1;
            double prawdopodobienstwoKrzyzowania = 0.8;

            for (var k = 0; k < 1; k++)
            {
                for (var j = 0; j <= 5; j++)
                {
                 //   file = "a280.txt";
                    file = files[k];
                    path = folder + file + ".txt";
                    iloscMiast = ObliczIloscMiast(path);
                    var algorytm = new Algorytm(path, iloscMiast, prawdopodobienstwoKrzyzowania, prawdopodobinstwoMutacji);
                    var populacja = algorytm.UtworzPopulacje();



                    for (var i = 0; i <= 200000; i++)
                    {
                        algorytm.OcenTrasy(populacja);
                        algorytm.Selekcja(populacja);
                        algorytm.Krzyzowanie(populacja);
                        algorytm.Mutowanie(populacja);

                    }


                    using (var sw = new StreamWriter("C:\\Users\\aa\\Desktop\\WDTIGS\\wyniki.txt", true))
                    {
                        sw.WriteLine(file + ";" + algorytm.Minimum);
                    }
                }

            }
        }

        private static int ObliczIloscMiast(string path)
        {
            using (var sr = new StreamReader(path))
            {
                return int.Parse(sr.ReadLine());
            }

        }
    }
}
