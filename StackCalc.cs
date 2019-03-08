using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Calc
{
	public class HistoryItem
	{
		private string m_strExpression;
		private double m_dblValue;

		public double Value
		{
			get { return m_dblValue; }
			set { m_dblValue = value; }
		}

		public string Hex
		{
			get
			{
				string res = "";
				double dblIntegerPart = Math.Truncate( m_dblValue );
				if( dblIntegerPart < 0 )
				{
					dblIntegerPart *= -1;
					res += "-";
				}
				res += "0x" + ( (long)dblIntegerPart ).ToString( "X" );

				if( Math.Abs( m_dblValue ) - dblIntegerPart > 0.000000000001 )
				{
					res += ".?";
				}
				return res;
			}
		}

		public string Binary
		{
			get
			{
				string res = "";

				double dblIntegerPart = Math.Truncate( m_dblValue );

				long iValue = Math.Abs( (long)dblIntegerPart );

				for( int i = 0; i < 64 && ( ( iValue >> i ) > 0 ); i++ )
				{
					res += ( iValue & ( 1 << i ) ) != 0 ? "1" : "0";
				}

				if( dblIntegerPart < 0 )
				{
					res += "-";

				}

				List<char> chars = new List<char>( res.ToCharArray() );
				chars.Reverse();
				res = new string( chars.ToArray() );

				if( Math.Abs( m_dblValue - dblIntegerPart ) > 0.000000000001 )
				{
					res += ".?";
				}
				return res;
			}
		}

		public string Expression
		{
			get { return m_strExpression; }
			set { m_strExpression = value; }
		}

		public HistoryItem( double in_dblValue, string in_strExpression )
		{
			m_strExpression = in_strExpression;
			m_dblValue = in_dblValue;
		}
	}

	public class StackCalc : ObservableCollection<HistoryItem>
	{
		public void Push( double in_number, string in_strExpression )
		{
			Add( new HistoryItem( in_number, in_strExpression ) );
		}

		public void Push( Functions.Function in_func )
		{
			if( Count >= in_func.ParameterCount )
			{
				List<double> parameters = new List<double>();

				for( int i = in_func.ParameterCount; i > 0; i-- )
				{
					parameters.Add( this[Count - i].Value );
					RemoveAt( Count - i );
				}

				string expression = in_func.Name + "(";

				for( int i = 0; i < parameters.Count; i++ )
				{
					expression += parameters[i].ToString();
					if( i != parameters.Count-1 )
						expression += ",";
				}

				expression += ")";

				Push( in_func.Call( parameters ), expression );
			}
			else
			{
				throw new Exception( "Not enough operand on the stack.  Add at least two numbers in the stack." );
			}
		}
	}
}
