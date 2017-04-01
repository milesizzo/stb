using GameEngine.Content;
using GameEngine.Scenes;
using GameEngine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.Graphics;
using GameEngine.Helpers;
using Microsoft.Xna.Framework.Input;

namespace StopTheBoats.Scenes
{
    public enum MenuItem
    {
        None,
        PlayGame,
        Editor,
        Quit,
    }

    public class MainMenuScene : UIOnlyScene
    {
        private MenuItem selectedItem = MenuItem.None;

        public MainMenuScene(string name, GraphicsDevice graphics, Store store) : base(name, graphics, store) { }

        public override void SetUp()
        {
            base.SetUp();

            this.UI.Clear();
            this.UI.DrawMouseCursor = (mouse, renderer) =>
            {
                this.Store.Sprites("Base", "mouse_cursor").DrawSprite(renderer.Screen, new Vector2(mouse.X, mouse.Y), Color.White, 0, new Vector2(0.5f), SpriteEffects.None);
            };
            this.UI.Enabled = true;

            UIElement.ScreenDimensions = new Size2(this.Graphics.Viewport.Width, this.Graphics.Viewport.Height);
            var window = new UIPanel();
            window.Origin = UIOrigin.TopCentre;
            window.Placement.RelativeX = 0.5f;
            window.Placement.RelativeY = 0.2f;
            window.Size.X = 400;
            window.Size.Y = 400;
            window.Colour = new Color(0.1f, 0.1f, 0.1f);
            window.Enabled = true;

            var menu = new UIButtonGroup(window);
            menu.Size.RelativeX = 1f;
            menu.Size.RelativeY = 0.8f;
            menu.Origin = UIOrigin.BottomCentre;
            menu.Placement.RelativeX = 0.5f;
            menu.Placement.RelativeY = 1f;
            menu.AddButton(this.Store.Fonts("Base", "envy12"), "Play vs AI", () =>
            {
                this.selectedItem = MenuItem.PlayGame;
                this.SceneEnded = true;
            });
            menu.AddButton(this.Store.Fonts("Base", "envy12"), "Polygon bounds editor", () =>
            {
                this.selectedItem = MenuItem.Editor;
                this.SceneEnded = true;
            });
            //menu.AddButton(this.Store.Fonts("Base", "envy12"), "Button 2", Color.Yellow);
            //menu.AddButton(this.Store.Fonts("Base", "envy12"), "Button 3", Color.Yellow);
            menu.AddButton(this.Store.Fonts("Base", "envy12"), "Quit immediately", () =>
            {
                this.selectedItem = MenuItem.Quit;
                this.SceneEnded = true;
            });

            /*
            window.MouseFocus += (owner) =>
            {
                var mouse = Mouse.GetState();
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    var local = window.ScreenToLocal(new Vector2(mouse.X, mouse.Y));
                    if (this.mouseHold == null)
                    {
                        this.mouseHold = local;
                    }
                    window.Origin = UIOrigin.TopLeft;
                    local -= this.mouseHold.Value;
                    window.Placement.X = local.X;
                    window.Placement.Y = local.Y;
                }
                else
                {
                    this.mouseHold = null;
                }
            };
            window.MouseLeave += (owner) =>
            {
                this.mouseHold = null;
            };
            */

            this.UI.Add(window);
        }

        public MenuItem SelectedItem { get { return this.selectedItem; } }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            /*
            if (KeyboardHelper.KeyPressed(Keys.F1))
            {
                this.SetCurrentScene("StopTheBoats");
            }
            else if (KeyboardHelper.KeyPressed(Keys.F2))
            {
                this.SetCurrentScene("BoundsEditor");
            }
            */
        }

        public override void Draw(Renderer renderer)
        {
            this.Graphics.Clear(Color.Black);
            base.Draw(renderer);
        }
    }
}
