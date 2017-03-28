using System;
using System.Collections.Generic;

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
        public abstract T Read<T>(string key);

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

        public abstract void Write<T>(string key, T value);
        
        public static void ReadFuncHelper<T>(Func<IDeserializer, T> userData, IDeserializer context, out T obj)
        {
            obj = userData(context);
        }

        public IList<T> ReadList<T>(string key, Func<IDeserializer, T> reader)
        {
            return this.ReadList<T, Func<IDeserializer, T>>(key, reader, ReadFuncHelper);
        }

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
