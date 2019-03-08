using System;
using System.Collections.Generic;

namespace Calc
{
    namespace Functions
    {
        public class Factory
        {
			static Dictionary<string,Function> m_functions = new Dictionary<string,Function>();

			static Factory()
			{
				// Standard operators
				RegisterFunction("+", new Add());
				RegisterFunction("-", new Substract());
				RegisterFunction("*", new Multiply());
				RegisterFunction("/", new Divide());
				RegisterFunction("^", new Pow());
                    
                // Unary operator
				RegisterFunction("- unary", new Minus());

                // Standard functions
				RegisterFunction("abs", new GenericFunction1( new GenericFunction1.Func( Math.Abs ) ));
				RegisterFunction("acos", new GenericFunction1( new GenericFunction1.Func( Math.Acos ) ));
				RegisterFunction("asin", new GenericFunction1( new GenericFunction1.Func( Math.Asin ) ));
				RegisterFunction("atan", new GenericFunction1( new GenericFunction1.Func( Math.Atan ) ));
				RegisterFunction("ceiling", new GenericFunction1( new GenericFunction1.Func( Math.Ceiling ) ));
				RegisterFunction("combination", new Combination());
				RegisterFunction("cos", new GenericFunction1( new GenericFunction1.Func( Math.Cos ) ));
				RegisterFunction("cosh", new GenericFunction1( new GenericFunction1.Func( Math.Cosh ) ));
				RegisterFunction("exp", new GenericFunction1( new GenericFunction1.Func( Math.Exp ) ));
				RegisterFunction("factorial", new Factorial());
				RegisterFunction("floor", new GenericFunction1( new GenericFunction1.Func( Math.Floor ) ));
				RegisterFunction("ln", new GenericFunction1( new GenericFunction1.Func( Math.Log ) ));
				RegisterFunction("log", new GenericFunction2( new GenericFunction2.Func( Math.Log ) ));
				RegisterFunction("log10", new GenericFunction1( new GenericFunction1.Func( Math.Log10 ) ));
				RegisterFunction("min", new GenericFunction2( new GenericFunction2.Func( Math.Min ) ));
				RegisterFunction("max", new GenericFunction2( new GenericFunction2.Func( Math.Max ) ));
				RegisterFunction("pow", new GenericFunction2( new GenericFunction2.Func( Math.Pow ) ));
				RegisterFunction("sin", new GenericFunction1( new GenericFunction1.Func( Math.Sin ) ));
				RegisterFunction("sinh", new GenericFunction1( new GenericFunction1.Func( Math.Sinh ) ));
				RegisterFunction("sqrt", new GenericFunction1( new GenericFunction1.Func( Math.Sqrt ) ));
				RegisterFunction("tan", new GenericFunction1( new GenericFunction1.Func( Math.Tan ) ));
				RegisterFunction("tanh", new GenericFunction1( new GenericFunction1.Func( Math.Tanh ) ));
				RegisterFunction("truncate", new GenericFunction1( new GenericFunction1.Func( Math.Truncate ) ));
			}

			static void RegisterFunction( string name, Function func )
			{
				func.Name = name;
				m_functions[name] = func;
			}

            public static Function Create( string in_operator )
            {
                string op = in_operator.ToLower();

				Function func;
				if( ! m_functions.TryGetValue( in_operator, out func ) )
				{
					throw new Exception( "Unknown Operator: " + in_operator );
				}

				return func;
            }
        }

        abstract public class Function
        {
            public abstract double Call( List<double> in_parameters );
            public abstract int ParameterCount { get;}
			public string Name { get; set; }

			public override string ToString()
			{
				if( Name.Length > 0 )
					return Name;
				return base.ToString();
			}
        }


        public class GenericFunction1 : Function
        {
            public delegate double Func( double in_dbl );
            Func m_func;

            public GenericFunction1( Func in_func )
            {
                m_func = in_func;
            }

            public override double Call( List<double> in_parameters )
            {
                if(in_parameters.Count != ParameterCount)
                    throw new Exception( m_func.Method.ToString() + " must have 1 parameter" );

                return m_func( in_parameters[0] );
            }
            public override int ParameterCount
            {
                get { return 1; }
            }
        }

        public class GenericFunction2 : Function
        {
            public delegate double Func( double in_dbl1, double in_dbl2 );
            Func m_func;

            public GenericFunction2( Func in_func )
            {
                m_func = in_func;
            }

            public override double Call( List<double> in_parameters )
            {
                if(in_parameters.Count != ParameterCount)
                    throw new Exception( m_func.Method.ToString() + " must have 2 parameter" );

                return m_func( in_parameters[0], in_parameters[1] );
            }
            public override int ParameterCount
            {
                get { return 2; }
            }
        }

        public class Ln : Function
        {
            public override double Call( List<double> in_parameters )
            {
                if(in_parameters.Count != ParameterCount)
                    throw new Exception( "Ln must have 1 parameter" );

                return Math.Log( in_parameters[0], Math.E );
            }
            public override int ParameterCount
            {
                get { return 1; }
            }
        }
		
		public class Factorial : Function
		{
			public override double Call( List<double> in_parameters )
			{
				if( in_parameters.Count != ParameterCount )
					throw new Exception( "Factorial must have 1 parameter" );

				// Check for int
				if( Math.Truncate( in_parameters[0] ) != in_parameters[0] )
					throw new Exception( "Factorial only accept integer paramater" );
				
				return Do( in_parameters[0] );
			}
			public override int ParameterCount
			{
				get { return 1; }
			}

			public static double Do( double a )
			{
				double result = 1;
				for( int i = 2; i <= a; i++ )
				{
					result *= i;
				}
				return result;
			}
		}

		public class Combination : Function
		{
			public override double Call( List<double> in_parameters )
			{
				if( in_parameters.Count != ParameterCount )
					throw new Exception( "Combination must have 2 parameter" );

				// Check for int
				if( Math.Truncate( in_parameters[0] ) != in_parameters[0] &&
					Math.Truncate( in_parameters[1] ) != in_parameters[1] )
					throw new Exception( "Combination only accept integer paramaters" );

				return Factorial.Do( in_parameters[0] ) /
					( Factorial.Do( in_parameters[1] ) * Factorial.Do( in_parameters[0] - in_parameters[1] ) );
			}
			public override int ParameterCount
			{
				get { return 2; }
			}
		}		

        // Operators

        public enum Associativity
        {
            Associative,        // * +
            LeftAssociative,    // - /
            RightAssociative,   // ^
            NonAssociative      // not used here
        }

        abstract public class Operator : Function
        {
            public abstract int Priority { get;}
            public abstract Associativity Associativity { get;}
        }

        abstract public class UnaryOperator : Operator
        {
            public override double Call( List<double> in_parameters )
            {
                if(in_parameters.Count != ParameterCount)
                    throw new Exception( "UnaryOperator must have 1 parameters" );

                return Call( in_parameters[0] );
            }

            public abstract double Call( double in_dbl );
            public override int ParameterCount
            {
                get { return 1; }
            }

            public override Associativity Associativity
            {
                get { return Associativity.Associative; }
            }
        }

        public class Minus : UnaryOperator
        {
            public override int Priority { get { return 0; } }
            public override double Call( double in_dbl ) { return in_dbl * -1;  }
        }

        abstract public class BinaryOperator : Operator
        {
            public override double Call( List<double> in_parameters )
            {
                if(in_parameters.Count != ParameterCount)
                    throw new Exception( "BinaryOperator must have 2 parameters" );

                return Call( in_parameters[0], in_parameters[1] );
            }

            public abstract double Call( double in_left, double in_right );

            public override int ParameterCount
            {
                get { return 2; }
            }
        }

        public class Add : BinaryOperator
        {
            public override double Call( double in_left, double in_right )
            {
                return in_left + in_right;
            }

            public override int Priority { get { return 3; } }
            public override Associativity Associativity { get { return Associativity.Associative; } }
        }

        public class Substract : BinaryOperator
        {
            public override double Call( double in_left, double in_right )
            {
                return in_left - in_right;
            }

            public override int Priority { get { return 3; } }
            public override Associativity Associativity { get { return Associativity.LeftAssociative; } }
        }

        public class Multiply : BinaryOperator
        {
            public override double Call( double in_left, double in_right )
            {
                return in_left * in_right;
            }

            public override int Priority { get { return 2; } }
            public override Associativity Associativity { get { return Associativity.Associative; } }
        }

        public class Divide : BinaryOperator
        {
            public override double Call( double in_left, double in_right )
            {
                return in_left / in_right;
            }

            public override int Priority { get { return 2; } }
            public override Associativity Associativity { get { return Associativity.LeftAssociative; } }
        }

        public class Pow : BinaryOperator
        {
            public override double Call( double in_left, double in_right )
            {
                return Math.Pow( in_left, in_right );
            }

            public override int Priority { get { return 1; } }
            public override Associativity Associativity { get { return Associativity.RightAssociative; } }
        }
    }
}