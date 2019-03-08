using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;

namespace Calc
{
	public partial class MainWindow
	{
        ToolTip m_toolTip = new ToolTip();
        StackCalc m_stack = new StackCalc();
        int m_indexHistory = 0;

        public StackCalc Stack
        {
            get { return m_stack; }
        }

		public MainWindow()
		{
			this.InitializeComponent();

            m_toolTip.Style = (Style)FindResource( "EditToolTip" );
            m_toolTip.PlacementTarget = m_inputBox;
		}

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            m_inputBox.Focus();
			RefillColumns();
        }

        private void m_inputBox_PreviewKeyDown( object sender, System.Windows.Input.KeyEventArgs e )
        {
            m_toolTip.IsOpen = false;

            if(e.Key == Key.Enter)
            { 
                string strExpression = m_inputBox.Text;

                try
                {
                    if(Regex.Match( strExpression, "[0-9]" ).Success)
                    {
                        // Resolve infix expression
                        double dblResult = Expressions.Resolve( strExpression );

                        // Push result to stack
                        m_stack.Push( dblResult, strExpression );
                    }
                    else
                    {
                        m_stack.Push( Functions.Factory.Create( strExpression ) );
                    }

                    m_indexHistory = m_stack.Count;

                    // If valid
                    m_inputBox.Text = "";
                }
                catch(Exception ex)
                {
                    // eventually show error
                    m_toolTip.Content = ex.Message;
                    m_toolTip.IsOpen = true;
                }
            }
            else if(e.Key == Key.Up || e.Key == Key.Down)
            {
                if( e.Key == Key.Up )
                    --m_indexHistory;
                else
                    ++m_indexHistory;

                m_indexHistory = Math.Max( m_indexHistory, 0 );
                m_indexHistory = Math.Min( m_indexHistory, m_stack.Count );
                if(m_indexHistory == m_stack.Count)
                {
                    m_inputBox.Text = "";
                }
                else
	            {
                    m_inputBox.Text = m_stack[m_indexHistory].Expression;
                    m_inputBox.CaretIndex = m_inputBox.Text.Length;
	            }

            }
        }

		private void Clear_Click( object sender, RoutedEventArgs e )
		{
			Stack.Clear();
		}

		private void ShowColumnMenu_Click( object sender, RoutedEventArgs e )
		{
			RefillColumns();
		}

		private void RefillColumns()
		{
			GridViewColumnCollection columns = ( m_stackListView.View as GridView ).Columns;
			columns.Clear();

			if( m_showDecimalMenu.IsChecked )
			{
				GridViewColumn column = new GridViewColumn();
				column.DisplayMemberBinding = new Binding( "Value" );
				column.Header = "Decimal";
				column.Width = Double.NaN;
				columns.Add( column );
			}
			if( m_showHexadecimalMenu.IsChecked )
			{
				GridViewColumn column = new GridViewColumn();
				column.DisplayMemberBinding = new Binding( "Hex" );
				column.Header = "Hexadecimal";
				column.Width = Double.NaN;
				columns.Add( column );
			}
			if( m_showBinaryMenu.IsChecked )
			{
				GridViewColumn column = new GridViewColumn();
				column.DisplayMemberBinding = new Binding( "Binary" );
				column.Header = "Binary";
				column.Width = Double.NaN;
				columns.Add( column );
			}
			if( m_showExpressionMenu.IsChecked )
			{
				GridViewColumn column = new GridViewColumn();
				column.DisplayMemberBinding = new Binding( "Expression" );
				column.Header = "Expression";
				column.Width = 150;
				columns.Add( column );
			}
		}
	}
}