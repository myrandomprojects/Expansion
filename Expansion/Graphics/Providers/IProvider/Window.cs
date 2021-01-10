using Expansion.Graphics.Classes;
using System;
using System.Collections.Generic;

namespace Expansion.Graphics.Providers.IProvider
{
    public abstract class Window : Widget 
    {
        public List<Widget> Widgets => Children;
        public abstract bool IsOpen { get; protected set; }

        public abstract void Tick();

        /*
        public virtual void AddWidget(Widget widget)
        {
            Widgets.Add(widget);
        }
        */
    }
}