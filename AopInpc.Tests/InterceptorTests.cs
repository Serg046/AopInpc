﻿using System;
using Castle.DynamicProxy;
using Moq;
using Xunit;

namespace AopInpc.Tests
{
    public class InterceptorTests
    {
        private readonly Interceptor _interceptor = new Interceptor();

        private Mock<IInvocation> GetInvocationMock(Type type)
        {
            var invocationMock = new Mock<IInvocation>();
            invocationMock.SetupGet(m => m.Method).Returns(type.GetProperty(nameof(BaseViewModel.Prop)).SetMethod);
            invocationMock.SetupGet(m => m.TargetType).Returns(type);
            return invocationMock;
        }

        [Fact]
        public void Intercept_PropSetter_InpcCalledWithCorrectName()
        {
            var inpcMock = new Mock<INotifyPropertyChangedCaller>();
            var invocationMock = GetInvocationMock(typeof(BaseViewModel));
            invocationMock.SetupGet(m => m.Proxy).Returns(inpcMock.Object);

            _interceptor.Intercept(invocationMock.Object);

            invocationMock.Verify(invc => invc.Proceed(), Times.Once());
            inpcMock.Verify(inpc => inpc.RaisePropertyChanged(nameof(BaseViewModel.Prop)));
        }

        [Fact]
        public void Intercept_InheritedPropSetter_InpcCalledWithCorrectName()
        {
            var inpcMock = new Mock<INotifyPropertyChangedCaller>();
            var invocationMock = GetInvocationMock(typeof(ViewModel));
            invocationMock.SetupGet(m => m.Proxy).Returns(inpcMock.Object);

            _interceptor.Intercept(invocationMock.Object);

            invocationMock.Verify(invc => invc.Proceed(), Times.Once());
            inpcMock.Verify(inpc => inpc.RaisePropertyChanged(nameof(BaseViewModel.Prop)));
        }

        private class ViewModel : BaseViewModel
        {
        }

        private class BaseViewModel
        {
            [InjectInpc]
            public int Prop { get; set; }
        }
    }
}
