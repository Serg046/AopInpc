# AopInpc

[![Build status](https://ci.appveyor.com/api/projects/status/owvt12r5l2vs4hfs?svg=true)](https://ci.appveyor.com/project/Serg046/aopinpc) [![NuGet version](https://badge.fury.io/nu/AopInpc.svg)](https://badge.fury.io/nu/AopInpc)

Castle.DynamicProxy based library that provides a way to implement INotifyPropertyChanged automatically using AOP style.

## Declaration:
```csharp
public class ViewModel : INotifyPropertyChangedCaller
{
    [Inpc]
    public virtual string Property { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    public void RaisePropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```
## Usage:
- AopInpcFactory
```csharp
static void Main(string[] args)
{
    var viewModel = AopInpcFactory.Create<ViewModel>();
    //var viewModel = AopInpcFactory.Decorate(new ViewModel());
    viewModel.PropertyChanged += (sender, eventArgs)
        => Console.WriteLine($"The property \"{eventArgs.PropertyName}\" was updated with value \"{viewModel.Property}\".");
    viewModel.Property = "Hello world!";
}
```
- DI-container
```csharp
static void Main(string[] args)
{
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<InpcInterceptor>();
    containerBuilder.RegisterType<ViewModel>()
        .EnableClassInterceptors()
        .InterceptedBy(typeof(InpcInterceptor));
    var container = containerBuilder.Build();

    var viewModel = container.Resolve<ViewModel>();
    viewModel.PropertyChanged += (sender, eventArgs)
        => Console.WriteLine($"The property \"{eventArgs.PropertyName}\" was updated with value \"{viewModel.Property}\".");
    viewModel.Property = "Hello world!";
}
```
## Output:
> The property "Property" was updated with value "Hello world!".
