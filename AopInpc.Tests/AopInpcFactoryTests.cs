using System;
using System.ComponentModel;
using Xunit;

namespace AopInpc.Tests
{
    public class AopInpcFactoryTests
    {
        [Fact]
        public void Create_PropWithoutInpcAttribute_InpcCallIsNotInjected()
        {
            var injectedVm = AopInpcFactory.Create<ViewModel>();
            injectedVm.PropertyChanged += (sender, args) => throw new InvalidOperationException(args.PropertyName);

            injectedVm.Prop = 5;
        }

        [Fact]
        public void Create_PropWithInpcAttribute_InpcCallInjected()
        {
            var injectedVm = AopInpcFactory.Create<ViewModel>();
            injectedVm.PropertyChanged += (sender, args) => throw new InvalidOperationException(args.PropertyName);

            var ex = Assert.Throws<InvalidOperationException>(() => injectedVm.InjectProp = 5);
            Assert.Equal(nameof(ViewModel.InjectProp), ex.Message);
        }

        [Fact]
        public void Create_NullAsEmptyCtorArgument_InpcCallInjected()
        {
            var injectedVm = AopInpcFactory.Create<ViewModel>(null);
            injectedVm.PropertyChanged += (sender, args) => throw new InvalidOperationException(args.PropertyName);

            var ex = Assert.Throws<InvalidOperationException>(() => injectedVm.InjectProp = 5);
            Assert.Equal(nameof(ViewModel.InjectProp), ex.Message);
        }

        [Fact]
        public void Create_CtorWithArguments_InpcCallInjected()
        {
            var injectedVm = AopInpcFactory.Create<ViewModel>(7);
            injectedVm.PropertyChanged += (sender, args) => throw new InvalidOperationException(args.PropertyName);

            Assert.Equal(7, injectedVm.Prop);
            var ex = Assert.Throws<InvalidOperationException>(() => injectedVm.InjectProp = 5);
            Assert.Equal(nameof(ViewModel.InjectProp), ex.Message);
        }

        [Fact]
        public void Validate_IncorrectVewModel_Fails()
        {
            Assert.True(AopInpcFactory.Validate(typeof(ViewModel)));
            Assert.False(AopInpcFactory.Validate(typeof(NonVirtualPropViewModel)));
            Assert.False(AopInpcFactory.Validate(typeof(NonPublicPropGetterViewModel)));
            Assert.False(AopInpcFactory.Validate(typeof(NonPublicPropSetterViewModel)));
        }

        [Fact]
        public void Decorate_PropWithoutInpcAttribute_InpcCallIsNotInjected()
        {
            var injectedVm = AopInpcFactory.Decorate(new ViewModel());
            injectedVm.PropertyChanged += (sender, args) => throw new InvalidOperationException(args.PropertyName);

            injectedVm.Prop = 5;
        }

        [Fact]
        public void Decorate_PropWithInpcAttribute_InpcCallInjected()
        {
            var injectedVm = AopInpcFactory.Decorate(new ViewModel());
            injectedVm.PropertyChanged += (sender, args) => throw new InvalidOperationException(args.PropertyName);

            var ex = Assert.Throws<InvalidOperationException>(() => injectedVm.InjectProp = 5);
            Assert.Equal(nameof(ViewModel.InjectProp), ex.Message);
        }

        [Fact]
        public void Decorate_NullAsTarget_Fails()
        {
            ViewModel viewModel = null;
            Assert.Throws<ArgumentNullException>(() => AopInpcFactory.Decorate(viewModel));
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

            [Inpc]
            public virtual int InjectProp { get; set; }
        }

        public class NonVirtualPropViewModel : BaseViewModel
        {
            [Inpc]
            public int Prop { get; set; }
        }

        public class NonPublicPropGetterViewModel : BaseViewModel
        {
            [Inpc]
            public virtual int Prop { internal get; set; }
        }

        public class NonPublicPropSetterViewModel : BaseViewModel
        {
            [Inpc]
            public virtual int Prop { get; internal set; }
        }
    }
}
