using System;
using System.Collections.Generic;

namespace GameEngine.Templates
{
    public class TemplateStore<T> : ITemplate where T : class, ITemplate
    {
        protected Dictionary<string, T> store = new Dictionary<string, T>();

        public TemplateStore()
        {
        }

        public void Add(string key, T obj)
        {
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
                this.Add(key, created);
                return created;
            }
            var cached = obj as TSubClass;
            if (cached == null)
            {
                throw new InvalidOperationException(string.Format("Invalid type; expected {0} but got {1}", typeof(TSubClass).Name, obj.GetType().Name));
            }
            return cached;
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

        public TSubClass Get<TSubClass>(string key) where TSubClass : class, T
        {
            var obj = this[key];
            var typed = obj as TSubClass;
            if (typed == null)
            {
                throw new InvalidOperationException(string.Format("Invalid type; expected {0} but got {1}", typeof(TSubClass).Name, obj.GetType().Name));
            }
            return typed;
        }

        public bool TryGet(string key, out T obj)
        {
            return this.store.TryGetValue(key, out obj);
        }

        public IEnumerable<KeyValuePair<string, T>> All
        {
            get { return this.store; }
        }
    }
}
