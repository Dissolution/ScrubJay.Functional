# ScrubJay.Functional
Functional types for C#

### `Unit`


### `Option`~~~~


### `Result`

#### Consuming and returning Errors
Without going too deeply into functional paradigms, a common pattern is to execute a method that returns a `Result`, check if it has Errored, and if it has, do something with that error (logging perhaps) and then to return either that Error or  a new Error.

In [Rust](https://www.rust-lang.org/), it is possible to `return` from within a `match` statement, so this is easy:
```rust
// Early return on error
let mut file = match File::create("my_best_friends.txt") {
    Err(e) => return Err(e), // doesn't just return from this branch, returns from this _method_
    Ok(f) => f,
};
```

This is much harder to do fluently in C#:
```csharp
// Does not compile:
Result<int, Exception> result = Result.TryParse<int>("⁉️");
int i;
result.Match(
    ok => {
        i = ok;
    },
    error => {
        _logger.Log(error);
        return error~~~~;
    });

// Verbose:
Result<int, Exception> result = int.TryParse("???");
if (result.IsErr(out var err))
{
    _logger.Log(err);
    return err;
}
int i = result.Unwrap();    // has a redundant check for isOk that we already know is true!
```

Thus, the methods `IsSuccess` and `IsFailure` were added to extract the ok and error values at the same time:
**Beware: One of `ok` or `error` will be the default value**
```csharp
Result<int, Exception> result = Result.TryParse<int>("⁉️");
if (result.IsFailure(out var error, out var ok))
{
    _logger.Log(error);
    return error;    
}       
else
{
    // continue to use ok as a valid value
    ...
}        
```

There is also a deconstructor using `Option`
```csharp
Result<int, Exception> result = Result.TryParse<int>("⁉️");
(Option<int> asOk, Option<Exception> asError) = result;
if (asError.IsSome(out var error))
{
    _logger.Log(error);
    return error;
}
else if (asOk.IsSome(out var ok))
{
    ...
}
```