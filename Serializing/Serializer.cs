using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StopTheBoats.Serializing
{
    public class List<T1, T2> : List<Tuple<T1, T2>> { }

    public delegate void ReadAction<T>(IDeserializer context, out T obj);

    public delegate void ReadUserAction<T, TUser>(TUser userData, IDeserializer context, out T obj);

    public interface IDeserializer
    {
        T Read<T>(string key);

        T Read<T>(string key, ReadAction<T> action);

        T Read<T, TUser>(string key, TUser userData, ReadUserAction<T, TUser> action);

        IList<T> ReadList<T>(string key);

        IList<T> ReadList<T>(string key, ReadAction<T> reader);

        IList<T> ReadList<T, TUser>(string key, TUser userData, ReadUserAction<T, TUser> reader);
    }

    public interface ISerializer
    {
        void Write<T>(string key, T value);

        void Write<T>(string key, T value, Action<ISerializer, T> writer);

        void WriteList<T>(string key, IList<T> values);

        void WriteList<T>(string key, IList<T> values, Action<ISerializer, T> writer);
    }

    public abstract class SerializerContext : IDeserializer, ISerializer
    {
        public abstract T Read<T>(string key);

        private static void ReadHelper<T>(ReadAction<T> userData, IDeserializer context, out T obj)
        {
            userData(context, out obj);
        }

        public T Read<T>(string key, ReadAction<T> func)
        {
            return this.Read<T, ReadAction<T>>(key, func, ReadHelper);
        }

        public abstract T Read<T, TUser>(string key, TUser userData, ReadUserAction<T, TUser> action);

        public abstract IList<T> ReadList<T>(string key);

        public IList<T> ReadList<T>(string key, ReadAction<T> reader)
        {
            return this.ReadList<T, ReadAction<T>>(key, reader, ReadHelper);
        }

        public abstract IList<T> ReadList<T, TUser>(string key, TUser userData, ReadUserAction<T, TUser> reader);

        public abstract void Write<T>(string key, T value);

        public abstract void Write<T>(string key, T value, Action<ISerializer, T> writer);

        public abstract void WriteList<T>(string key, IList<T> values);

        public abstract void WriteList<T>(string key, IList<T> values, Action<ISerializer, T> writer);
    }

    public abstract class Serializer<TContext> where TContext : SerializerContext
    {
        //private readonly List<Type, Func<object, TToken>> serializer = new List<Type, Func<object, TToken>>();

        public Serializer()
        {
        }

        public static bool IsAnonymousType(Type type)
        {
            var hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            var nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;
            return isAnonymousType;
        }

        /*public void AddSerializer(Type type, Func<object, TToken> serializer)
        {
            this.serializer.Add(Tuple.Create(type, serializer));
        }*/

        public abstract TContext CreateContext();

        /*public TToken Serialize(object obj)
        {
            var type = obj.GetType();
            if (IsAnonymousType(type))
            {
                var childContext = this.CreateContext();
                foreach (var prop in obj.GetType().GetProperties())
                {
                    childContext.Set(prop.Name, prop.GetValue(obj));
                }
                return childContext.ToToken();
            }
            else
            {
                foreach (var entry in this.serializer)
                {
                    if (entry.Item1.IsAssignableFrom(type))
                    {
                        return entry.Item2(obj);
                    }
                }
                throw new InvalidOperationException($"No way to write objects of type {type.Name}");
            }
        }*/

        public TContext Serialize()
        {
            return this.CreateContext();
        }

        public abstract TContext Load(string filename);

        public abstract void Save(string filename, TContext context);
    }

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

        public override T Read<T>(string key)
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

        public override void Write<T>(string key, T value)
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
        public MgiJsonSerializer()
        {
        }

        public override JsonContext CreateContext()
        {
            return new JsonContext();
        }

        public override void Save(string filename, JsonContext context)
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            using (var writer = new StreamWriter(filename + ".json"))
            using (var json = new JsonTextWriter(writer))
            {
                serializer.Serialize(json, context.Current);
            }
        }

        public override JsonContext Load(string filename)
        {
            var serializer = new JsonSerializer();
            using (var reader = new StreamReader(filename + ".json"))
            using (var json = new JsonTextReader(reader))
            {
                return new JsonContext(serializer.Deserialize<JObject>(json));
            }
        }
    }
}
