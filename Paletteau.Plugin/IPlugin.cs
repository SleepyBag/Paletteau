using Paletteau.Infrastructure;
using System.Collections.Generic;

namespace Paletteau.Plugin
{
    public interface IPlugin
    {
        List<Result> Query(Query query);
        void Init(PluginInitContext context);
    }
}