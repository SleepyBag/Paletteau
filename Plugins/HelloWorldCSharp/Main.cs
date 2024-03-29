﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paletteau.Plugin;
using Paletteau.Infrastructure;

namespace HelloWorldCSharp
{
    class Main : IPlugin
    {
        public List<Result> Query(Query query)
        {
            var result = new Result
            {
                Title = "Hello World from CSharp",
                SubTitle = $"Query: {query.Search}",
                IcoPath = Path.Combine("Images", "app.png")
            };
            return new List<Result> {result};
        }

        public void Init(PluginInitContext context)
        {
            
        }
    }
}
