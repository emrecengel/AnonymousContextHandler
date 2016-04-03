using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AnonymousContextHandler.Extensions
{
    internal static class ObjectExtensions
    {
        public static void SetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberLamda,
            TValue value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression == null)
            {
                var unarySelectorExpression = memberLamda.Body as UnaryExpression;

                if (unarySelectorExpression == null)
                    throw new Exception("Not known expression type!");

                memberSelectorExpression = unarySelectorExpression.Operand as MemberExpression;
            }

            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(target, value, null);
                }
            }
        }

        public static object GetPropValue(this object obj, string name)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null)
                {
                    return null;
                }

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null)
                {
                    return null;
                }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }
    }
}