using System.IO;
using System.Linq;
using System;
using System.Runtime.InteropServices;

namespace Sample
{
    /// <summary>
    /// <para>Interface for the application service, which can handle multiple commands. 
    /// </para>
    /// <para>Application server will host multiple application services, passing commands to them
    /// via this interface. Additional cross-cutting concerns can be wrapped around as necessary 
    ///  (<see cref="LoggingWrapper"/>)</para> 
    /// <para>This is only one option of wiring things together. </para>
    /// </summary>
    public interface IApplicationService
    {
        void Execute(ICommand cmd);
    }
    /// <summary><para>
    /// Interface, which marks our events to provide some strong-typing.
    /// In real-world systems we can have more fine-grained interfaces</para> 
    /// </summary>
    public interface IEvent { }

    /// <summary>
    /// <para>Interface for commands, which we send to the application server.
    /// In real-world systems we can have more fine-grained interfaces</para>
    /// </summary>
    public interface ICommand { }

    /// <summary>
    /// Base class for all identities. It might not seem that useful in this sample,
    /// however becomes really useful in the projects, where you have dozens of aggregate
    /// types mixed with stateless (functional) services
    /// </summary>
    public interface IIdentity { }

    public interface IDocumentReader<in TKey, TView>
    {
        /// <summary>
        /// Gets the view with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="view">The view.</param>
        /// <returns>
        /// true, if it exists
        /// </returns>
        bool TryGet(TKey key, out TView view);
    }

    //public interface IDocumentWriter<in TKey, TEntity>
    //{
    //    TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists);
    //    bool TryDelete(TKey key);
    //}

    public interface IDocumentStrategy
    {
        string GetEntityBucket<TEntity>();
        string GetEntityLocation<TEntity>(object key);


        void Serialize<TEntity>(TEntity entity, Stream stream);
        TEntity Deserialize<TEntity>(Stream stream);
    }
    public enum AddOrUpdateHint
    {
        ProbablyExists,
        ProbablyDoesNotExist
    }

    [ComVisible(true)]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct unit
    {
        public static readonly unit it = default(unit);
    }
}