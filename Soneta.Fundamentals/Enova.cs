using System;
using System.Linq;
using System.Collections;
using Soneta.Start;
using Soneta.Business;
using Soneta.Business.App;
using Soneta.Tools;
using System.ComponentModel.Composition;

namespace Soneta.Fundamentals
{
    internal class Enova
    {
        private Enova() { }

        private static Enova _instance;

        internal static Enova Instance
            => _instance ?? (_instance = new Enova());

        internal void InitializeAndLogIn()
        {
            string dbname = "szkolenia";
            string user = "Administrator";
            string pwd = "";

            LoadEnovaAssemblies();
            InitializeDb(dbname);
            LogIn(user, pwd);
        }

        private void LoadEnovaAssemblies()
        {
            //var loader = new Loader { WithUI = false, WithNet = false };
            var loader = new Loader { WithUI = true, WithNet = false }; // UI potrzebne dla opcji wydruku
            //loader.WithExtra = true; // jeśli potrzebne dlle oznaczone jako "extra" w Soneta.files.xml
            loader.Load();
        }
       
        private Database db;
        internal Database Db => db;
        private void InitializeDb(string _dbname)
        {
            db = BusApplication.Instance[_dbname];
        }

        public void ListDatabases()
        {
            BusApplication.Instance.Cast<Database>().ToList().ForEach(db => Console.WriteLine(db.Name));
        }

        internal IEnumerable Databases
        {
            get
            {
                return BusApplication.Instance.Cast<Database>().ToList().Select<Database, string>(db => db.Name);
            }
        }

        private IDisposable SessionStateDisposable;
        private Login _login;

        internal Login Login => _login;

        internal void LogIn(string user, string pwd)
        {
            if (_login == null) {
                try {
                    SessionStateDisposable = SessionState.Create().Attach();
                    _login = Db.Login(false, user, pwd);
                    Console.WriteLine("Poprawnie zalogowano do bazy");
                }
                catch {
                    Console.WriteLine("Błąd logowania do bazy");
                    throw;
                }
            }
        }

        internal Session CreateSession(bool readOnly, bool config)
        {
            return Login.CreateSession(readOnly, config);
        }

        internal void Unload()
        {
            if (_login != null)
            {
                _login.Database.Dispose();
                _login.Dispose();

                SessionStateDisposable?.Dispose();
            }
            //CoreTools.FinishApplication();
        }
    }
}
