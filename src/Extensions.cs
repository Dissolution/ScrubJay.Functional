namespace ScrubJay.Functional;

/// <summary>
/// Extensions on <see cref="IEnumerable{T}"/> and similar types
/// </summary>
[PublicAPI]
public static class Extensions
{
    extension<T>(IEnumerable<T>? enumerable)
    {
        public IEnumerable<TNew> SelectWhere<TNew>(Func<T, Option<TNew>>? selectWhere)
        {
            if (enumerable is null || selectWhere is null)
                yield break;
            
            foreach (T value in enumerable)
            {
                if (selectWhere(value).IsSome(out var newValue))
                {
                    yield return newValue;
                }
            }
        }
        
        public IEnumerable<TNew> SelectWhere<TNew>(Func<T, Result<TNew>>? selectWhere)
        {
            if (enumerable is null || selectWhere is null)
                yield break;
            
            foreach (T value in enumerable)
            {
                if (selectWhere(value).IsOk(out var newValue))
                {
                    yield return newValue;
                }
            }
        }
        
        public IEnumerable<TNew> SelectWhere<TNew, TError>(Func<T, Result<TNew, TError>>? selectWhere)
        {
            if (enumerable is null || selectWhere is null)
                yield break;
            
            foreach (T value in enumerable)
            {
                if (selectWhere(value).IsOk(out var newValue))
                {
                    yield return newValue;
                }
            }
        }
    }
}