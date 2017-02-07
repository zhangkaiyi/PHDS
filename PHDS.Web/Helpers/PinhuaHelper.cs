using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
//using System.Linq.Dynamic;

namespace Helpers
{
    public class Pinhua
    {
        static public string CreateRcId()
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var today = DateTime.Now.ToString("yyyyMMdd");
                var id = database.GetNewId_s(26, 1).FirstOrDefault().newId.Value.ToString("D5");
                return "rc" + today + id;
            }
        }

        static public string GetRtId(string templateName)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var rtId = from p in database.ES_Tmp
                           where p.RtName == templateName
                           select p.RtId;
                return rtId.First() ?? string.Empty;
            }
        }

        static public void NewRecord<M>(string tempateName, M record) where M : class
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var rcId = CreateRcId();
                var rtId = GetRtId(tempateName);
                var repCase = new PHDS.Entities.Edmx.ES_RepCase
                {
                    rcId = rcId,
                    RtId = rtId,
                    lstFiller = 2,
                    lstFillerName = HttpContext.Current.User.Identity.Name,
                    lstFillDate = DateTime.UtcNow,
                    fillDate = DateTime.UtcNow,
                };
                if (typeof(M) == typeof(PHDS.Entities.Edmx.收款单))
                {
                    (record as PHDS.Entities.Edmx.收款单).ExcelServerRCID = rcId;
                    (record as PHDS.Entities.Edmx.收款单).ExcelServerRTID = rtId;
                    database.ES_RepCase.Add(repCase);
                    database.收款单.Add(record as PHDS.Entities.Edmx.收款单);
                    database.SaveChanges();
                }
                else if (typeof(M) == typeof(PHDS.Entities.Edmx.付款单))
                {
                    (record as PHDS.Entities.Edmx.付款单).ExcelServerRCID = rcId;
                    (record as PHDS.Entities.Edmx.付款单).ExcelServerRTID = rtId;
                    database.ES_RepCase.Add(repCase);
                    database.付款单.Add(record as PHDS.Entities.Edmx.付款单);
                    database.SaveChanges();
                }
                else if (typeof(M) == typeof(PHDS.Entities.Edmx.物料登记))
                {
                    (record as PHDS.Entities.Edmx.物料登记).ExcelServerRCID = rcId;
                    (record as PHDS.Entities.Edmx.物料登记).ExcelServerRTID = rtId;
                    database.ES_RepCase.Add(repCase);
                    database.物料登记.Add(record as PHDS.Entities.Edmx.物料登记);
                    database.SaveChanges();
                }
            }
        }

        static public void NewRecord<M, D>(M main, D details)
        {
            foreach (var single in details as IEnumerable<int>)
            {

            }
        }

        static public void DeleteRecord(string tempateName, string RcId)
        {

        }
        #region GenerateTrackingNoBy System.Linq.Dynamic
        //static public string GenerateTrackingNoByDynamicLinq<T>(Expression<Func<T, object>> expression, string prefix, int idLength) where T : class
        //{
        //    // 查找 PinhuaEntities 是否包含要求查找的属性 T 的集合 DbSet<T>
        //    System.Reflection.PropertyInfo[] lstPropertyInfo = typeof(Entities.Edmx.PinhuaEntities).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    var oFind = lstPropertyInfo.FirstOrDefault(x => x.PropertyType == typeof(DbSet<T>));
        //    // 获取失败则返回
        //    if (oFind == null)
        //        return string.Empty;

        //    // 从表达式树中获取用于生成编号的字段名称，例如：Entities.Edmx.物料登记.编号，就是"编号"
        //    MemberExpression body = (MemberExpression)expression.Body;
        //    var expressionName = (body.Member is PropertyInfo) ? body.Member.Name : null;

        //    // 创建数据库 EF 查询实例
        //    var database = Activator.CreateInstance<PHDS.Entities.Edmx.PinhuaEntities>();

        //    // 从实例中获取 DbSet<T> 的值，并按条件查询
        //    var value = (IQueryable<T>)oFind.GetValue(database, null);
        //    var results = value.Where($"{expressionName}.Substring(0,{prefix.Length}) == \"{prefix}\" && {expressionName}.Substring({prefix.Length}).Length == {idLength}")
        //        .OrderBy($"{expressionName} descending");

        //    int index = 0;
        //    if (results.Count() == 0)
        //    {
        //        index = 1;
        //    }
        //    else if (results.Count() > 0)
        //    {
        //        var result = results.FirstOrDefault();
        //        System.Reflection.PropertyInfo[] lstPropertyInfo2 = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //        var oFind2 = lstPropertyInfo2.FirstOrDefault(x => x.Name == expressionName);
        //        var value2 = (string)oFind2.GetValue(result, null);
        //        index = int.Parse(value2.Substring(prefix.Length)) + 1;
        //    }
        //    return prefix + index.ToString($"D{idLength}");
        //}
        #endregion

        static public string GenerateTrackingNo<T>(Expression<Func<T, object>> expression, string prefix, int idLength) where T : class
        {
            // 查找 PinhuaEntities 是否包含要求查找的属性 T 的集合 DbSet<T>
            System.Reflection.PropertyInfo[] lstPropertyInfo = typeof(PHDS.Entities.Edmx.PinhuaEntities).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var oFind = lstPropertyInfo.FirstOrDefault(x => x.PropertyType == typeof(DbSet<T>));
            // 获取失败则返回
            if (oFind == null)
                return string.Empty;

            // 从表达式树中获取用于生成编号的字段名称，例如：Entities.Edmx.物料登记.编号，就是"编号"
            MemberExpression body = (MemberExpression)expression.Body;
            var expressionName = (body.Member is PropertyInfo) ? body.Member.Name : null;

            // 创建数据库 EF 查询实例
            var database = Activator.CreateInstance<PHDS.Entities.Edmx.PinhuaEntities>();

            // 从实例中获取 DbSet<T> 的值，并按条件查询
            var value = (IQueryable<T>)oFind.GetValue(database, null);

            // 构造表达式树完成动态查询
            ParameterExpression pe = Expression.Parameter(typeof(T), "x");
            Expression e1 = null;
            {
                Expression left = Expression.Property(pe, typeof(T).GetProperty(expressionName));
                Expression leftProp = Expression.Property(left, typeof(string).GetProperty("Length"));
                Expression right = Expression.Constant(prefix.Length + idLength);
                e1 = Expression.Equal(leftProp, right);
            }
            Expression e2 = null;
            {
                Expression left = Expression.Property(pe, typeof(T).GetProperty(expressionName));
                Expression leftCall = Expression.Call(left, typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) }),
                    new Expression[] { Expression.Constant(0), Expression.Constant(prefix.Length) });
                Expression right = Expression.Constant(prefix);
                e2 = Expression.Equal(leftCall, right);
            }
            // Where
            var e1Ande2 = Expression.AndAlso(e1, e2);
            var whereExpression = Expression.Lambda<Func<T, bool>>(e1Ande2, new ParameterExpression[] { pe });
            // Orderby
            var orderbyProp = Expression.Property(pe, typeof(T).GetProperty(expressionName));
            var orderbyExpression = Expression.Lambda<Func<T, string>>(orderbyProp, new ParameterExpression[] { pe });
            // Select
            var selectProp = Expression.Property(pe, typeof(T).GetProperty(expressionName));
            var selectExpression = Expression.Lambda<Func<T, string>>(selectProp, new ParameterExpression[] { pe });
            // 调用
            var results = value.Where(whereExpression).OrderByDescending(orderbyExpression).Select(selectExpression);

            int index = 0;
            if (results.Count() == 0)
            {
                index = 1;
            }
            else if (results.Count() > 0)
            {
                var result = results.FirstOrDefault();
                index = int.Parse(result.Substring(prefix.Length)) + 1;
            }
            return prefix + index.ToString($"D{idLength}");

            #region MethodCallExpression
            //MethodCallExpression whereCallExpression = Expression.Call(
            //    typeof(Queryable),
            //    "Where",
            //    new Type[] { value.ElementType },
            //    value.Expression,
            //    Expression.Lambda<Func<Entities.Edmx.物料登记, bool>>(e1, new ParameterExpression[] { pe }));

            //IQueryable<Entities.Edmx.物料登记> results = value.Provider.CreateQuery<Entities.Edmx.物料登记>(whereCallExpression);
            #endregion
        }

        public static class Copy
        {
            public static void ShadowCopy(object from, object to)
            {
                if (from.GetType() == to.GetType())
                {
                    System.Reflection.PropertyInfo[] properties = from.GetType().GetProperties();

                    foreach (var p in properties)
                    {
                        var value = p.GetValue(from, null);
                        if (value != GetDefault(p.GetType()))
                            p.SetValue(to, value, null);
                    }
                }
            }

            public static object GetDefault(Type type)
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }
                return null;
            }
        }
    }
}