using Expressions.Task3.E3SQueryProvider.QueryProvider;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Expressions.Task3.E3SQueryProvider
{
    public class ExpressionToFtsRequestTranslator : ExpressionVisitor
    {
        Queue ProcessingQueue = new Queue();

        readonly StringBuilder _resultStringBuilder;

        public ExpressionToFtsRequestTranslator()
        {
            _resultStringBuilder = new StringBuilder();
        }

        public string Translate(Expression exp)
        {
            Visit(exp);

            return _resultStringBuilder.ToString();
        }

        #region protected methods

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable)
                && node.Method.Name == "Where")
            {
                var predicate = node.Arguments[1];
                Visit(predicate);

                return node;
            }

            if (E3SQueries.Methods.TryGetValue(node.Method.Name, out var method))
                ProcessingQueue.Enqueue(method);

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    if (node.Left.NodeType != ExpressionType.MemberAccess &&
                        node.Right.NodeType != ExpressionType.MemberAccess)
                        throw new NotSupportedException("Expression isn't relevant.");

                    if (node.Left.NodeType == ExpressionType.MemberAccess)
                        Visit(node.Left);
                    else
                        Visit(node.Right);

                    _resultStringBuilder.Append("(");

                    if (node.Right.NodeType == ExpressionType.Constant)
                        Visit(node.Right);
                    else
                        Visit(node.Left);

                    _resultStringBuilder.Append(")");
                    break;
                default:
                    throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
            };

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _resultStringBuilder.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var result = node.Value.ToString();
            while (ProcessingQueue.Count > 0)
            {
                var method = ProcessingQueue.Dequeue() as Func<string, string>;
                result = method(result);
            }

            _resultStringBuilder.Append(result);
            return node;
        }

        #endregion
    }
}
