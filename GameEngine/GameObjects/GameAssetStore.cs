using Microsoft.Xna.Framework.Content;
using GameEngine.Content;
using GameEngine.Templates;
using CommonLibrary.Serializing;
using System.Linq;
using System;

namespace GameEngine.GameObjects
{
    public class GameAssetStore : AssetStore
    {
        private readonly GameObjectTemplateStore objects = new GameObjectTemplateStore();

        public GameAssetStore(string name, ContentManager content) : base(name, content)
        {
        }

        public GameObjectTemplateStore Objects { get { return this.objects; } }

        #region Serialization

        internal override void Write(ISerializer context)
        {
            base.Write(context);
            context.Write("objects", this.Objects, (child, objects) =>
            {
                var items = objects.All.Select(kvp => kvp.Value).ToList();
                child.WriteList("store", items, (grandchild, obj) =>
                {
                    grandchild.Write("type", obj.GetType().AssemblyQualifiedName);
                    obj.Write(grandchild);
                });
            });
        }

        internal override void Read(IDeserializer context)
        {
            base.Read(context);
            context.ReadList("objects", (child) =>
            {
                var type = Type.GetType(child.Read<string>("type"));
                var name = child.Read<string>("name");
                var obj = Activator.CreateInstance(type, name) as GameObjectTemplate;
                if (obj == null)
                {
                    throw new InvalidCastException($"Expected a GameObjectTemplate but got {type.Name}");
                }
                obj.Read(this, child);
                this.Objects.AddOrReplace(obj);
                return obj;
            });
        }

        #endregion
    }
}