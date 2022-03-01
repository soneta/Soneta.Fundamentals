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
    [ExportMetadata("Key", ConsoleKey.F4)]
    [ExportMetadata("Description", "Lista baz")]
    [ExportMetadata("Priority", 40)]
    class ListaBaz : IOption
    {
        public bool Run()
        {
            Enova.Instance.ListDatabases();
            return true;
        }

    }
}
