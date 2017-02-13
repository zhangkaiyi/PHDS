using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace Helpers
{
    public static class Common
    {
        public static string GetActionName<TController, T1, TResult>(Expression<Func<TController, Func<T1, TResult>>> expression)
        {
            var unaryExpression = (UnaryExpression)expression.Body;
            var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
            var constantExpression = (ConstantExpression)methodCallExpression.Object;
            var methodInfo = (MethodInfo)constantExpression.Value;
            return methodInfo.Name;
        }
        public static string GetActionName<TController>(Expression<Func<TController, System.Web.Mvc.ActionResult>> expression)
        {
            return ((MethodCallExpression)expression.Body).Method.Name;
        }
        public static string GetPropertyName<TController, TResult>(Expression<Func<TController, TResult>> express)
        {
            var memberExpress = express.Body as MemberExpression;
            if (memberExpress != null)
            {
                return memberExpress.Member.Name;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}