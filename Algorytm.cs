using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.CodeDom;

namespace WDTIGS
{
    class Algorytm
    {
        private readonly int _iloscMiast;
        private readonly string[][] _odleglosci;
        public int Minimum; //zmienna dla najkrótszej trasy
        public Trasa NajkrotszaTrasa;
        private readonly double _prawdopodobienstwoKrzyzowania;
        private readonly double _prawdopodobiebstwoMutacji;

        public Algorytm(string path, int iloscMiast, double prawdopodobienstwoKrzyzowania, double prawdopodobiebstwoMutacji)
        {
            _iloscMiast = iloscMiast;
            _prawdopodobienstwoKrzyzowania = prawdopodobienstwoKrzyzowania;
            _prawdopodobiebstwoMutacji = prawdopodobiebstwoMutacji;
            _odleglosci = WczytajOdleglosci(path);
            Minimum = 1000000000;
        }

        public int ObliczOdleglosc(List<int> trasa)
        {
            var kolejnoscMiast = trasa;
            var odleglosci = _odleglosci;
            var suma = 0;

            for (var i = 1; i <= kolejnoscMiast.Count - 1; i++)
            {
                var miasto = kolejnoscMiast[i - 1];
                var nastepneMiasto = kolejnoscMiast[i];
                if (miasto > nastepneMiasto)
                {
                    suma += int.Parse(odleglosci[miasto - 1][nastepneMiasto - 1]);
                }
                else
                {
                    suma += int.Parse(odleglosci[nastepneMiasto - 1][miasto - 1]);
                }
            }
            return suma;
        }

        public Dictionary<int, int> LosujPary()
        {
            var r = new Random();
            var pary = new Dictionary<int, int>();
            do
            {
                var pierwszy = r.Next(0, 20);
                var drugi = r.Next(0, 20);
                if (pierwszy != drugi)
                {
                    if (!(pary.ContainsKey(pierwszy) || pary.ContainsValue(pierwszy)) &&
                                        !(pary.ContainsKey(drugi) || pary.ContainsValue(drugi)))
                    {
                        pary.Add(pierwszy, drugi);
                    }
                }
            }
            while (pary.Count < 10);
            return pary;
        }

        //Krzyżowanie po przez wstawianie podtrasy według pozycji
        // 1. losowanie dwóch miast do krzyżowania
        // 2. ilość miast do zmiany
        public List<Trasa> Krzyzowanie(List<Trasa> populacja)
        {
            var r = new Random();



            var pary = LosujPary();
            var prawdopodobienstwo = _prawdopodobienstwoKrzyzowania;
            foreach (var para in pary)
            {
                var losowa = r.NextDouble();

                if (losowa <= prawdopodobienstwo)
                {
                    var trasa1 = populacja[para.Key].KolejnoscMiast;
                    var trasa2 = populacja[para.Value].KolejnoscMiast;
                    var startPosition = r.Next(0, trasa1.Count); // losowa pozycja początkowa wycinania
                    var endPosition = r.Next(startPosition, trasa1.Count) - startPosition; // ilosc pozycji do wyciecia
                    var lenght = startPosition + endPosition; // pozycja poczatku ogona


                    var potomek1 = trasa1.GetRange(startPosition, endPosition);
                    var potomek2 = trasa2.GetRange(startPosition, endPosition);

                    var trasa1Tail = trasa1.GetRange(lenght, trasa1.Count - lenght);
                    trasa1Tail.Reverse();
                    var trasa2Tail = trasa2.GetRange(lenght, trasa1.Count - lenght);
                    trasa2Tail.Reverse();

                    for (var i = 0; i < trasa1Tail.Count; i++)
                    {
                        if (!potomek1.Contains(trasa2Tail[i]))
                        {
                            potomek1.Add(trasa2Tail[i]);
                        }

                        if (!potomek2.Contains(trasa1Tail[i]))
                        {
                            potomek2.Add(trasa1Tail[i]);
                        }

                    }

                    for (var i = 0; i < trasa1.Count - trasa1Tail.Count; i++)
                    {
                        if (!potomek1.Contains(trasa2[i]))
                        {
                            potomek1.Add(trasa2[i]);
                        }

                        if (!potomek2.Contains(trasa1[i]))
                        {
                            potomek2.Add(trasa1[i]);
                        }



                    }

                    populacja[para.Key].KolejnoscMiast = potomek2;
                    populacja[para.Value].KolejnoscMiast = potomek1;
                }
            }

            return populacja;
        }

        public List<Trasa> Mutowanie(List<Trasa> populacja)
        {

            var prawdopodobienstwo = _prawdopodobiebstwoMutacji;
            var r = new Random();


            for (var i = 0; i < 20; i++)
            {
                for (var j = 0; j < _iloscMiast; j++)
                {


                    var losowa = r.NextDouble();
                    if (losowa <= prawdopodobienstwo)
                    {
                        var losowaPozycja = r.Next(0, _iloscMiast);
                        var miasto1 = populacja[i].KolejnoscMiast[j];
                        var miasto2 = populacja[i].KolejnoscMiast[losowaPozycja];

                        populacja[i].KolejnoscMiast[j] = miasto2;
                        populacja[i].KolejnoscMiast[losowaPozycja] = miasto1;

                        populacja[i].SumaOdleglosci = ObliczOdleglosc(populacja[i].KolejnoscMiast);

                    }
                }
            }
            return populacja;
        }

        public List<Trasa> Selekcja (List<Trasa> populacja)
        {

            var wylosowaneTrasy = new List<Trasa>();

            var sumaOcen = populacja.Sum(x => x.Ocena);
            var r = new Random();

            var czyWylosowano = false;
            do
            {
                var losowa = r.Next(0, sumaOcen);
                for (var i = 0; i < 20; i++)
                {
                    losowa -= populacja[i].Ocena;
                    if (losowa < 0 && !czyWylosowano)
                    {
                        wylosowaneTrasy.Add(populacja[i]);
                        czyWylosowano = true;
                    }

                }
                czyWylosowano = false;
            }
            while (wylosowaneTrasy.Count != 20);

            return wylosowaneTrasy;
        }

        private List<int> LosujTrase()
        {
            var kolejnoscMiast = new List<int>();
            var r = new Random();


            while (kolejnoscMiast.Count <= _iloscMiast - 1)
            {
                var losowa = r.Next(1, _iloscMiast + 1);

                if (!kolejnoscMiast.Contains(losowa))
                {

                    kolejnoscMiast.Add(losowa);
                }

            }
            return kolejnoscMiast;
        }

        public List<Trasa> UtworzPopulacje()
        {
            var populacja = new List<Trasa>();
            for (var i = 0; i < 20; i++)
            {
                var trasa = new Trasa
                {
                    Id = i,
                    KolejnoscMiast = LosujTrase()
                };
                populacja.Add(trasa);
                System.Threading.Thread.Sleep(100);
            }
            return populacja;
        }

        private static string[][] WczytajOdleglosci(string path)
        {
            var readText = (File.ReadAllLines(path));

            //pierwsza linia to ilość miast
            var iloscMiast = int.Parse(readText[0]);

            //zmienna przechowująca odległości miedzy miastami
            var odleglosci = new string[iloscMiast][];
            //inicjuje zmienna
            for (var x = 0; x < odleglosci.Length; x++)
            {
                odleglosci[x] = new string[x];
            }

            //ciąg odległosci wprowadzam do macierzy
            for (int i = 0, j = 1; i < odleglosci.Length; j++, i++)
            {
                odleglosci[i] = readText[j].Split(' ');
            }

            return odleglosci;
        }

        public List<Trasa> ObliczSumyWPopulacji(List<Trasa> populacja)
        {
            var odleglosci = _odleglosci;
            foreach (var trasa in populacja)
            {
                var kolejnoscMiast = trasa.KolejnoscMiast;

                var suma = 0;

                for (var i = 1; i <= kolejnoscMiast.Count - 1; i++)
                {
                    var miasto = kolejnoscMiast[i - 1];
                    var nastepneMiasto = kolejnoscMiast[i];
                    if (miasto > nastepneMiasto)
                    {
                        suma += int.Parse(odleglosci[miasto - 1][nastepneMiasto - 1]);
                    }
                    else
                    {
                        suma += int.Parse(odleglosci[nastepneMiasto - 1][miasto - 1]);
                    }
                }

                trasa.SumaOdleglosci = suma;

            }
            return populacja;
        }

        private static int ZnajdzNajdluzszaTrase(IEnumerable<Trasa> populacja)
        {
            var najdluzszaTrasa = populacja.Max(x => x.SumaOdleglosci);
            return najdluzszaTrasa;
        }

        public List<Trasa> OcenTrasy(List<Trasa> populacja)
        {
            ObliczSumyWPopulacji(populacja);
            var max = ZnajdzNajdluzszaTrase(populacja) + 1;
            var najkrotszaTrasa = 10000000;
            foreach (var trasa in populacja)
            {
                trasa.Ocena = max - trasa.SumaOdleglosci;
                if (trasa.SumaOdleglosci < najkrotszaTrasa)
                    najkrotszaTrasa = trasa.SumaOdleglosci;
            }

            if (Minimum > najkrotszaTrasa)
            {
                Minimum = najkrotszaTrasa;
                Console.WriteLine("Najkrótsza trasa: " + Minimum);
            }




            return populacja;

        }





    }
}
