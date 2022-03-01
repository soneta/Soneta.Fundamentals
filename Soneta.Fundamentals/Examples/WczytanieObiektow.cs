using Soneta.Business;
using Soneta.Business.App;
using Soneta.CRM;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Collections;
using Soneta.Towary;
using Soneta.Handel;
using Soneta.Business.UI;

namespace Soneta.Fundamentals.Examples
{
    [Export(typeof(IOption))]
    [ExportMetadata("Key", ConsoleKey.F1)]
    [ExportMetadata("Description", "Wczytanie kontrahentów")]
    [ExportMetadata("Priority", 10)]
    class WczytanieKontrahentow : IOption
    {
        public bool Run()
        {
            // login + sesja
            Login login = Enova.Instance.Login;
            using (Session session = login.CreateSession(true, false)) {
                // uchwyt do modułu
                CRMModule crm = CRMModule.GetInstance(session); // session.GetCRM();

                // iteracja po tabeli
                foreach (Kontrahent k in crm.Kontrahenci.WgNazwy)
                {
                    Console.WriteLine(k.Nazwa);
                }
            }
            return true;
        }
    }

    [Export(typeof(IOption))]
    [ExportMetadata("Key", ConsoleKey.F2)]
    [ExportMetadata("Description", "Wczytanie towarów")]
    [ExportMetadata("Priority", 20)]
    class WczytanieTowarow : IOption
    {
        public bool Run()
        {
            Login login = Enova.Instance.Login;
            using (Session session = login.CreateSession(true, false))
            {
                TowaryModule tm = TowaryModule.GetInstance(session);
                foreach (Towar t in tm.Towary.WgEAN)
                {
                    Console.WriteLine(t);
                }
            }

            return true;
        }
    }

}
