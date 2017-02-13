using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {
        private static string GetControllerName(Type controllerType)
        {
            var controllerName = controllerType.Name.EndsWith("Controller")
                ? controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length)
                : controllerType.Name;
            return controllerName;
        }

        private static string GetActionName(MethodCallExpression call)
        {
            return call.Method.Name; ;
        }
        public static string ActionName<TController>(this HtmlHelper helper, Expression<Func<TController,ActionResult>> expression)
        {
            return GetActionName((MethodCallExpression)expression.Body);
        }

        public static string ControllerName<TController>(this HtmlHelper helper)
        {
            return GetControllerName(typeof(TController));
        }
        public static MvcForm BeginForm<TController>(this HtmlHelper helper, Expression<Func<TController,ActionResult>> actionSelector, FormMethod method,object htmlAttributes)
        {
            var action = GetActionName((MethodCallExpression)actionSelector.Body);
            var controller = GetControllerName(typeof(TController));
            return helper.BeginForm(action, controller, method, htmlAttributes);
        }
        
    }

    public static class UrlExtensions
    {
        private static string GetControllerName(Type controllerType)
        {
            var controllerName = controllerType.Name.EndsWith("Controller")
                ? controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length)
                : controllerType.Name;
            return controllerName;
        }

        private static string GetActionName(MethodCallExpression call)
        {
            return call.Method.Name; ;
        }

        private static RouteValueDictionary GetRouteValues(MethodCallExpression call)
        {
            var routeValues = new RouteValueDictionary();

            var args = call.Arguments;
            ParameterInfo[] parameters = call.Method.GetParameters();
            var pairs = args.Select((a, i) => new
            {
                Argument = a,
                ParamName = parameters[i].Name
            });
            foreach (var argumentParameterPair in pairs)
            {
                string name = argumentParameterPair.ParamName;
                object value = ((ConstantExpression)argumentParameterPair.Argument).Value;
                if (value != null)
                {
                    var valueType = value.GetType();
                    if (valueType.IsValueType || valueType == typeof(string))
                    {
                        routeValues.Add(name, value);
                    }
                    else throw new NotSupportedException($"unsoupported parameter type {valueType.ToString()}");
                }
            }
            return routeValues;
        }

        public static string Action<TController>(this UrlHelper url, Expression<Func<TController, ActionResult>> actionSelector, string protocol = null, string hostname = null)
        {
            var action = GetActionName((MethodCallExpression)actionSelector.Body);
            var controller = GetControllerName(typeof(TController));
            var routeValues = GetRouteValues((MethodCallExpression)actionSelector.Body);

            return url.Action(action, controller, routeValues, protocol, hostname);
        }
    }
}