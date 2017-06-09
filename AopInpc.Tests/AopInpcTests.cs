using System;
using System.ComponentModel;
using Xunit;

namespace AopInpc.Tests
{
    public class AopInpcTests
    {
        [Fact]
        public void Create_PropWithoutInjectInpc_InpcCallIsNotInjected()
        {
            var injectedVm = AopInpc.Create<ViewModel>();
            injectedVm.PropertyChanged += (sender, args) => throw new InvalidOperationException(args.PropertyName);

            injectedVm.Prop = 5;
        }

        [Fact]
        public void Create_PropWithInjectInpc_InpcCallInjected()
        {
            var injectedVm = AopInpc.Create<ViewModel>();
            injectedVm.PropertyChanged += (sender, args) => throw new InvalidOperationException(args.PropertyName);

            var ex = Assert.Throws<InvalidOperationException>(() => injectedVm.InjectProp = 5);
            Assert.Equal(nameof(ViewModel.InjectProp), ex.Message);
        }

        [Fact]
        public void Create_NullAsEmptyCtorArgument_InpcCallInjected()
        {
            var injectedVm = AopInpc.Create<ViewModel>(null);
            injectedVm.PropertyChanged += (sender, args) => throw new InvalidOperationException(args.PropertyName);

            var ex = Assert.Throws<InvalidOperationException>(() => injectedVm.InjectProp = 5);
            Assert.Equal(nameof(ViewModel.InjectProp), ex.Message);
        }

        [Fact]
        public void Create_CtorWithArguments_InpcCallInjected()
        {
            var injectedVm = AopInpc.Create<ViewModel>(7);
            injectedVm.PropertyChanged += (sender, args) => throw new InvalidOperationException(args.PropertyName);

            Assert.Equal(7, injectedVm.Prop);
            var ex = Assert.Throws<InvalidOperationException>(() => injectedVm.InjectProp = 5);
            Assert.Equal(nameof(ViewModel.InjectProp), ex.Message);
        }

        [Fact]
        public void Validate_IncorrectVewModel_Fails()
        {
            Assert.True(AopInpc.Validate(typeof(ViewModel)));
            Assert.False(AopInpc.Validate(typeof(NonVirtualPropViewModel)));
            Assert.False(AopInpc.Validate(typeof(NonPublicPropGetterViewModel)));
            Assert.False(AopInpc.Validate(typeof(NonPublicPropSetterViewModel)));
        }

        public class BaseViewModel : INotifyPropertyChangedCaller
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void RaisePropertyChanged(string propertyName)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class ViewModel : BaseViewModel
        {
            public ViewModel()
            {
            }

            public ViewModel(int prop)
            {
                Prop = prop;
            }

            public virtual int Prop { get; set; }

            [InjectInpc]
            public virtual int InjectProp { get; set; }
        }

        public class NonVirtualPropViewModel : BaseViewModel
        {
            [InjectInpc]
            public int Prop { get; set; }
        }

        public class NonPublicPropGetterViewModel : BaseViewModel
        {
            [InjectInpc]
            public virtual int Prop { internal get; set; }
        }

        public class NonPublicPropSetterViewModel : BaseViewModel
        {
            [InjectInpc]
            public virtual int Prop { get; internal set; }
        }
    }
}
