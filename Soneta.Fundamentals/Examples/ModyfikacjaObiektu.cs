using Soneta.Business;
using Soneta.Business.App;
using Soneta.CRM;
using Soneta.Towary;
using Soneta.Types;
using System;
using System.ComponentModel.Composition;

namespace Soneta.Fundamentals.Examples
{
    [Export(typeof(IOption))]
    [ExportMetadata("Key", ConsoleKey.F3)]
    [ExportMetadata("Description", "Modyfikacja obiektu")]
    [ExportMetadata("Priority", 30)]
    class ModyfikacjaObiektu : IOption
    {
        public bool Run()
        {
            // login + sesja
            Login login = Enova.Instance.Login;
            using (Session session = login.CreateSession(false, false)) {
                
                //instancja modułu
                TowaryModule tm = TowaryModule.GetInstance(session);

                // wczytanie obiektu
                Towar bikini = tm.Towary.WgKodu["BIKINI"]; // odwołanie się po kluczu UNIKALNYM zwraca 1 obiekt 
                                                           // po nieunikalnym zwraca kolekcję (typ SubTable<>)

                // transakcja 
                ITransaction tr = session.Logout(true); // można mieć kilka transakcji w tej samej sesji
                                                        // mogą być zagnieżdżone

                // modyfikacja
                bikini.Opis = "Zmodyfikowane przez aplikację Soneta.Fundamentals " + DateTime.Now.ToString();

                // zatwierdzenie transakcji
                tr.Commit();
                tr.Dispose();

                // zapis sesji; tu dopiero następuje zapis do bazy
                session.Save();
            }

            return true;
        }
    }
}
