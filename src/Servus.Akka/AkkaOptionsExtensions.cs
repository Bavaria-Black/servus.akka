using Akka.Util;

namespace Servus.Akka;

public static class AkkaOptionsExtensions
{
    public static TOut Match<TIn, TOut>(this Option<TIn> option, Func<TIn, TOut> some, Func<TOut> none)
    {
        return option.HasValue ? some(option.Value) : none();
    }

    public static void Match<T>(this Option<T> option, Action<T> some, Action none)
    {
        if (option.HasValue)
        {
            some(option.Value);
        }
        else
        {
            none();
        }
    }
}