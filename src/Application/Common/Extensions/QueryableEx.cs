using Application.Common.Extensions;
using Application.Common.Models;
using Domain.Global;
using System.Linq.Expressions;

namespace Application.Common.Extenstions
{
    public static class QueryableEx
    {
        #region Private function

        public static IQueryable<T> OrderHelper<T>(this IQueryable<T> source, string propertyName,
            bool orderAscendingDirection, bool subLevel)
        {
            if (string.IsNullOrEmpty(propertyName)) return source;

            var type = typeof(T);
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type);

            if (property != null)
            {
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var resultExp = Expression.Call(
                    typeof(Queryable),
                    (subLevel ? "ThenBy" : "OrderBy") + (orderAscendingDirection ? string.Empty : "Descending"),
                    new[] { type, property.PropertyType },
                    source.Expression,
                    Expression.Quote(orderByExp)
                );
                return source.Provider.CreateQuery<T>(resultExp);
            }

            return source;
        }

        #endregion Private function

        #region Extented

        /// <summary>
        /// Dynamic Object Filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="filterTypes"></param>
        /// <param name="filterParams"></param>
        /// <param name="positiveNumbericValue">When value like ID, ignore if it less then 0 </param>
        /// <returns></returns>
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, Type[] filterTypes, object requestDto, bool positiveNumbericValue = false)
        {
            var filterParams = requestDto.GetType().ToDictionary(requestDto);
            if (filterParams == null) return source;

            #region Variable Declared

            var type = typeof(T);
            var parameter = Expression.Parameter(type);

            var textProp = new Dictionary<string, string>();
            var numbericProp = new Dictionary<string, (Type, object)>();
            var booleanProp = new Dictionary<string, (Type, bool)>();
            var numbericRangeProp = new Dictionary<string, (object, object)>();

            // Expression tree built variable
            var methodCalls = new List<MethodCallExpression>();
            var numbericBinaryExpression = new List<BinaryExpression>();

            var numbericDataType = new List<Type>
        {
            typeof(int), typeof(int?),
            typeof(long), typeof(long?),
            typeof(float), typeof(float?),
            typeof(short), typeof(short?),
            typeof(sbyte), typeof(sbyte?),
            typeof(decimal), typeof(decimal?)
            // Update more numberic type in here ...
        };

            #endregion Variable Declared

            // Group filter param corresponsing data types
            foreach (var param in filterParams)
            {
                var key = param.Key;
                var value = param.Value;

                // Check property information
                var property = type.GetProperty(key);
                if (property == null) continue;
                var propertyType = property.PropertyType;
                if (!filterTypes.Contains(propertyType)) continue;

                // Division data type
                if (numbericDataType.Contains(propertyType)) numbericProp.Add(property.Name, (propertyType, value));
                if (propertyType == typeof(string)) textProp.Add(key, value.ToString().ToStringSafe());
                if (propertyType == typeof(bool) || propertyType == typeof(bool?)) booleanProp.Add(key, (propertyType, Convert.ToBoolean(value)));
            }

            // Filter for each data type
            numbericBinaryExpression.AddRange(
                CreateExpressionCallForFilterByNumbericRangeValue(type, parameter, numbericRangeProp));
            numbericBinaryExpression.AddRange(CreateExpressionCallForFilterByNumbericValue(type, parameter, numbericProp, positiveNumbericValue).Concat(
                CreateExpressionCallForFilterByBooleanValue(type, parameter, booleanProp)));

            #region Build Expression Tree

            // Aggregate binary expression call
            if (numbericBinaryExpression.Count > 0)
            {
                var binaryExpression = numbericBinaryExpression.Aggregate((pre, next) => Expression.And(pre, next));
                if (binaryExpression != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);
                    source = source.Where(lambda);
                }
            }

            // Aggregate method expression call
            methodCalls.AddRange(CreateExpressionCallForFilterByText(type, parameter, textProp));
            if (methodCalls.Count >= 2)
            {
                var expression = Expression.And(methodCalls[0], methodCalls[1]);
                for (int index = 2; index < methodCalls.Count; index++)
                {
                    expression = Expression.And(expression, methodCalls[index]);
                }
                var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
                source = source.Where(lambda);
            }
            else if (methodCalls.Count == 1)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(methodCalls[0], parameter);
                source = source.Where(lambda);
            }

            #endregion Build Expression Tree

            return source;
        }

        /// <summary>
        /// Build a sequences expression tree for filtering text
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public static List<MethodCallExpression> CreateExpressionCallForFilterByText(Type type, ParameterExpression parameter, Dictionary<string, string> filterParams)
        {
            var methodCalls = new List<MethodCallExpression>();
            if (filterParams.Count == 0) return methodCalls;

            var containMethodInfor = typeof(string).GetMethod("Contains", new Type[] { typeof(System.String) });
            foreach (var param in filterParams)
            {
                var property = type.GetProperty(param.Key);
                if (property != null && containMethodInfor != null && !string.IsNullOrEmpty(param.Value))
                {
                    methodCalls.Add(Expression.Call(
                            Expression.Property(parameter, property),
                            containMethodInfor,
                            Expression.Constant(param.Value)));
                }
            }

            return methodCalls;
        }

        /// <summary>
        /// Build a sequences expression tree for filter numberic  range property
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameter"></param>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public static List<BinaryExpression> CreateExpressionCallForFilterByNumbericRangeValue(Type type, ParameterExpression parameter, Dictionary<string, (object, object)> filterParams, bool positiveNumbericValue = false)
        {
            var binaryExpression = new List<BinaryExpression>();
            if (filterParams.Count == 0) return binaryExpression;

            foreach (var param in filterParams)
            {
                var properyMember = Expression.Property(parameter, param.Key);
                if (properyMember != null)
                {
                    // Ignore case positive numberic value
                    if (positiveNumbericValue && (Convert.ToInt64(param.Value.Item1) < 0 || Convert.ToInt64(param.Value.Item2) < 0)) continue;
                    binaryExpression.Add(Expression.And(
                        Expression.GreaterThanOrEqual(properyMember, Expression.Constant(param.Value.Item1, typeof(int))),
                        Expression.LessThanOrEqual(properyMember, Expression.Constant(param.Value.Item2, typeof(int)))));
                }
            }

            return binaryExpression;
        }

        /// <summary>
        /// Build a sequences expression tree for filter one numberic value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameter"></param>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public static List<BinaryExpression> CreateExpressionCallForFilterByNumbericValue(Type type, ParameterExpression parameter, Dictionary<string, (Type, object)> filterParams, bool positiveNumbericValue = false)
        {
            var binaryExpression = new List<BinaryExpression>();
            if (filterParams.Count == 0) return binaryExpression;

            foreach (var param in filterParams)
            {
                var properyMember = Expression.Property(parameter, param.Key);
                if (properyMember != null)
                {
                    // Ignore case positive numberic value
                    if (positiveNumbericValue && Convert.ToInt64(param.Value.Item2) < 0) continue;
                    binaryExpression.Add(Expression.Equal(properyMember, Expression.Constant(param.Value.Item2, param.Value.Item1)));
                }
            }

            return binaryExpression;
        }

        /// <summary>
        /// Build a sequences expression tree for filter one boolean value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameter"></param>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public static List<BinaryExpression> CreateExpressionCallForFilterByBooleanValue(Type type, ParameterExpression parameter, Dictionary<string, (Type, bool)> filterParams)
        {
            var binaryExpression = new List<BinaryExpression>();
            if (filterParams.Count == 0) return binaryExpression;

            foreach (var param in filterParams)
            {
                var properyMember = Expression.Property(parameter, param.Key);
                if (properyMember != null)
                {
                    binaryExpression.Add(Expression.Equal(properyMember, Expression.Constant(param.Value.Item2, param.Value.Item1)));
                }
            }

            return binaryExpression;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortEntities"></param>
        /// <returns></returns>
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, SortRequestModel[]? sortEntities)
        {
            if (sortEntities == null) return source;
            var properties = typeof(T).GetProperties().Select(p => p.Name);

            var cName = "";
            var cDirAsc = true;
            var isNext = false;

            foreach (var entity in sortEntities)
            {
                if (string.IsNullOrEmpty(entity.Name) || string.IsNullOrEmpty(entity.Dir) || !properties.Contains(entity.Name)) continue;
                if (Enum.TryParse(entity.Dir, out EnumData.SortDir sortDir))
                {
                    cName = entity.Name;
                    cDirAsc = sortDir == EnumData.SortDir.ASC ? true : false;

                    source = !isNext ? cDirAsc ? source.OrderBy(cName) : source.OrderByDescending(cName)
                        : cDirAsc ? source.ThenBy(cName) : source.ThenByDescending(cName);

                    isNext = true;
                }
            }

            return source;
        }

        #endregion Extented

        #region Order

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderHelper(source, propertyName, true, false);
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderHelper(source, propertyName, false, false);
        }

        public static IQueryable<T> ThenBy<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderHelper(source, propertyName, true, true);
        }

        public static IQueryable<T> ThenByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderHelper(source, propertyName, true, false);
        }

        #endregion Order
    }
}