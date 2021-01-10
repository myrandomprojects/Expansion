using Expansion.Engine.Math3D;
using Expansion.Graphics.Providers.IProvider;
using System;
using System.Collections.Generic;

namespace Expansion.Graphics.Classes
{
    public class Widget
    {
        private Widget _parent;


        internal object ProviderTag { get; set; }
        internal IRenderer Renderer => Window.Renderer;
        public Window Window { get; private set; }
        public object Tag { get; set; }

        public Widget Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
                Window = _parent.Window ?? _parent as Window;
            }
        }
        public List<Widget> Children { get; } = new List<Widget>();

        public int Width { get; set; }
        public int Height { get; set; }

        public Widget AddWidget(Type type)
        {
            var widget = Renderer.CreateWidget(this, type, new Rectangle { Left = 0, Top = 0, Right = Parent.Width, Bottom = Parent.Height }) as Widget;
            Children.Add(widget);
            return widget;
        }

        public virtual void SetLayout(int x, int y, int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}