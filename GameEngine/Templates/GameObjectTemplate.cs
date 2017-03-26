using CommonLibrary.Serializing;
using GameEngine.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Templates
{
    public abstract class GameObjectTemplate : ITemplate
    {
        private string name;

        protected GameObjectTemplate(string name)
        {
            this.name = name;
        }

        public string Name { get { return this.name; } }

        public virtual void Write(ISerializer context)
        {
            context.Write("name", this.Name);
        }

        public virtual void Read(GameAssetStore store, IDeserializer context)
        {
            this.name = context.Read<string>("name");
        }
    }

    public class GameObjectTemplateStore : TemplateStore<GameObjectTemplate>
    {
    }
}
