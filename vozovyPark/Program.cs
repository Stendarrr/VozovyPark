using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace vozovyPark
{
    class User
    {
        public string uid { get; set; } //user id
        public string jmeno { get; set; }
        public string prijmeni { get; set; }
        public string heslo { get; set; }
        public DateTime logoutDateTime { get; set; }
        public bool zmenitHeslo { get; set; }

        public User(string uid, string jmeno, string prijmeni, string heslo, bool zmenitHeslo)
        {
            this.uid = uid;
            this.jmeno = jmeno;
            this.prijmeni = prijmeni;
            this.heslo = heslo;
            this.zmenitHeslo = zmenitHeslo;
        }
    }

    class Car
    {
        public string vin { get; set; } //vehicle identification number
        public string znacka { get; set; }
        public string model { get; set; }
        public string typ { get; set; }
        public double spotreba { get; set; }
        public string servisniZaznamy { get; set; }

        public Car(string vin, string znacka, string model, string typ, double spotreba, string servisniZaznamy)
        {
            this.vin = vin;
            this.znacka = znacka;
            this.model = model;
            this.typ = typ;
            this.spotreba = spotreba;
            this.servisniZaznamy = servisniZaznamy;
        }
    }
    class Rezervace
    {
        public string uid { get; set; }
        public string vin { get; set; }
        public DateTime vypujceni { get; set; }
        public DateTime vraceni { get; set; }


        public Rezervace(string uid, string vin)
        {
            this.uid = uid;
            this.vin = vin;
        }
    }

    class Program
    {
        //nastavení proměnných
        static List<User> users = new List<User>();
        static List<Car> cars = new List<Car>();
        static List<Rezervace> rezervace = new List<Rezervace>();
        static string uid;
        static void Main(string[] args)
        {
            //načtení objektů
            if (File.Exists("users.json"))
            {
                string jsonUsers = File.ReadAllText("users.json");
                users = JsonSerializer.Deserialize<List<User>>(jsonUsers);

            }
            else
            {
                //pokud neexistuje žádný uživatel, tak vytvoř admina
                users.Add(new User("admin", "Jan", "Nopartný", "", false));
            }
            if (File.Exists("cars.json"))
            {
                string jsonCars = File.ReadAllText("cars.json");
                cars = JsonSerializer.Deserialize<List<Car>>(jsonCars);
            }

            if (File.Exists("reservations.json"))
            {
                string jsonReservations = File.ReadAllText("reservations.json");
                rezervace = JsonSerializer.Deserialize<List<Rezervace>>(jsonReservations);
            }
            Console.WriteLine("Vítejte ve Vozovém parku ");
            /*
                        //přihlášení
                        bool logged = false;
                        do
                        {
                            Console.Write("Zadejte jméno: ");
                            uid = Console.ReadLine();
                            Console.Write("Zadejte heslo: ");
                            string heslo = Console.ReadLine();
                            foreach (User user in users)
                            {
                                if (user.uid == uid && user.heslo == heslo)
                                {
                                    logged = true;
                                }
                            }
                            if (!logged)
                            {
                                Console.WriteLine("Zadal jste špatné uživatelské jméno nebo heslo");
                                Console.WriteLine();
                            }
                        } while (!logged);
            */

            //uid admin - debug
            uid = "admin";
            //kontrola, zda si uživatel nemá změnit heslo
            User user = users.Find(e => e.uid == uid);
            if (user.zmenitHeslo)
            {
                Console.WriteLine("Máte vynucenou změnu hesla, změňte si heslo!!!!!!!");
                ZmenaHesla();
            }
            //user menu
            int vyber;
            do
            {
                Console.WriteLine("---------------");
                Console.WriteLine("(0) Odhlásit se");
                Console.WriteLine("(1) Změna hesla");
                Console.WriteLine("-----------------------------");
                Console.WriteLine("(2) Zobrazit seznam rezervací");
                Console.WriteLine("(3) Přidat rezervaci vozu");
                Console.WriteLine("(4) Zrušení rezervace");
                //admin menu
                if (uid == "admin")
                {
                    Console.WriteLine("-----------------------------");
                    Console.WriteLine("(5) Zobrazit seznam uživatelů");
                    Console.WriteLine("(6) Přidat uživatele");
                    Console.WriteLine("(7) Odstranit uživatele");
                    Console.WriteLine("(8) Vynutit změnu hesla");
                    Console.WriteLine("---------------------------");
                    Console.WriteLine("(9) Zobrazit seznam vozidel");
                    Console.WriteLine("(10) Přidat vůz");
                    Console.WriteLine("(11) Odstranit vůz");
                }
                Console.WriteLine();
                Console.Write("Vyberte si číslo z menu: ");
                try
                {
                    vyber = int.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    vyber = 99;
                }

                Console.WriteLine();
                switch (vyber)
                {
                    case 0:
                        OdhlasitSe();
                        break;
                    case 1:
                        ZmenaHesla();
                        break;
                    case 2:
                        ZobrazSeznamRezervaci();
                        break;
                    case 3:
                        PridejRezervaci();
                        break;
                    case 4:
                        ZruseniRezervace();
                        break;
                    //jen admin
                    case 5:
                        ZobrazSeznamUzivatelu();
                        break;
                    case 6:
                        PridejUzivatele();
                        break;
                    case 7:
                        OdstranUzivatele();
                        break;
                    case 8:
                        VynutitZmenuHesla();
                        break;
                    case 9:
                        ZobrazSeznamVozidel();
                        break;
                    case 10:
                        PridejVozidlo();
                        break;
                    case 11:
                        OdstranVozidlo();
                        break;
                    default:
                        Console.WriteLine("Tuto volbu neznám, zkuste to znovu");
                        break;

                }
            } while (vyber != 0);
        }
        static void OdhlasitSe()
        {
            User user = users.Find(e => e.uid == uid);
            user.logoutDateTime = DateTime.Now;
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string jsonUsers = JsonSerializer.Serialize(users, options);
            File.WriteAllText("users.json", jsonUsers);
            string jsonCars = JsonSerializer.Serialize(cars, options);
            File.WriteAllText("cars.json", jsonCars);
            string jsonReservations = JsonSerializer.Serialize(rezervace, options);
            File.WriteAllText("reservations.json", jsonReservations);

        }
        static void ZmenaHesla()
        {
            Console.WriteLine("====================================Změna hesla====================================");
            Console.WriteLine();
            Console.Write("Zadejte nové heslo: ");
            string noveHeslo = Console.ReadLine();
            User user = users.Find(e => e.uid == uid);
            user.heslo = noveHeslo;
            user.zmenitHeslo = false;
        }
        static void ZobrazSeznamRezervaci()
        {
            Console.WriteLine("====================================Historie rezervací vozidel====================================");
            Console.Write("ID Uživatele".PadRight(20, '.'));
            Console.Write("VIN".PadRight(20, '.'));
            Console.Write("Od".PadRight(20, '.'));
            Console.WriteLine("Do");
            foreach (Rezervace rezervace in rezervace)
            {
                Console.Write(rezervace.uid.PadRight(20));
                Console.Write(rezervace.vin.PadRight(20));
                Console.Write(Convert.ToString(rezervace.vypujceni).PadRight(20));
                Console.WriteLine(Convert.ToString(rezervace.vraceni));
            }
            Console.WriteLine();
        }
        static void PridejRezervaci()
        {
            Console.WriteLine("====================================Rezervace vozidla====================================");
            Console.WriteLine();
            Console.WriteLine("....................................Zobrazení seznamu vozidel................................");
            Console.WriteLine();
            Console.Write("#".PadRight(5, '.'));
            Console.Write("VIN".PadRight(20, '.'));
            Console.Write("Značka".PadRight(20, '.'));
            Console.Write("Model".PadRight(20, '.'));
            Console.WriteLine("Typ");
            int i = 1;
            foreach (Car auto in cars)
            {
                Console.Write(Convert.ToString(i++).PadRight(5));
                Console.Write(auto.vin.PadRight(20));
                Console.Write(auto.znacka.PadRight(20));
                Console.Write(auto.model.PadRight(20));
                Console.WriteLine(auto.typ);
            }
            Console.WriteLine();
            int index = -1;
            int index2 = 0;
            Car car = null;
            string uidRezervace = "";
            DateTime? zapujcka = null;
            DateTime? vratka = null;

            do
            {
                Console.Write("Zadejte číslo auto, který chcete zarezervovat: ");
                do
                {
                    try
                    {
                        index = int.Parse(Console.ReadLine());
                        car = cars[--index];
                    }
                    catch (Exception)
                    {
                        Console.Write("Neznámý záznam, zadejte číslo vozu, které chcete zarezervovat: ");
                    }
                } while (car == null);

                if (uid == "admin")
                {
                    Console.Write("Zadejte uživatele na kterého chcete vytvořit rezervaci: ");
                    uidRezervace = Console.ReadLine();
                }
                else
                {
                    uidRezervace = uid;
                }

                Console.Write("Zadejte předpokládané datum zapůjčení vozu: ");
                do
                {
                    try
                    {
                        zapujcka = DateTime.Parse(Console.ReadLine());

                    }
                    catch (Exception)
                    {

                        Console.Write("Zadané datum je ve špatném formátu, zadejte prosím dd.mm.yyyy: ");
                    }
                } while (zapujcka == null);

                Console.Write("Zadejte datum předpokládaného vrácení vozu: ");
                do
                {
                    try
                    {
                        vratka = DateTime.Parse(Console.ReadLine());

                    }
                    catch (Exception)
                    {

                        Console.Write("Zadané datum je ve špatném formátu, zadejte prosím dd.mm.yyyy: ");
                    }
                } while (vratka == null);
                index2 = rezervace.FindIndex(rez => rez.vin == car.vin && rez.vypujceni <= vratka && rez.vraceni >= zapujcka);
                if (index2 > -1)
                {
                    Console.WriteLine("Rezervace se překrývá s jinou, prosím zadejte novou.");
                }
            } while (index2 != -1);







            Rezervace r = new Rezervace(uidRezervace, car.vin);
            r.vypujceni = (DateTime)zapujcka;
            r.vraceni = (DateTime)vratka;
            rezervace.Add(r);

            Console.WriteLine();
            Console.WriteLine("Rezervace přijata.");
            Console.WriteLine();
        }

        static void ZruseniRezervace()
        {
            Console.WriteLine("====================================Zrušení rezervace vozidla====================================");
            Console.WriteLine();
            Console.WriteLine("....................................Zobrazení seznamu rezervací................................");
            Console.WriteLine();
            Console.Write("#".PadRight(5, '.'));
            Console.Write("ID uživatele".PadRight(20, '.'));
            Console.Write("VIN".PadRight(20, '.'));
            Console.Write("Od".PadRight(20, '.'));
            Console.WriteLine("Do");
            int i = 1;
            foreach (Rezervace rezervace in rezervace)
            {
                Console.Write(Convert.ToString(i++).PadRight(5));
                Console.Write(rezervace.uid.PadRight(20));
                Console.Write(rezervace.vin.PadRight(20));
                Console.Write(Convert.ToString(rezervace.vypujceni).PadRight(20));
                Console.WriteLine(Convert.ToString(rezervace.vraceni));
            }
            Console.WriteLine();
            Console.Write("Zadejte číslo záznamu, který chcete odstranit: ");
            try
            {
                int index = int.Parse(Console.ReadLine());
                cars.RemoveAt(index - 1);
                Console.WriteLine("Záznam byl úspěšně odstraněn.");
            }
            catch (Exception)
            {
                Console.WriteLine("Neznámý záznam");
            }
            Console.WriteLine();
        }
        static void ZobrazSeznamUzivatelu()
        {
            Console.WriteLine("====================================Zobrazení seznamu uživatel====================================");
            Console.WriteLine();
            Console.Write("ID".PadRight(20, '.'));
            Console.Write("Jméno".PadRight(20, '.'));
            Console.Write("Příjmení".PadRight(20, '.'));
            Console.Write("Musí změnit heslo?".PadRight(20, '.'));
            Console.WriteLine("Datum posledního odhlášení");
            foreach (User user in users)
            {
                Console.Write(user.uid.PadRight(20));
                Console.Write(user.jmeno.PadRight(20));
                Console.Write(user.prijmeni.PadRight(20));
                Console.Write(Convert.ToString(user.zmenitHeslo).PadRight(20));
                Console.WriteLine(user.logoutDateTime.ToString("d") + " " + user.logoutDateTime.ToString("t"));
            }
            Console.WriteLine();
        }
        static void PridejUzivatele()
        {
            Console.WriteLine("====================================Přidávání nových uživatel====================================");
            //pokračovat sám - sesbírat všechny informace
            Console.Write("Zadejte uživatelské jméno: ");
            string login = Console.ReadLine();
            Console.Write("Zadejte jméno");
            string name = Console.ReadLine();
            Console.Write("Zadejte příjmení: ");
            string surname = Console.ReadLine();
            Console.Write("Zadejte heslo: ");
            string password = Console.ReadLine();
            users.Add(new User(login, name, surname, password, false));
        }
        static void OdstranUzivatele()
        {
            Console.WriteLine("====================================Odstraňování uživatelů====================================");
            Console.WriteLine();
            Console.WriteLine("....................................Zobrazení seznamu uživatel................................");
            Console.WriteLine();
            Console.Write("#".PadRight(5, '.'));
            Console.Write("ID".PadRight(20, '.'));
            Console.Write("Jméno".PadRight(20, '.'));
            Console.WriteLine("Příjmení");
            int i = 1;
            foreach (User user in users)
            {
                Console.Write(Convert.ToString(i++).PadRight(5));
                Console.Write(user.uid.PadRight(20));
                Console.Write(user.jmeno.PadRight(20));
                Console.WriteLine(user.prijmeni.PadRight(20));
            }
            Console.WriteLine();
            Console.Write("Zadejte číslo záznamu, který chcete odstranit: ");
            try
            {
                int index = int.Parse(Console.ReadLine());
                users.RemoveAt(index - 1);
                Console.WriteLine("Záznam byl úspěšně odstraněn.");
            }
            catch (Exception)
            {
                Console.WriteLine("Neznámý záznam");
            }
            Console.WriteLine();
        }
        static void VynutitZmenuHesla()
        {
            Console.WriteLine("====================================Vynucení změny hesla====================================");
            Console.WriteLine();
            Console.Write("Zadejte hledaného uživatele: ");
            string uid = Console.ReadLine();
            User user = users.Find(e => e.uid == uid);
            user.zmenitHeslo = true;

        }
        static void ZobrazSeznamVozidel()
        {
            Console.WriteLine("====================================Zobrazení seznamu vozidel====================================");
            Console.WriteLine();
            Console.Write("VIN".PadRight(20, '.'));
            Console.Write("Značka".PadRight(20, '.'));
            Console.Write("Model".PadRight(20, '.'));
            Console.Write("Typ".PadRight(20, '.'));
            Console.WriteLine("Spotřeba");
            foreach (Car car in cars)
            {
                Console.Write(car.vin.PadRight(20));
                Console.Write(car.znacka.PadRight(20));
                Console.Write(car.model.PadRight(20));
                Console.Write(car.typ.PadRight(20));
                Console.WriteLine(car.spotreba + " l/100 km");
            }
            Console.WriteLine();
        }
        static void PridejVozidlo()
        {
            Console.WriteLine("====================================Přidávání vozidel====================================");
            Console.Write("Napište identifikační číslo vozu: ");
            string vin = Console.ReadLine();
            Console.Write("Zadejte značku vozidla: ");
            string brand = Console.ReadLine();
            Console.Write("Zadejte model: ");
            string model = Console.ReadLine();
            Console.Write("Zadejte typ vozu: ");
            string type = Console.ReadLine();
            Console.Write("Zadejte spotřebu (v l/100 km): ");
            double consumption = double.Parse(Console.ReadLine());
            Console.Write("Napište servisní záznamy: ");
            string serviceRecords = Console.ReadLine();
            cars.Add(new Car(vin, brand, model, type, consumption, serviceRecords));
        }
        static void OdstranVozidlo()
        {
            Console.WriteLine("====================================Odstraňování vozidel====================================");
            Console.WriteLine();
            Console.WriteLine("....................................Zobrazení seznamu vozidel................................");
            Console.WriteLine();
            Console.Write("#".PadRight(5, '.'));
            Console.Write("VIN".PadRight(20, '.'));
            Console.Write("Značka".PadRight(20, '.'));
            Console.Write("Model".PadRight(20, '.'));
            Console.WriteLine("Typ");
            int i = 1;
            foreach (Car car in cars)
            {
                Console.Write(Convert.ToString(i++).PadRight(5));
                Console.Write(car.vin.PadRight(20));
                Console.Write(car.znacka.PadRight(20));
                Console.Write(car.model.PadRight(20));
                Console.WriteLine(car.typ);
            }
            Console.WriteLine();
            Console.Write("Zadejte číslo záznamu, který chcete odstranit: ");
            try
            {
                int index = int.Parse(Console.ReadLine());
                cars.RemoveAt(index - 1);
                Console.WriteLine("Záznam byl úspěšně odstraněn.");
            }
            catch (Exception)
            {
                Console.WriteLine("Neznámý záznam");
            }
            Console.WriteLine();
        }
    }
}

