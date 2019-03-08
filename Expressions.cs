using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Calc
{
    class Expressions
    {
        public static double Resolve( string in_strExpression )
        {
            Queue<object> postFix = Expressions.InfixToPostfix( in_strExpression );

            return Expressions.ResolvePostfix( postFix );
        }

        private static Queue<object> InfixToPostfix( string in_expression )
        {
            Queue<object> output = new Queue<object>();
            Stack<object> stack = new Stack<object>();

            // implement the Shunting yard algorithm
            // http://en.wikipedia.org/wiki/Shunting_yard_algorithm

            string[] tokens = Regex.Split( in_expression, @"([^\w.])" );

            Functions.Function previonFunc = null;
            string previousToken = "";

            foreach(string token in tokens)
            {
                if(token.Trim().Length == 0)
                    continue;

                Functions.Function func = null;

                double dblValue;
                if(Double.TryParse( token, out dblValue ))
                {
                    output.Enqueue( dblValue );
                }
                else if(token == "(")
                {
                    stack.Push( token );
                }
                else if(token == ")")
                {
                    while(!(stack.Peek() is string &&
                        (stack.Peek() as string) == "("))
                    {
                        output.Enqueue( stack.Pop() );
                    }

                    stack.Pop(); // ")"

                    // Check here if Peek() is a function (if the parentensis belong to a function call)
                    if(stack.Count > 0 && stack.Peek() is Functions.Function)
                    {
                        output.Enqueue( stack.Pop() );
                    }
                }
                else if(token == ",")
                {
                    while(!( stack.Peek() is string &&
                        ( stack.Peek() as string ) == "(" ))
                    {
                        output.Enqueue( stack.Pop() );
                    }
                }
                else
                {
                    func = Functions.Factory.Create( token );

                    if( func is Functions.Operator &&
                        ( output.Count == 0 || previonFunc != null || previousToken  == "("))
                    {
                        // Create a unary operator
                        func = Functions.Factory.Create( token + " unary" );
                    }

                    if(func is Functions.Operator)
                    {
                        Functions.Operator op = func as Functions.Operator;

                        while(stack.Count > 0 && stack.Peek() is Functions.Operator &&
                             ( ( ( op.Associativity == Functions.Associativity.Associative || op.Associativity == Functions.Associativity.LeftAssociative ) &&
                             ( stack.Peek() as Functions.Operator ).Priority <= op.Priority )
                             ||
                             ( ( op.Associativity == Functions.Associativity.RightAssociative ) &&
                             ( stack.Peek() as Functions.Operator ).Priority < op.Priority ) ))
                        {
                            output.Enqueue( stack.Pop() );
                        }
                    }

                    stack.Push( func );
                }

                previonFunc = func;
                previousToken = token;
            }

            while(stack.Count > 0)
            {
                output.Enqueue( stack.Pop() );
            }

            return output;
        }

        private static double ResolvePostfix( Queue<object> postFix )
        {
            Stack<double> state = new Stack<double>();
            while(postFix.Count > 0)
            {
                while( postFix.Count > 0 && postFix.Peek() is double )
                    state.Push( (double)postFix.Dequeue() );

                if(postFix.Count > 0)
                {
                    Functions.Function func = postFix.Dequeue() as Functions.Function;

                    List<double> parameters = new List<double>();
                    for(int i = 0; i < func.ParameterCount; i++)
                    {
                        parameters.Add( state.Pop() );
                    }

                    parameters.Reverse();
                    
                    state.Push( func.Call( parameters ) );
                }
            }

            if(state.Count != 1)
                throw new Exception( "Failed to evaluate expression.  No result available" );

            return state.Pop();
        }

        private static void TestExpression( string in_strExpression, double in_dblExpectedResult )
        {
            try
            {
                double res = Resolve( in_strExpression );
                if(Math.Abs( res - in_dblExpectedResult) > 0.0000000001 )
                    throw new Exception( "returned unexpected result" );
            }
            catch(Exception ex)
            {
                System.Console.WriteLine( in_strExpression + " : " + ex.Message );
            }
        }

        public static void UnitTest()
        {
            TestExpression( "5", 5 );
            TestExpression( "5.6", 5.6 );

            TestExpression( "5+6", 11 );
            TestExpression( "5-6", -1 );
            TestExpression( "5*6", 30 );
            TestExpression( "6/5", 1.2 );
            TestExpression( "5+6*4", 29 );
            TestExpression( "5*6+4", 34 );
            TestExpression( "5*6-4", 26 );
            TestExpression( "5-6-4", -5 );
            TestExpression( "5-6/4", 3.5 );
            TestExpression( "5*6/4", 7.5 );
            TestExpression( "5+6/4", 6.5 );
            TestExpression( "6/3/1", 2 );

            // Boudary
            TestExpression( "5/0", Double.PositiveInfinity );

            // Complex Expression
            TestExpression( "5233*56+46894*5655", 265478618 );
            TestExpression( "30*50/40*50", 1875 );

            // Parentesis 1
            TestExpression( "6/(5/12)", 14.4 );
            TestExpression( "6+(5/25)", 6.2 );
            TestExpression( "6/(5+7)", 0.5 );
            TestExpression( "6/(5-7)", -3 );
            TestExpression( "9/(6*5)", 0.3 );
            TestExpression( "9+(6*5)", 39 );
            TestExpression( "9-(6*5)", -21 );
            TestExpression( "9-(3-2)", 8 );
            TestExpression( "9*(6*5)", 270 );
            TestExpression( "9*(6/5)", 10.8 );
            TestExpression( "9*(6-4)", 18 );

            // Parentesis 2
            TestExpression( "9*(6+5)-2", 97 );
            TestExpression( "9*(6+5)*(4+5)-2", 889 );
            TestExpression( "(2+29)*(6+5)-2", 339 );
            TestExpression( "4*(((6+5)-2)+34)", 172 );
            TestExpression( "4*(((6+5)-2)+34)+4/(49+77)*(6+5)-2", 170.3492063492063492063492063492063492063492063492063492063492063 );

            // Power
            TestExpression( "4^3", 64 );
            TestExpression( "4^(3+3)", 4096 );
            TestExpression( "3+4^(3+3)", 4099 );
            TestExpression( "2*4^(3+3)", 8192 );

            // Functions
            TestExpression( "sin(4)", -0.756802495307928251372639094511829094135912887336472571485416 );
            TestExpression( "2*4^sin(3+3)", 1.35770401794689945972605364288774882372622956181222971912883197 );
            TestExpression( "sin(cos(4+3))", 0.6844887989926140581921331582830879029675087194665736188145078879 );

            // Binary functions
            TestExpression( "max(2,5)", 5 );
            TestExpression( "max( min(2,5), max(4.5,3+6))*2", 18 );
            TestExpression( "2/abs( -3 )", 0.6666666666666666666666666666666666666666666666666666666666666667 );

            // Unary operator
            TestExpression( "-5-5", -10 );
            TestExpression( "-5-5-5", -15 );
            TestExpression( "-5-(-5-5)", 5 );
            TestExpression( "(-5-5)-5", -15 );
            TestExpression( "-5-(-5)-5", -5 );
            TestExpression( "-5*-5+-5", 20 );   // not even even supported by mscalc:)
            TestExpression( "-5-(-5*-5)", -30 );
            TestExpression( "-sin(2)", -0.9092974268256816953960198659117448427022549714478902683789730115 );
            TestExpression( "-sin(-2)", 0.9092974268256816953960198659117448427022549714478902683789730115 );

            // A couple tough
            TestExpression( "-344.665*sin(-5*max(5,tan(44)))/45*33+2", -31.452478361462556738107029501154988327825943235077794148054 );


        }
    }
}