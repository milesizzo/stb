using System.Collections.Generic;

namespace StopTheBoats.Templates
{
    public class TemplateStore<T> where T : ITemplate
    {
        protected Dictionary<string, T> store = new Dictionary<string, T>();

        public TemplateStore()
        {
        }

        public void Add(string key, T obj)
        {
            this.store[key] = obj;
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
    }
}
