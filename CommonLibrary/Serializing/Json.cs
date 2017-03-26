using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Serializing
{
    public class JsonContext : SerializerContext
    {
        public JObject Current;

        public JsonContext() : this(new JObject())
        {
        }

        public JsonContext(JObject current)
        {
            this.Current = current;
        }

        protected override T ReadImpl<T>(string key)
        {
            return this.Current[key].Value<T>();
        }

        public override T Read<T, TUser>(string key, TUser userData, ReadUserAction<T, TUser> action)
        {
            var child = this.Current[key].Value<JObject>();
            var context = new JsonContext(child);
            T obj;
            action(userData, context, out obj);
            return obj;
        }

        public override IList<T> ReadList<T>(string key)
        {
            var result = new List<T>();
            foreach (var element in this.Current[key])
            {
                result.Add(element.Value<T>());
            }
            return result;
        }

        public override IList<T> ReadList<T, TUser>(string key, TUser userData, ReadUserAction<T, TUser> reader)
        {
            var result = new List<T>();
            foreach (var element in this.Current[key])
            {
                T obj;
                reader(userData, new JsonContext(element.Value<JObject>()), out obj);
                result.Add(obj);
            }
            return result;
        }

        protected override void WriteImpl<T>(string key, T value)
        {
            this.Current[key] = JToken.FromObject(value);
        }

        public override void Write<T>(string key, T value, Action<ISerializer, T> writer)
        {
            var context = new JsonContext();
            writer(context, value);
            this.Current[key] = context.Current;
        }

        public override void WriteList<T>(string key, IList<T> values)
        {
            var obj = new JArray();
            foreach (var item in values)
            {
                obj.Add(JToken.FromObject(item));
            }
            this.Current[key] = obj;
        }

        public override void WriteList<T>(string key, IList<T> values, Action<ISerializer, T> writer)
        {
            var obj = new JArray();
            foreach (var item in values)
            {
                var context = new JsonContext();
                writer(context, item);
                obj.Add(context.Current);
            }
            this.Current[key] = obj;
        }
    }

    public class MgiJsonSerializer : Serializer<JsonContext>
    {
        public MgiJsonSerializer(string filename, SerializerMode mode) : base(filename, mode) { }

        protected override JsonContext CreateEmptyContext()
        {
            return new JsonContext();
        }

        protected override void Save()
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            using (var writer = new StreamWriter(this.Filename))
            using (var json = new JsonTextWriter(writer))
            {
                serializer.Serialize(json, this.Context.Current);
            }
        }

        protected override JsonContext Load()
        { 
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var reader = new StreamReader(this.Filename))
            using (var json = new JsonTextReader(reader))
            {
                return new JsonContext(serializer.Deserialize<JObject>(json));
            }
        }
    }
}
