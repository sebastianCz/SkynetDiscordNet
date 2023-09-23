using System.Linq.Expressions;

public class Mapper<TSource, TTarget>
{
    public Mapper<TSource, TTarget> Map<TProperty>(
        Expression<Func<TSource, TProperty>> from,
        Expression<Func<TTarget, TProperty>> to
    )
    {
        // store mappings
        return this;
    }

    public TTarget To(TSource source)
    {
        // map from source to target
        return default;
    }

    public TSource From(TTarget target)
    {
        // reverse map from target to source
        return default;
    }
}