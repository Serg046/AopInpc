# AopInpc

[![Build status](https://ci.appveyor.com/api/projects/status/owvt12r5l2vs4hfs?svg=true)](https://ci.appveyor.com/project/Serg046/aopinpc) [![NuGet version](https://badge.fury.io/nu/AopInpc.svg)](https://badge.fury.io/nu/AopInpc)

Castle.DynamicProxy based library which provides ability to implement INotifyPropertyChanged interface automatically using AOP style.

## Declaration:
```csharp
public class ViewModel : INotifyPropertyChangedCaller
{
    [InjectInpc]
    public virtual string Property { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    public void RaisePropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```
## Usage:
```csharp
static void Main(string[] args)
{
    ViewModel injectedViewModel = AopInpc.AopInpc.Create<ViewModel>();
    injectedViewModel.PropertyChanged += (sender, eventArgs)
        => Console.WriteLine($"The property \"{eventArgs.PropertyName}\" was updated with value \"{injectedViewModel.Property}\".");
    injectedViewModel.Property = "Hello world!";
}
```
## Output:
> The property "Property" was updated with value "Hello world!".
