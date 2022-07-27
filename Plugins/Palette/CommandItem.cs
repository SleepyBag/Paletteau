using Newtonsoft.Json;
using Paletteau.Infrastructure;
using Paletteau.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palette
{
    internal class CommandItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("subTitle")]
        public string SubTitle { get; set; }

        /// <summary>
        /// return true to hide wox after select result
        /// </summary>
        public Func<ActionContext, Process, bool> Action { get; set; }

        public CommandItem() { }
    }
}
