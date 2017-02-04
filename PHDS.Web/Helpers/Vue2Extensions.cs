using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace System.Web.Mvc
{
    public static class Vue2Extensions
    {
        public static HtmlString vModelFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var name = helper.NameFor(expression).ToString();
            return new HtmlString($"v-model=\"mvcModel.{name}\" id=\"{name}\" name=\"{name}\"");
        }

        public static Vue2Helper<TModel> Vue2<TModel>(this HtmlHelper<TModel> helper)
        {
            return new Vue2Helper<TModel>(helper);
        }

        public class Vue2Helper<TModel>
        {
            HtmlHelper<TModel> Html { get; set; }
            public Vue2Helper(HtmlHelper<TModel> obj)
            {
                Html = obj;
            }

            public HtmlString modelName = new HtmlString("mvcModel");
            

            public HtmlString vModelFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
            {
                var name = Html.NameFor(expression).ToString();
                return new HtmlString($"v-model=\"mvcModel.{name}\" id=\"{name}\" name=\"{name}\"");
            }

            public HtmlString modelDataFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
            {
                return new HtmlString($"this.{modelName}.{Html.NameFor(expression)}");
            }
        }
    }
}