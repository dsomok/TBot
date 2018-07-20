using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TBot.Infrastructure.Messaging.Abstractions.Subscriptions;

namespace TBot.Infrastructure.Messaging.Abstractions.Hosting
{
    interface IRegistrationExpressionBuilder<TBus>
    {
        object CreateHandlerInstance();
        ParameterExpression CreateMessageParameter();
        MethodInfo GetHandleMethod(string handleMethodName);
        LambdaExpression BuildHandleInvokationExpression(object handlerInstance, MethodInfo handleMethod, ParameterExpression messageParameter);
        MethodInfo GetMessageBusRegistrationMethod(string registrationMethodName);
        Task<ISubscription> InvokeMessageBusRegistration(MethodInfo registrationMethod, string serviceName, LambdaExpression handleExpression);

        Task<ISubscription> Build(string serviceName, string handleMethodName, string messageBusRegistrationMethodName);
    }
}
