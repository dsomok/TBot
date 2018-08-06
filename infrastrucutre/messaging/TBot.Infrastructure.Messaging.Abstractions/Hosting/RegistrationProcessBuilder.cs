using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using TBot.Infrastructure.Messaging.Abstractions.Subscriptions;

namespace TBot.Infrastructure.Messaging.Abstractions.Hosting
{
    class RegistrationProcessBuilder<TBus> : IRegistrationExpressionBuilder<TBus>
    {
        private readonly (Type HandlerType, Type MessageType, Type ResponseType) _handler;
        private readonly IContainer _container;


        public RegistrationProcessBuilder((Type HandlerType, Type MessageType, Type ResponseType) handler, IContainer container)
        {
            _handler = handler;
            _container = container;
        }


        public object CreateHandlerInstance()
        {
            return this._container.Resolve(this._handler.HandlerType);
        }

        public ParameterExpression CreateMessageParameter()
        {
            return Expression.Parameter(this._handler.MessageType);
        }

        public MethodInfo GetHandleMethod(string handleMethodName)
        {
            return this._handler.HandlerType.GetMethod(handleMethodName, BindingFlags.Public | BindingFlags.Instance);
        }

        public LambdaExpression BuildHandleInvokationExpression(
            object handlerInstance, 
            MethodInfo handleMethod,
            ParameterExpression messageParameter
        )
        {
            var body = Expression.Call(Expression.Constant(handlerInstance), handleMethod, messageParameter);
            return Expression.Lambda(body, messageParameter);
        }

        public MethodInfo GetMessageBusRegistrationMethod(string registrationMethodName)
        {
            return typeof(TBus)
                   .GetMethod(registrationMethodName, BindingFlags.Public | BindingFlags.Instance)
                   .MakeGenericMethod(this._handler.MessageType);
        }

        public Task<ISubscription> InvokeMessageBusRegistration(MethodInfo registrationMethod, string serviceName, LambdaExpression handleExpression)
        {
            var bus = this._container.Resolve<TBus>();
            return registrationMethod.Invoke(bus, new object[] { serviceName, handleExpression.Compile() }) as Task<ISubscription>;
        }

        public Task<ISubscription> Build(string serviceName, string handleMethodName, string messageBusRegistrationMethodName)
        {
            var handlerInstance = this.CreateHandlerInstance();
            var commandExpressionParameter = this.CreateMessageParameter();
            var handleMethod = this.GetHandleMethod(handleMethodName);
            var handleExpression = this.BuildHandleInvokationExpression(handlerInstance, handleMethod, commandExpressionParameter);

            var registerHandlerMethod = this.GetMessageBusRegistrationMethod(messageBusRegistrationMethodName);
            var registerTask = this.InvokeMessageBusRegistration(registerHandlerMethod, serviceName, handleExpression);

            return registerTask;
        }
    }
}
