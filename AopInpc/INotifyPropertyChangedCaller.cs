using System.ComponentModel;

namespace AopInpc
{
    public interface INotifyPropertyChangedCaller : INotifyPropertyChanged
    {
        void RaisePropertyChanged(string propertyName);
    }
}
