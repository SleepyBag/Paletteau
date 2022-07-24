using Paletteau.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palette.Providers
{
    internal interface IPaletteProvider
    {
        void Init(ProviderContext providerContext);

        List<CommandItem> GetItems(ProcessIdentifier process);

        bool IsUpdated();
    }
}
