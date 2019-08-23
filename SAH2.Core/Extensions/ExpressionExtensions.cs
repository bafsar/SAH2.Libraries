/***********************************************************************************************************
 ***********************************************************************************************************
 ***********************************************************************************************************
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***                                   Based on the example on                                           ***
 ***                      http://www.albahari.com/nutshell/predicatebuilder.aspx                         ***
 ***                                     address and improved.                                           ***
 ***                                                                                                     ***
 ***                      These codes are different that codes linked above                              ***
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***********************************************************************************************************
 ***********************************************************************************************************
 **********************************************************************************************************/

using System;
using System.Linq;
using System.Linq.Expressions;

namespace SAH2.Core.Extensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Sets as <see cref="mainExpression"/> the first non-null expression in <see cref="expressions"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startIndex">In the next method, the start index of which navigation in the expressions sequence will begin.</param>
        /// <param name="mainExpression"></param>
        /// <param name="expressions"></param>
        private static void AssignMainExpression<T>(out int startIndex, ref Expression<Func<T, bool>> mainExpression,
            params Expression<Func<T, bool>>[] expressions)
        {
            startIndex = 0;

            if (mainExpression == null)
            {
                for (var i = 0; i < expressions.Length; i++)
                {
                    var expression = expressions[i];
                    if (expression == null)
                        continue;

                    mainExpression = expression;
                    startIndex = i + 1;
                    break;
                }
            }
        }


        /// <summary>
        /// Merges expressions using that given <see cref="BinaryExpression"/> method and returns result expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binaryExpressionFunc"></param>
        /// <param name="mainExpression"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> MergeExpressions<T>(Func<Expression, Expression, BinaryExpression> binaryExpressionFunc, Expression<Func<T, bool>> mainExpression, params Expression<Func<T, bool>>[] expressions)
        {
            AssignMainExpression(out var startIndex, ref mainExpression, expressions);

            if (mainExpression == null)
                return null;

            var returnExpression = mainExpression;
            for (var i = startIndex; i < expressions.Length; i++)
            {
                var expression = expressions[i];
                if (expression == null)
                    continue;

                var invokedExpr = Expression.Invoke(expression, mainExpression.Parameters.Cast<Expression>());
                returnExpression = Expression.Lambda<Func<T, bool>>(binaryExpressionFunc(returnExpression.Body, invokedExpr), mainExpression.Parameters);
            }

            return returnExpression;
        }



        /// <summary>
        /// Merges expressions using <see langword="AND"/> operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainExpression"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> mainExpression, params Expression<Func<T, bool>>[] expressions)
        {
            return MergeExpressions(Expression.AndAlso, mainExpression, expressions);
        }

        /// <summary>
        /// Merges expressions using <see langword="OR"/> operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainExpression"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> mainExpression, params Expression<Func<T, bool>>[] expressions)
        {
            return MergeExpressions(Expression.OrElse, mainExpression, expressions);
        }
    }
}