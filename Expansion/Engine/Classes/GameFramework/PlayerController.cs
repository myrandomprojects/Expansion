using Expansion.Graphics.Classes;
using Expansion.Graphics.Providers;
using Expansion.Graphics.Providers.IProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expansion.Engine.Classes.GameFramework
{
    public class PlayerController
    {
        public Game Game { get; set; }
        public Window Window { get { return View.Window; } }
        public Widget View { get; }
        public GameObject PlayerCharacter { get { return PlayerCamera?.Owner; } }
        public Camera.Camera PlayerCamera { get; set; }

        public PlayerController(Widget view = null)
        {
            //Game = game;
            if (view == null)
            {
                var window = CreateWindow("OpenCL", "GameGame", 640, 480);
                View = window.AddWidget(typeof(SceneView));
            }
            else View = view;
        }

        public Window CreateWindow(string renderProvider, string title, int W, int H)
        {
            var renderer = ProviderManager.TryGetRenderer(renderProvider);

            var window = renderer.CreateWindow(title, W, H);
            //Windows.Add(window);

            return window;
        }

        public void Possess(GameObject character)
        {
            PlayerCamera = character.Components.OfType<Camera.Camera>().First();
        }

        internal void Tick()
        {
            throw new NotImplementedException();
        }
    }
}
