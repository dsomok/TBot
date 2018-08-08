using System;
using System.Linq;
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
            return this._handler
                       .HandlerType
                       .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                       .Single(
                           m => m.Name == handleMethodName &&
                                m.GetParameters().Length == 1
                       );
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
            var genericArgumentsCount = this._handler.ResponseType == null ? 1 : 2;
            var openRegisterHandlerMethod = typeof(TBus)
                                         .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                         .Single(m =>
                                             m.Name == registrationMethodName &&
                                             m.GetGenericArguments().Length == genericArgumentsCount
                                         );

            var closedRegisterHandlerMethod = this._handler.ResponseType == null
                ? openRegisterHandlerMethod.MakeGenericMethod(this._handler.MessageType)
                : openRegisterHandlerMethod.MakeGenericMethod(this._handler.MessageType, this._handler.ResponseType);

            return closedRegisterHandlerMethod;
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
