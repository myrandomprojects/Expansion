using Expansion.Graphics.Providers.IProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expansion.Graphics.Providers
{
    class ProviderManager
    {
        static List<IRenderer> renderers = new List<IRenderer>();

        static public IRenderer TryGetRenderer(string name)
        {
            var renderer = renderers.Find(r => r.Name == name);
            if (renderer == null)
            {
                var type = typeof(IRenderer);

                renderer = type.Assembly.GetTypes()
                    .First(t => t.IsSubclassOf(type) && !t.IsAbstract && t.GetProperty("StaticName").GetValue(null) as string == name)
                    .GetConstructor(null).Invoke(null) as IRenderer;

                renderers.Add(renderer);
            }

            return renderer;
        }
    }
}
