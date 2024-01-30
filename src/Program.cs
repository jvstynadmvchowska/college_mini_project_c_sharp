using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace Projekt_
{
    public delegate bool Wybierz<T>(T t);
    public enum RodzajSilnikaFlag
    {
        BENZYNOWY = 1,
        DIESEL = 2,
        ELEKTRYCZNY = 4
    }
    [Serializable]
    public abstract class Samochod : ICloneable, IComparable<Samochod>
    {
        public string Marka { get; set; }
        public string Model { get; set; }
        public RodzajSilnikaFlag RodzajSilnika { get; set; }
        public int RokProdukcji { get; set; }
        public abstract object Clone();
        public virtual int CompareTo(Samochod other)
        {
            if (this.RokProdukcji > other.RokProdukcji)
            {
                return 1;
            }
            if (this.RokProdukcji < other.RokProdukcji)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public Samochod(string marka, string model, int rokProdukcji)
        {
            Marka = marka;
            Model = model;
            RokProdukcji = rokProdukcji;
        }
        public virtual void WyswietlInformacje()
        {
            Console.Write($"Marka: {Marka}, Model: {Model}, " +
                $"Rok produkcji: {RokProdukcji}");
        }
        public class SamochodPoModeluComparer : IComparer<Samochod>
        {
            public int Compare(Samochod x, Samochod y)
            {
                if (x.Model.CompareTo(y.Model) > 0)
                {
                    return 1;
                }
                if (x.Model.CompareTo(y.Model) < 0)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }

            }
        }
    }
    [Serializable]
    public class SamochodSpalinowy : Samochod
    {
        public override object Clone()
        {
            return new SamochodSpalinowy(this.Marka, this.Model, this.RokProdukcji, this.RodzajSilnika);
        }
        public SamochodSpalinowy(string marka, string model, int rokProdukcji, RodzajSilnikaFlag rodzajSilnika) :
            base(marka, model, rokProdukcji)
        {
            RodzajSilnika = rodzajSilnika;
            if (rodzajSilnika == RodzajSilnikaFlag.ELEKTRYCZNY)
            {
                throw new ArgumentException("Samochody hybrydowe tworzymy jako hybrydowe, a nie spalinowe!");
            }
        }
        public override void WyswietlInformacje()
        {
            base.WyswietlInformacje();
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            Console.Write($" Rodzaj silnika: {RodzajSilnika}\n");
        }
    }
    [Serializable]
    public class SamochodElektryczny : Samochod
    {
        public int PojemnoscBaterii { get; set; }
        public override object Clone()
        {
            return new SamochodElektryczny(this.Marka, this.Model, this.RokProdukcji, this.PojemnoscBaterii);
        }
        public override int CompareTo(Samochod other)
        {
            if (other.GetType().Equals(this.GetType()))
            {
                if (this.PojemnoscBaterii > (other as SamochodElektryczny).PojemnoscBaterii)
                {
                    return -1;
                }
                if (this.PojemnoscBaterii < (other as SamochodElektryczny).PojemnoscBaterii)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return base.CompareTo(other);
            }
        }
        public SamochodElektryczny(string marka, string model, int rokProdukcji, int pojemnoscBaterii) :
            base(marka, model, rokProdukcji)
        {
            PojemnoscBaterii = pojemnoscBaterii;
        }
        public override void WyswietlInformacje()
        {
            base.WyswietlInformacje();
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            Console.Write($" Pojemność baterii: {PojemnoscBaterii} kWh\n");
        }
    }
    [Serializable]
    public class SamochodHybrydowy : Samochod
    {
        public int PojemnoscBaterii { get; set; }
        public override object Clone()
        {
            return new SamochodHybrydowy(this.Marka, this.Model, this.RokProdukcji, this.RodzajSilnika, this.PojemnoscBaterii);
        }
        public SamochodHybrydowy(string marka, string model, int rokProdukcji, RodzajSilnikaFlag rodzajSilnika, int pojemnoscBaterii) :
            base(marka, model, rokProdukcji)
        {
            if (rodzajSilnika.Equals(RodzajSilnikaFlag.BENZYNOWY | RodzajSilnikaFlag.ELEKTRYCZNY) == false)
            {
                throw new ArgumentException("Samochód hybrydowy składa się z silnika elektrycznego oraz spalinowego!");
            }
            PojemnoscBaterii = pojemnoscBaterii;
        }
        public override void WyswietlInformacje()
        {
            base.WyswietlInformacje();
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            Console.Write($" Rodzaj silnika: {RodzajSilnikaFlag.BENZYNOWY}, {RodzajSilnikaFlag.ELEKTRYCZNY} Pojemność baterii: {PojemnoscBaterii} kWh\n");
        }

    }
    [Serializable]
    public class SalonSamochodowy
    {

        List<Samochod> samochody;
        public void DodajSamochod(Samochod samochod)
        {
            samochody.Add(samochod);
        }
        public SalonSamochodowy()
        {
            samochody = new List<Samochod>();
        }
        public void UsunSamochod(Samochod samochod)
        {
            List<Samochod> kopia = new List<Samochod>();
            foreach (var item in samochody)
            {
                if (item.Equals(samochod) == false)
                {
                    kopia.Add(item);
                }
            }
            samochody = kopia;
        }
        public static SalonSamochodowy WczytajSalonZPliku(string nazwaPliku)
        {
            FileStream zrodlo = new FileStream(nazwaPliku, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            SalonSamochodowy salonzpliku = new SalonSamochodowy();
            List<Samochod> wczytanesamochody = (List<Samochod>)bf.Deserialize(zrodlo);
            salonzpliku.samochody = wczytanesamochody;
            zrodlo.Close();
            Console.WriteLine("\tSalon samochodowy został wczytany z pliku.");
            return salonzpliku;
        }
        public void WyswietlPosortowane()
        {
            samochody.Sort();
            WyswietlSamochody(samochody);
        }
        public void WyswietlPosortowane(IComparer<Samochod> comparer)
        {
            samochody.Sort(comparer);
            WyswietlSamochody(samochody);
        }
        public void WyswietlSamochody()
        {
            Console.WriteLine();
            foreach (var item in samochody)
            {
                item.WyswietlInformacje();
            }
        }
        public void WyswietlSamochody(List<Samochod> lista)
        {
            foreach (var item in lista)
            {
                item.WyswietlInformacje();
            }
        }
        public void WyswietlSamochody(Wybierz<Samochod> w)
        {
            Console.WriteLine();
            foreach (var item in samochody)
            {
                if (w(item))
                {
                    item.WyswietlInformacje();
                }
            }

        }
        public void ZapiszSalonDoPliku(string nazwaPliku)
        {
            using (FileStream zrodlo = new FileStream(nazwaPliku, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(zrodlo, samochody);
            }
            Console.WriteLine("\tSalon samochodowy został zapisany do pliku.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            FileStream filestream = new FileStream("Wynik.txt", FileMode.Create);
            StreamWriter streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);

            Console.WriteLine("\t\t\t\tBBBBB     M     M    W       W");
            Console.WriteLine("\t\t\t\tB     B    MM   MM    W       W");
            Console.WriteLine("\t\t\t\tB     B    M M M M    W   W   W");
            Console.WriteLine("\t\t\t\tBBBBB      M  M  M    W W W W");
            Console.WriteLine("\t\t\t\tB     B    M     M    WW   WW");
            Console.WriteLine("\t\t\t\tB     B    M     M    W     W");
            Console.WriteLine("\t\t\t\tBBBBB      M     M    W     W");
            Console.WriteLine("\t\t\t\t\tSalon sprzedaży");

            SalonSamochodowy salon = new SalonSamochodowy();

            SamochodSpalinowy spalinowy1 = new SamochodSpalinowy("BMW", "M3", 2022, RodzajSilnikaFlag.BENZYNOWY);
            // Tworzenie i dodawanie 50 instancji samochodów do salonu
            salon.DodajSamochod(spalinowy1);
            salon.DodajSamochod(new SamochodSpalinowy("BMW", "M5", 2023, RodzajSilnikaFlag.DIESEL));
            salon.DodajSamochod(new SamochodElektryczny("BMW", "i3", 2022, 60));
            salon.DodajSamochod(new SamochodElektryczny("BMW", "i3", 2022, 70));
            salon.DodajSamochod(new SamochodElektryczny("BMW", "i3", 2022, 93));
            salon.DodajSamochod(new SamochodHybrydowy("BMW", "330e", 2023, RodzajSilnikaFlag.BENZYNOWY | RodzajSilnikaFlag.ELEKTRYCZNY, 40));
            salon.DodajSamochod(new SamochodSpalinowy("BMW", "X5", 2022, RodzajSilnikaFlag.DIESEL));
            salon.DodajSamochod(new SamochodSpalinowy("BMW", "X7", 2023, RodzajSilnikaFlag.BENZYNOWY));
            salon.DodajSamochod(new SamochodElektryczny("BMW", "i4", 2022, 70));
            salon.DodajSamochod(new SamochodHybrydowy("BMW", "530e", 2023, RodzajSilnikaFlag.BENZYNOWY | RodzajSilnikaFlag.ELEKTRYCZNY, 50));
            salon.DodajSamochod(new SamochodSpalinowy("BMW", "M2", 2022, RodzajSilnikaFlag.BENZYNOWY));
            salon.DodajSamochod(new SamochodSpalinowy("BMW", "M8", 2023, RodzajSilnikaFlag.DIESEL));
            salon.DodajSamochod(new SamochodElektryczny("BMW", "iX3", 2022, 65));
            salon.DodajSamochod(new SamochodHybrydowy("BMW", "745e", 2023, RodzajSilnikaFlag.BENZYNOWY | RodzajSilnikaFlag.ELEKTRYCZNY, 45));
            salon.DodajSamochod(new SamochodSpalinowy("BMW", "X3", 2022, RodzajSilnikaFlag.DIESEL));
            salon.DodajSamochod(new SamochodSpalinowy("BMW", "X6", 2023, RodzajSilnikaFlag.BENZYNOWY));
            salon.DodajSamochod(new SamochodElektryczny("BMW", "i8", 2022, 75));
            salon.DodajSamochod(new SamochodHybrydowy("BMW", "X1 xDrive25e", 2023, RodzajSilnikaFlag.BENZYNOWY | RodzajSilnikaFlag.ELEKTRYCZNY, 55));
            salon.DodajSamochod(new SamochodSpalinowy("BMW", "330i", 2022, RodzajSilnikaFlag.BENZYNOWY));
            salon.DodajSamochod(new SamochodSpalinowy("BMW", "430i", 2023, RodzajSilnikaFlag.DIESEL));
            salon.DodajSamochod(new SamochodElektryczny("BMW", "iX5", 2022, 80));
            salon.DodajSamochod(new SamochodHybrydowy("BMW", "X2 xDrive25e", 2023, RodzajSilnikaFlag.BENZYNOWY | RodzajSilnikaFlag.ELEKTRYCZNY, 60));

            Console.WriteLine("\tWszystkie samochody w salonie:");
            salon.WyswietlSamochody();

            // Testowanie serializacji i deserializacji
            string nazwaPliku = "salon.bin";
            salon.ZapiszSalonDoPliku(nazwaPliku);

            Console.WriteLine("\n\tUsuwanie samochodu z salonu...");
            salon.UsunSamochod(spalinowy1);

            Console.WriteLine("\n\tWszystkie samochody po usunięciu:");
            salon.WyswietlSamochody();

            SalonSamochodowy wczytanySalon = SalonSamochodowy.WczytajSalonZPliku(nazwaPliku);

            Console.WriteLine("\n\tWczytane samochody z pliku:");
            wczytanySalon.WyswietlSamochody();

            Console.WriteLine("\n\tWyświetlanie samochodów według poszukiwanego wzorca (delegat i funkcja anonimowa):");
            salon.WyswietlSamochody((s) => s.Model == "M3");
            salon.WyswietlSamochody((s) => s.Model == "M2");

            Console.WriteLine("\n\tWyświetlanie posortowanych po roku produkcji, a elektryczne następnie po pojemności baterii malejąco:");
            salon.WyswietlPosortowane();

            Console.WriteLine("\n\tWyświetlanie po modelu:");
            salon.WyswietlPosortowane(new Samochod.SamochodPoModeluComparer());

            Console.WriteLine("\n\tTest obsługi wyjątków:");
            ExceptionsTester(() => { new SamochodSpalinowy("BMW", "M5", 2023, RodzajSilnikaFlag.ELEKTRYCZNY); });
            ExceptionsTester(() => { new SamochodHybrydowy("BMW", "745e", 2023, RodzajSilnikaFlag.BENZYNOWY, 45); });
            ExceptionsTester(() => { new SamochodHybrydowy("BMW", "745e", 2023, RodzajSilnikaFlag.DIESEL, 45); });
            ExceptionsTester(() => { new SamochodHybrydowy("BMW", "745e", 2023, RodzajSilnikaFlag.ELEKTRYCZNY, 45); });

            Console.SetOut(System.IO.TextWriter.Null);
        }

        public delegate void TesterDelegate();

        public static void ExceptionsTester(TesterDelegate test)
        {
            try
            {
                test();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

}
