using System.Diagnostics;
using Akka.Actor;
using Akka.Dispatch;
using Servus.Akka.Messaging;
using Servus.Core.Collections;
using Servus.Core.Diagnostics;
using Servus.Core.Reflection;

namespace Servus.Akka.Diagnostics;

public class TracedMessageActor : UntypedActor
{
    private readonly HandlerRegistry _handlerRegistry = new();
    private readonly Type _activitySourceType;

    public TracedMessageActor()
    {
        _activitySourceType = GetType();
    }

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="message">TBD</param>
    protected sealed override void OnReceive(object message)
    {
        // Seal the method so that implementors cannot use it. They should only use Receive and Become
        using var activity = message.InvokeIf<IWithTracing, Activity?>(t =>
            ActivitySourceRegistry.StartActivity(_activitySourceType, t.GetType().Name, t));
        ExecuteMessageHandler(message);
    }

    private void ExecuteMessageHandler(object message)
    {
        if (message is IMessageEnvelope envelope)
        {
            message = envelope.Message;
        }

        var wasHandled = _handlerRegistry.Handle(message);
        if (!wasHandled)
            Unhandled(message);
    }

    /// <summary>
    /// Changes the actor's behavior and replaces the current receive handler with the specified handler.
    /// </summary>
    /// <param name="configure">Configures the new handler by calling the different Receive overloads.</param>
    protected void Become(Action configure)
    {
        _handlerRegistry.Clear();

        base.Become(ExecuteMessageHandler);
        
        CreateNewHandler(configure);
    }

    /// <summary>
    /// Changes the actor's behavior and replaces the current receive handler with the specified handler.
    /// The current handler is stored on a stack, and you can revert to it by calling <see cref="ActorBase.UnbecomeStacked"/>
    /// <remarks>Please note, that in order to not leak memory, make sure every call to <see cref="BecomeStacked"/>
    /// is matched with a call to <see cref="ActorBase.UnbecomeStacked"/>.</remarks>
    /// </summary>
    /// <param name="configure">Configures the new handler by calling the different Receive overloads.</param>
    protected void BecomeStacked(Action configure)
    {
        base.BecomeStacked(ExecuteMessageHandler);
        CreateNewHandler(configure);
    }

    protected new void UnbecomeStacked()
    {
        _handlerRegistry.Pop();
        base.UnbecomeStacked();
    }
    
    private void CreateNewHandler(Action configure)
    {
        if (_handlerRegistry.Count != 0) _handlerRegistry.Stash();
        configure();
    }

    private static Action<T> WrapAsyncHandler<T>(Func<T, Task> asyncHandler)
    {
        return m =>
        {
            ActorTaskScheduler.RunTask(Wrap);
            return;

            Task Wrap() => asyncHandler(m);
        };
    }

    /// <summary>
    /// Registers an asynchronous handler for incoming messages of the specified type <typeparamref name="T"/>.
    /// If <paramref name="shouldHandle"/>!=<c>null</c> then it must return true before a message is passed to <paramref name="handler"/>.
    /// <remarks>The actor will be suspended until the task returned by <paramref name="handler"/> completes.</remarks>
    /// <remarks>This method may only be called when constructing the actor or from <see cref="Become(System.Action)"/> or <see cref="BecomeStacked"/>.</remarks>
    /// <remarks>Note that handlers registered prior to this may have handled the message already. 
    /// In that case, this handler will not be invoked.</remarks>
    /// </summary>
    /// <typeparam name="T">The type of the message</typeparam>
    /// <param name="handler">The message handler that is invoked for incoming messages of the specified type <typeparamref name="T"/></param>
    /// <param name="shouldHandle">When not <c>null</c> it is used to determine if the message matches.</param>
    protected void ReceiveAsync<T>(Func<T, Task> handler, Predicate<T>? shouldHandle = null)
    {
        Receive(WrapAsyncHandler(handler), shouldHandle);
    }

    /// <summary>
    /// Registers an asynchronous handler for incoming messages of the specified type <typeparamref name="T"/>.
    /// If <paramref name="shouldHandle"/>!=<c>null</c> then it must return true before a message is passed to <paramref name="handler"/>.
    /// <remarks>The actor will be suspended until the task returned by <paramref name="handler"/> completes.</remarks>
    /// <remarks>This method may only be called when constructing the actor or from <see cref="Become(System.Action)"/> or <see cref="BecomeStacked"/>.</remarks>
    /// <remarks>Note that handlers registered prior to this may have handled the message already. 
    /// In that case, this handler will not be invoked.</remarks>
    /// </summary>
    /// <typeparam name="T">The type of the message</typeparam>
    /// <param name="shouldHandle">When not <c>null</c> it is used to determine if the message matches.</param>
    /// <param name="handler">The message handler that is invoked for incoming messages of the specified type <typeparamref name="T"/></param>
    protected void ReceiveAsync<T>(Predicate<T>? shouldHandle, Func<T, Task> handler)
    {
        Receive(WrapAsyncHandler(handler), shouldHandle);
    }

    /// <summary>
    /// Registers an asynchronous handler for incoming messages of the specified <paramref name="messageType"/>.
    /// If <paramref name="shouldHandle"/>!=<c>null</c> then it must return true before a message is passed to <paramref name="handler"/>.
    /// <remarks>The actor will be suspended until the task returned by <paramref name="handler"/> completes.</remarks>
    /// <remarks>This method may only be called when constructing the actor or from <see cref="Become(Action)"/> or <see cref="BecomeStacked"/>.</remarks>
    /// <remarks>Note that handlers registered prior to this may have handled the message already. 
    /// In that case, this handler will not be invoked.</remarks>
    /// </summary>
    /// <param name="messageType">The type of the message</param>
    /// <param name="handler">The message handler that is invoked for incoming messages of the specified <paramref name="messageType"/></param>
    /// <param name="shouldHandle">When not <c>null</c> it is used to determine if the message matches.</param>
    protected void ReceiveAsync(Type messageType, Func<object, Task> handler, Predicate<object>? shouldHandle = null)
    {
        Receive(messageType, WrapAsyncHandler(handler), shouldHandle);
    }

    /// <summary>
    /// Registers an asynchronous handler for incoming messages of the specified <paramref name="messageType"/>.
    /// If <paramref name="shouldHandle"/>!=<c>null</c> then it must return true before a message is passed to <paramref name="handler"/>.
    /// <remarks>The actor will be suspended until the task returned by <paramref name="handler"/> completes.</remarks>
    /// <remarks>This method may only be called when constructing the actor or from <see cref="Become(Action)"/> or <see cref="BecomeStacked"/>.</remarks>
    /// <remarks>Note that handlers registered prior to this may have handled the message already. 
    /// In that case, this handler will not be invoked.</remarks>
    /// </summary>
    /// <param name="messageType">The type of the message</param>
    /// <param name="shouldHandle">When not <c>null</c> it is used to determine if the message matches.</param>
    /// <param name="handler">The message handler that is invoked for incoming messages of the specified <paramref name="messageType"/></param>
    protected void ReceiveAsync(Type messageType, Predicate<object> shouldHandle, Func<object, Task> handler)
    {
        Receive(messageType, WrapAsyncHandler(handler), shouldHandle);
    }

    /// <summary>
    /// Registers an asynchronous handler for incoming messages of any type.
    /// <remarks>The actor will be suspended until the task returned by <paramref name="handler"/> completes.</remarks>
    /// <remarks>This method may only be called when constructing the actor or from <see cref="Become(Action)"/> or <see cref="BecomeStacked"/>.</remarks>
    /// <remarks>Note that handlers registered prior to this may have handled the message already. 
    /// In that case, this handler will not be invoked.</remarks>
    /// </summary>
    /// <param name="handler">The message handler that is invoked for all</param>
    protected void ReceiveAnyAsync(Func<object, Task> handler)
    {
        ReceiveAny(WrapAsyncHandler(handler));
    }

    /// <summary>
    /// Registers a handler for incoming messages of the specified type <typeparamref name="T"/>.
    /// If <paramref name="shouldHandle"/>!=<c>null</c> then it must return true before a message is passed to <paramref name="handler"/>.
    /// <remarks>This method may only be called when constructing the actor or from <see cref="Become(Action)"/> or <see cref="BecomeStacked"/>.</remarks>
    /// <remarks>Note that handlers registered prior to this may have handled the message already. 
    /// In that case, this handler will not be invoked.</remarks>
    /// </summary>
    /// <typeparam name="T">The type of the message</typeparam>
    /// <param name="handler">The message handler that is invoked for incoming messages of the specified type <typeparamref name="T"/></param>
    /// <param name="shouldHandle">When not <c>null</c> it is used to determine if the message matches.</param>
    /// <exception cref="InvalidOperationException">This exception is thrown if this method is called outside the actor's constructor or from <see cref="Become(Action)"/>.</exception>
    protected void Receive<T>(Action<T> handler, Predicate<T>? shouldHandle = null)
    {
        _handlerRegistry.Register(shouldHandle ?? (_ => true), handler);
    }

    /// <summary>
    /// Registers a handler for incoming messages of the specified type <typeparamref name="T"/>.
    /// If <paramref name="shouldHandle"/>!=<c>null</c> then it must return true before a message is passed to <paramref name="handler"/>.
    /// <remarks>This method may only be called when constructing the actor or from <see cref="Become(Action)"/> or <see cref="BecomeStacked"/>.</remarks>
    /// <remarks>Note that handlers registered prior to this may have handled the message already. 
    /// In that case, this handler will not be invoked.</remarks>
    /// </summary>
    /// <typeparam name="T">The type of the message</typeparam>
    /// <param name="handler">The message handler that is invoked for incoming messages of the specified type <typeparamref name="T"/></param>
    /// <param name="shouldHandle">When not <c>null</c> it is used to determine if the message matches.</param>
    /// <exception cref="InvalidOperationException">This exception is thrown if this method is called the actor's constructor or from <see cref="Become(Action)"/>.</exception>
    protected void Receive<T>(Predicate<T>? shouldHandle, Action<T> handler)
    {
        Receive(handler, shouldHandle);
    }

    /// <summary>
    /// Registers a handler for incoming messages of the specified <paramref name="messageType"/>.
    /// If <paramref name="shouldHandle"/>!=<c>null</c> then it must return true before a message is passed to <paramref name="handler"/>.
    /// <remarks>This method may only be called when constructing the actor or from <see cref="Become(Action)"/> or <see cref="BecomeStacked"/>.</remarks>
    /// <remarks>Note that handlers registered prior to this may have handled the message already. 
    /// In that case, this handler will not be invoked.</remarks>
    /// </summary>
    /// <param name="messageType">The type of the message</param>
    /// <param name="handler">The message handler that is invoked for incoming messages of the specified <paramref name="messageType"/></param>
    /// <param name="shouldHandle">When not <c>null</c> it is used to determine if the message matches.</param>
    /// <exception cref="InvalidOperationException">This exception is thrown if this method is called outside the actor's constructor or from <see cref="Become(Action)"/>.</exception>
    protected void Receive(Type messageType, Action<object> handler, Predicate<object>? shouldHandle = null)
    {
        _handlerRegistry.Register(messageType, shouldHandle ?? (_ => true), handler);
    }

    /// <summary>
    /// Registers a handler for incoming messages of the specified <paramref name="messageType"/>.
    /// If <paramref name="shouldHandle"/>!=<c>null</c> then it must return true before a message is passed to <paramref name="handler"/>.
    /// <remarks>This method may only be called when constructing the actor or from <see cref="Become(Action)"/> or <see cref="BecomeStacked"/>.</remarks>
    /// <remarks>Note that handlers registered prior to this may have handled the message already. 
    /// In that case, this handler will not be invoked.</remarks>
    /// </summary>
    /// <param name="messageType">The type of the message</param>
    /// <param name="handler">The message handler that is invoked for incoming messages of the specified <paramref name="messageType"/></param>
    /// <param name="shouldHandle">When not <c>null</c> it is used to determine if the message matches.</param>
    /// <exception cref="InvalidOperationException">This exception is thrown if this method is called outside the actor's constructor or from <see cref="Become(Action)"/>.</exception>
    protected void Receive(Type messageType, Predicate<object> shouldHandle, Action<object> handler)
    {
        Receive(messageType, handler, shouldHandle);
    }

    /// <summary>
    /// Registers a handler for incoming messages of any type.
    /// <remarks>This method may only be called when constructing the actor or from <see cref="Become(Action)"/> or <see cref="BecomeStacked"/>.</remarks>
    /// <remarks>Note that handlers registered prior to this may have handled the message already. 
    /// In that case, this handler will not be invoked.</remarks>
    /// </summary>
    /// <param name="handler">The message handler that is invoked for all</param>
    /// <exception cref="InvalidOperationException">This exception is thrown if this method is called outside the actor's constructor or from <see cref="Become(Action)"/>.</exception>
    protected void ReceiveAny(Action<object> handler)
    {
        _handlerRegistry.Register(handler);
    }

    protected void ReplyTraced(object message) => Sender.TellTraced(message);
    protected void Reply(object message) => Sender.Tell(message);
}