﻿using GameEngine.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Content;
using MonoGame.Extended;
using GameEngine.Templates;
using GameEngine.UI;

namespace StopTheBoats.Scenes
{
    public class UIScene : GameAssetScene
    {
        private UIPanel window;

        public UIScene(string name, GraphicsDevice graphics, Store store) : base(name, graphics, store)
        {
        }

        public override void SetUp()
        {
            UIElement.ScreenDimensions = new Size2(this.Camera.Viewport.Width, this.Camera.Viewport.Height);
            this.window = new UIPanel();
            this.window.Origin = UIOrigin.TopCentre;
            this.window.Placement.RelativeX = 0.5f;
            this.window.Placement.RelativeY = 0.2f;
            this.window.Size.X = 400;
            this.window.Size.Y = 400;

            //this.window.ForegroundColour = Color.DarkGray;
            this.window.Colour = new Color(0.1f, 0.1f, 0.1f);

            var menu = new UIButtonGroup(this.window);
            menu.Size.RelativeX = 1f;
            //menu.Size.RelativeY = 0.8f;
            menu.Origin = UIOrigin.Centre;
            menu.Placement.RelativeX = 0.5f;
            menu.Placement.RelativeY = 0.5f;
            menu.AddButton(this.Store.Fonts("Base", "envy12"), "Button 1", Color.Yellow);
            menu.AddButton(this.Store.Fonts("Base", "envy12"), "Button 2", Color.Yellow);
            menu.AddButton(this.Store.Fonts("Base", "envy12"), "Button 3", Color.Yellow);
            menu.AddButton(this.Store.Fonts("Base", "envy12"), "Button 4", Color.Yellow);

            /*
            var button = new UIButton(this.window);
            button.Colour = Color.DarkBlue;
            button.Size = new Size2(this.window.Width / 2, 32);
            button.Position = new Vector2(this.window.Width / 2, this.window.Height / 2);
            button.Origin = UIOrigin.Centre;
            //button.Label.Size = new Size2(this.window.Size.Width, 0);
            button.Label.Position = new Vector2(button.Width / 2, button.Height / 2);
            button.Label.Text = "Testing";
            button.Label.TextColour = Color.Yellow;
            button.Label.Font = this.Store.Fonts("Base", "envy12");
            */
        }

        public override void Draw(Renderer renderer)
        {
            this.Camera.Clear(Color.Black);
            this.window.Draw(renderer.Screen);
        }

        public override void Update(GameTime gameTime)
        {
            this.window.Update(gameTime);
        }
    }
}
