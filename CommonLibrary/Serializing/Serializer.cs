using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CommonLibrary.Serializing
{
    public class List<T1, T2> : List<Tuple<T1, T2>> { }

    public delegate void GenericReadAction(IDeserializer context, out object obj);

    public delegate void GenericWriteAction(ISerializer context, object obj);

    public delegate void ReadAction<T>(IDeserializer context, out T obj);

    public delegate void ReadUserAction<T, TUser>(TUser userData, IDeserializer context, out T obj);

    public interface IDeserializer
    {
        T Read<T>(string key);

        T Read<T>(string key, ReadAction<T> action);

        T Read<T, TUser>(string key, TUser userData, ReadUserAction<T, TUser> action);

        T Read<T>(string key, Func<IDeserializer, T> reader);

        void ReadInto<T>(string key, T obj, Action<IDeserializer, T> reader);

        IList<T> ReadList<T>(string key);

        IList<T> ReadList<T>(string key, ReadAction<T> reader);

        IList<T> ReadList<T, TUser>(string key, TUser userData, ReadUserAction<T, TUser> reader);

        IList<T> ReadList<T>(string key, Func<IDeserializer, T> reader);
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
        /*private static readonly Dictionary<Type, Converter> Converters = new Dictionary<Type, Converter>();

        private class Converter
        {
            private object read;
            private object write;

            public Converter(GenericWriteAction write)
            {
            }

            public void SetWrite<T>(Action<ISerializer, T> convert)
            {
                this.write = convert;
            }

            public void SetRead<T>(ReadAction<T> convert)
            {
                this.read = convert;
            }

            public void Read<T>(IDeserializer context, out T obj)
            {
                ((ReadAction<T>)this.read)(context, out obj);
            }
            
            public void Write<T>(ISerializer context, T obj)
            {
                ((Action<ISerializer, T>)this.write)(context, obj);
            }
        }*/

        public virtual T Read<T>(string key)
        {
            /*var type = typeof(T);
            Converter convert;
            if (Converters.TryGetValue(type, out convert))
            {
                T obj;
                convert.Read(this, out obj);
                return obj;
            }*/
            return this.ReadImpl<T>(key);
        }

        protected abstract T ReadImpl<T>(string key);

        /*public static void RegisterType<T>(ReadAction<T> read, Action<ISerializer, T> write)
        {
            var converter = new Converter((context, obj) => write(context, (T)obj));
            converter.SetRead(read);
            Converters[typeof(T)] = converter;
        }*/

        private static void ReadHelper<T>(ReadAction<T> userData, IDeserializer context, out T obj)
        {
            userData(context, out obj);
        }

        public T Read<T>(string key, ReadAction<T> func)
        {
            return this.Read<T, ReadAction<T>>(key, func, ReadHelper);
        }

        public T Read<T>(string key, Func<IDeserializer, T> reader)
        {
            return this.Read<T, Func<IDeserializer, T>>(key, reader, ReadFuncHelper);
        }

        public void ReadInto<T>(string key, T obj, Action<IDeserializer, T> reader)
        {
            this.Read<T>(key, (ctx) =>
            {
                reader(ctx, obj);
                return obj;
            });
        }

        public abstract T Read<T, TUser>(string key, TUser userData, ReadUserAction<T, TUser> action);

        public abstract IList<T> ReadList<T>(string key);

        public IList<T> ReadList<T>(string key, ReadAction<T> reader)
        {
            return this.ReadList<T, ReadAction<T>>(key, reader, ReadHelper);
        }

        public abstract IList<T> ReadList<T, TUser>(string key, TUser userData, ReadUserAction<T, TUser> reader);

        public void Write<T>(string key, T value)
        {
            /*var type = typeof(T);
            Converter convert;
            if (Converters.TryGetValue(type, out convert))
            {
                convert.Write(this, value);
            }
            else
            {*/
                this.WriteImpl<T>(key, value);
            //}
        }
        
        public static void ReadFuncHelper<T>(Func<IDeserializer, T> userData, IDeserializer context, out T obj)
        {
            obj = userData(context);
        }

        public IList<T> ReadList<T>(string key, Func<IDeserializer, T> reader)
        {
            return this.ReadList<T, Func<IDeserializer, T>>(key, reader, ReadFuncHelper);
        }

        protected abstract void WriteImpl<T>(string key, T value);

        public abstract void Write<T>(string key, T value, Action<ISerializer, T> writer);

        public abstract void WriteList<T>(string key, IList<T> values);

        public abstract void WriteList<T>(string key, IList<T> values, Action<ISerializer, T> writer);
    }

    public enum SerializerMode
    {
        Read,
        Write,
    }

    public abstract class Serializer<TContext> : IDisposable where TContext : SerializerContext
    {
        private readonly string filename;
        private readonly SerializerMode mode;
        private readonly TContext context;

        public Serializer(string filename, SerializerMode mode)
        {
            this.filename = filename;
            this.mode = mode;
            switch (this.mode)
            {
                case SerializerMode.Read:
                    this.context = this.Load();
                    break;
                case SerializerMode.Write:
                    this.context = this.CreateEmptyContext();
                    break;
            }
        }

        public TContext Context { get { return this.context; } }

        public string Filename { get { return this.filename; } }

        public void Dispose()
        {
            if (this.mode == SerializerMode.Write)
            {
                this.Save();
            }
        }

        protected abstract TContext CreateEmptyContext();

        protected abstract TContext Load();

        protected abstract void Save();
    }
}
