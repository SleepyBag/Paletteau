using Newtonsoft.Json;
using Palette.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Palette
{
    internal class CommandList
    {
        ProcessIdentifier processIdentifier;
        List<IPaletteProvider> providers = new List<IPaletteProvider>();
        List<CommandItem> commandItems = null;

        public CommandList(ProcessIdentifier _processIdentifier)
        {
            processIdentifier = _processIdentifier;
        }

        public void AddProvider(IPaletteProvider provider)
        {
            providers.Add(provider);
        }

        public bool IsUpdated()
        {
            return providers.Any(provider => provider.IsUpdated());
        }

        public List<CommandItem> ToList()
        {
            // if command cached and there is no update
            if (commandItems != null && !IsUpdated())
                return commandItems;
            // update command cache
            // TODO: don't update cache of up-to-date providers
            commandItems = new List<CommandItem>();
            foreach (var provider in providers)
            {
                commandItems = commandItems.Concat(provider.GetItems(processIdentifier)).ToList();
            }
            return commandItems;
        }
    }

    internal class CommandTable
    {
        Dictionary<ProcessIdentifier, CommandList> commandListTable = new Dictionary<ProcessIdentifier, CommandList>();

        public bool Contains(Process process)
        {
            var processIdentifier = new ProcessIdentifier(process);
            return commandListTable.ContainsKey(processIdentifier);
        }

        public CommandList this[Process process]
        {
            get => commandListTable[new ProcessIdentifier(process)];
        }

        public void RegisterProvider(ProcessIdentifier processIdentifier, IPaletteProvider provider)
        {
            if (!commandListTable.ContainsKey(processIdentifier))
                commandListTable[processIdentifier] = new CommandList(processIdentifier);
            commandListTable[processIdentifier].AddProvider(provider);
        }
    }
}
