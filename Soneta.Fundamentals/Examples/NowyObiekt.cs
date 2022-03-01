using Soneta.Business;
using Soneta.Business.App;
using Soneta.Towary;
using System;
using System.ComponentModel.Composition;

namespace Soneta.Fundamentals.Examples
{
    [Export(typeof(IOption))]
    [ExportMetadata("Key", ConsoleKey.F5)]
    [ExportMetadata("Description", "Utworzenie obiektu")]
    [ExportMetadata("Priority", 50)]
    class NowyObiekt : IOption
    {
        public bool Run()
        {
            // login + sesja
            Login login = Enova.Instance.Login;
            using (Session session = login.CreateSession(false, false)) {
                
                // instancja modułu
                TowaryModule tm = TowaryModule.GetInstance(session);
                
                // transakcja biznesowa
                using (ITransaction tr = session.Logout(true)) {
                    // nowy obiekt + dodanie do tabeli
                    // z poziomu każdego obiektu wczytanego od tego miejsca mamy uchwyt do sesji
                    // sesja jest zazwyczaj przyłączana do obiektów na operacji AddRow(),
                    // wcześniej obiekt jest w stanie 'detached' (bez sesji, z ujemnym ID)
                    Towar towar = new Towar();
                    tm.Towary.AddRow(towar); // przed dodaniem jest w trybie detached, niewiele z nim można zrobić

                    towar.Kod = "?";
                    towar.Nazwa = "Nowy Towar";

                    // zatwierdzenie transakcji
                    tr.Commit();
                }
                // zapis sesji
                session.Save(); 
            }
            return true;
        }
    }
}
