using System;
using System.Collections.Generic;

namespace GameEngine.Templates
{
    public class TemplateStore<T> where T : class, ITemplate
    {
        protected Dictionary<string, T> store = new Dictionary<string, T>();

        public TemplateStore()
        {
        }

        public void Add(T obj)
        {
            var key = obj.Name;
            if (this.store.ContainsKey(key))
            {
                throw new InvalidOperationException(string.Format("Asset already exists in store: {0}", key));
            }
            this.store[key] = obj;
        }

        public TSubClass GetOrAdd<TSubClass>(string key, Func<string, TSubClass> createFunc) where TSubClass : class, T
        {
            T obj;
            if (!this.store.TryGetValue(key, out obj))
            {
                var created = createFunc(key);
                this.Add(created);
                return created;
            }
            var cached = obj as TSubClass;
            if (cached == null)
            {
                throw new InvalidOperationException(string.Format("Invalid type; expected {0} but got {1}", typeof(TSubClass).Name, obj.GetType().Name));
            }
            return cached;
        }

        public void AddOrReplace(T template)
        {
            this.store[template.Name] = template;
        }

        public void AddOrReplace(IEnumerable<T> templates)
        {
            foreach (var template in templates)
            {
                this.AddOrReplace(template);
            }
        }

        public T this[string key]
        {
            get
            {
                T template;
                if (!this.store.TryGetValue(key, out template))
                {
                    throw new KeyNotFoundException(string.Format("Could not find asset: {0}", key));
                }
                return template;
            }
        }

        public bool TryGet(string key, out T obj)
        {
            return this.store.TryGetValue(key, out obj);
        }

        public TSubClass Get<TSubClass>(string key) where TSubClass : class, T
        {
            T obj;
            if (!this.TryGet(key, out obj))
            {
                throw new KeyNotFoundException($"Could not find template with name {key}");
            }
            var asSubClass = obj as TSubClass;
            if (asSubClass == null)
            {
                throw new InvalidCastException($"Expected type {typeof(TSubClass).Name} but got {obj.GetType().Name}");
            }
            return asSubClass;
        }

        public IEnumerable<KeyValuePair<string, T>> All
        {
            get { return this.store; }
        }

        public IEnumerable<string> Keys
        {
            get { return this.store.Keys; }
        }

        public IEnumerable<T> Templates
        {
            get { return this.store.Values; }
        }
    }
}
