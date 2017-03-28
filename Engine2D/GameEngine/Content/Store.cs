using CommonLibrary.Serializing;
using GameEngine.Templates;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameEngine.Content
{
    public class Store : TemplateStore<AssetStore>
    {
        private readonly ContentManager content;

        public Store(ContentManager content)
        {
            this.content = content;
        }

        public void LoadFromJson(string filename)
        {
            using (var serializer = new MgiJsonSerializer(filename, SerializerMode.Read))
            {
                this.Load(serializer.Context);
            }
        }

        public void SaveToJson(string key, string filename)
        {
            using (var serializer = new MgiJsonSerializer(filename, SerializerMode.Write))
            {
                this.Save(serializer.Context, this[key]);
            }
        }

        public void SaveAllToJson(string path)
        {
            foreach (var key in this.Keys)
            {
                this.SaveToJson(key, Path.Combine(path, $"{key}.json"));
            }
        }

        private void Load(IDeserializer context)
        {
            var typeName = context.Read<string>("type");
            var name = context.Read<string>("name");
            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new InvalidOperationException($"Could not find type: {typeName}");
            }

            AssetStore store;
            if (this.TryGet(name, out store))
            {
                if (!type.IsInstanceOfType(store))
                {
                    throw new InvalidOperationException($"The asset store {name} already exists in the store, but it's of type {store.GetType().Name} (you are trying to load {type.Name})");
                }
            }
            else
            {
                store = Activator.CreateInstance(type, name, this.content) as AssetStore;
                if (store == null)
                {
                    throw new InvalidCastException($"Expected a type derived from AssetStore but got {typeName}");
                }
                this.Add(store);
            }
            store.Read(context);
        }

        private void Save(ISerializer context, AssetStore store)
        {
            context.Write("type", store.GetType().AssemblyQualifiedName);
            context.Write("name", store.Name);
            store.Write(context);
        }

        private T Search<T>(string key) where T : class, ITemplate
        {
            T result = null;
            foreach (var assets in this.Templates)
            {
                TemplateStore<T> store;
                store = assets.Fonts as TemplateStore<T>;
                if (store == null) store = assets.Sprites as TemplateStore<T>;
                if (store == null) store = assets.Audio as TemplateStore<T>;
                if (store == null) throw new InvalidCastException($"Unknown template type: {typeof(T).Name}");
                if (store.TryGet(key, out result))
                {
                    break;
                }
            }
            if (result == null)
            {
                throw new KeyNotFoundException($"Could not find template named {key} of type {typeof(T).Name} in any store");
            }
            return result;
        }

        public T New<T>(string key) where T : AssetStore
        {
            var store = Activator.CreateInstance(typeof(T), key, this.content) as T;
            this.Add(store);
            return store;
        }

        public FontTemplate Fonts(string key)
        {
            return this.Search<FontTemplate>(key);
        }

        public FontTemplate Fonts(string assetStore, string name)
        {
            return this[assetStore].Fonts[name];
        }

        public AudioTemplate Audio(string key)
        {
            return this.Search<AudioTemplate>(key);
        }

        public AudioTemplate Audio(string assetStore, string name)
        {
            return this[assetStore].Audio[name];
        }

        public SpriteTemplate Sprites(string key)
        {
            return this.Search<SpriteTemplate>(key);
        }

        public SpriteTemplate Sprites(string assetStore, string name)
        {
            return this[assetStore].Sprites[name];
        }
    }
}
