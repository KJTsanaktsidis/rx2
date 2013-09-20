using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Rx2.Automaton;

namespace Rx2.CodeGen
{
    public class DFADelegateGenerator
    {
        public delegate bool DFAMatchDelegate(string input);

        /*public static DFAMatchDelegate Emit(DFAGraph dfa)
        {
            var returnTarget = Expression.Label(typeof(int));
            var inputParam = Expression.Parameter(typeof (string), "input");

            var lenVar = Expression.Variable(typeof (int));

            var methodBody = Expression.Block(
                new []{lenVar},

                Expression.Assign(lenVar, Expression.Property(inputParam, "Length")),
                Expression.Return(returnTarget, lenVar, typeof(int)),
                Expression.Label(returnTarget, Expression.Constant(0, typeof(int)))
                );

            var lambda = Expression.Lambda<DFAMatchDelegate>(methodBody, inputParam);
            return lambda.Compile();
        }*/

        /*public static DFAMatchDelegate Emit(DFAGraph dfa)
        {
            var inputPointer = Expression.Variable(typeof (int));
            var inputParam = Expression.Parameter(typeof (string), "input");
            var stateNumber = Expression.Variable(typeof (int));

            var firstStateNumber = dfa.StartState.Label;

            var returnTarget = Expression.Label(typeof (bool));

            var currentC = Expression.Property(inputParam, "Item", inputPointer);

            var switchCases = new List<Expression>();
            foreach (var state in dfa.AdjList.Keys)
            {
                var thiscase = Expression.Block(
                    new []{inputPointer},


                    );
            }

            var stateTable = Expression.Switch(
                stateNumber,
                Expression.Return(returnTarget, Expression.Constant(false))


                );
        }*/
    }
}
