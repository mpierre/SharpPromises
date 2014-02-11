using System;

namespace SharpPromises
{
    public interface Promise<T>
    {
        Promise<E> Then<E>(Func<T, E> func);
        Promise<E> Then<E>(Func<T, Promise<E>> func);
        Promise<T> Always(Action<bool, T, Exception> func);
        Promise<E> Always<E>(Func<bool, T, Exception, Promise<E>> func);
        Promise<T> Done(Action<T> func);
        Promise<T> Fail(Action<Exception> func);
    }
}