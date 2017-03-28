﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Templates;

namespace GameEngine.Content
{
    public class FontStore : TemplateStore<FontTemplate>
    {
        private ContentManager content;

        public FontStore(ContentManager content)
        {
            this.content = content;
        }

        public FontTemplate Load(string name, string assetName)
        {
            return new FontTemplate(name, assetName, this.content.Load<SpriteFont>(assetName));
        }
    }
}
