using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DrWPF.Windows.Data;

namespace PropertyGridEditor
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class PropGrid : UserControl
	{
		//they key is the property name
		//value.Item1 is here to denote the control we wil be displaying to the screen
		//Value.Item2 is the actual data.
		//Dictionary<String, Tuple<Control, object>> properties = new Dictionary<string, Tuple<Control, object>>();
		public ObservableDictionary<String, object> PropDictionary = new ObservableDictionary<string, object>();
		private String CurrentProp = "";
		public PropGrid()
		{
			InitializeComponent();
		}

		public PropGrid(ref ObservableDictionary<String, object> d)
		{
			InitializeComponent();
			PropDictionary = d;
		}

		public void AddProperty(String PropName, Control ctype, object data)
		{
			if (PropDictionary.ContainsKey(PropName)) return; //avoid dict crash
																												//add a row.
			InnerPropGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

			//add the label
			PropDictionary.Add(PropName, data);
			int num = PropDictionary.Count - 1;


			Border bor = new Border() //create label then add it.
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Background = Brushes.Transparent,
				Tag = PropName
			};
			Grid.SetColumnSpan(bor, 2);
			Grid.SetRow(bor, num);
			bor.MouseRightButtonDown += Ctype_MouseRightButtonDown;
			InnerPropGrid.Children.Add(bor); //add label to grid and display



			Label l = new Label() //create label then add it.
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Content = PropName,
				Background = Brushes.Transparent,
				Foreground = Brushes.White,
				Tag = PropName
			};

			
			Grid.SetRow(l, num);
			Grid.SetColumn(l, 0);
			InnerPropGrid.Children.Add(l); //add label to grid and display

			if (ctype is TextBox)
			{
				if (data is String)
				{
					ctype.HorizontalAlignment = HorizontalAlignment.Stretch;
					ctype.Margin = new Thickness(10, 2, 10, 2);
					((TextBox)ctype).Text = (String)data;
					ctype.Height = 30;

					Grid.SetRow(ctype, num);
					Grid.SetColumn(ctype, 1);
					ctype.BringIntoView();
					ctype.Tag = PropName; //used for EZ dictionary access later
					ctype.KeyDown += Ctype_KeyDown;
					
					InnerPropGrid.Children.Add(ctype); //add the desired control type.
				}
			}
			else if (ctype is ComboBox)
			{
				if (data is List<String>)
				{
					ctype.HorizontalAlignment = HorizontalAlignment.Left;
					ctype.VerticalAlignment = VerticalAlignment.Center;
					ctype.Margin = new Thickness(10, 2, 10, 2);
					((ComboBox)ctype).ItemsSource = (List<String>)data;
					ctype.Height = 30;

					Grid.SetRow(ctype, num);
					Grid.SetColumn(ctype, 1);
					ctype.BringIntoView();
					ctype.Tag = PropName; //used for EZ dictionary access later
					((ComboBox)ctype).SelectionChanged += PropGrid_SelectionChanged;
					


					((ComboBox)ctype).SelectedIndex = 0;
					InnerPropGrid.Children.Add(ctype); //add the desired control type.
				}
			}
			else if (ctype is CheckBox)
			{
				if (data is Boolean)
				{
					ctype.HorizontalAlignment = HorizontalAlignment.Left;
					ctype.VerticalAlignment = VerticalAlignment.Center;
					ctype.Margin = new Thickness(10, 2, 10, 2);
					ctype.Height = 30;

					Grid.SetRow(ctype, num);
					Grid.SetColumn(ctype, 1);
					ctype.BringIntoView();
					ctype.Tag = PropName; //used for EZ dictionary access later
					((CheckBox)ctype).Click += Ctype_Click;
					

					if (data is Boolean)
						((CheckBox)ctype).IsChecked = (bool)data;
					InnerPropGrid.Children.Add(ctype); //add the desired control type.
				}
			}
		}

		private void Ctype_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			CurrentProp = ((Border)sender).Tag.ToString();
			ContextMenu cm = this.FindResource("RemoveProp_CM") as ContextMenu;
			cm.PlacementTarget = sender as UIElement;
			cm.IsOpen = true;
		}

		private void OpenClearCM(object sender, MouseButtonEventArgs e)
		{
			ContextMenu cm = this.FindResource("ClearProps_CM") as ContextMenu;
			cm.PlacementTarget = sender as UIElement;
			cm.IsOpen = true;
		}

		private void ClearProps_Click(object sender, RoutedEventArgs e)
		{
			foreach (String s in PropDictionary.Keys.ToList())
				RemoveProperty(s);
		}

		private void RemoveProperty_Click(object sender, RoutedEventArgs e)
		{
			RemoveProperty(CurrentProp);
		}

		public void RemoveProperty(String PropName)
		{
			if (!PropDictionary.ContainsKey(PropName)) return; //don't remove what doesn't exist
			int num = 0;
			while (PropDictionary.Keys.ToList()[num] != PropName)
				num++; //inc until key is found.

			//remove the row definition
			//InnerPropGrid.Children;

			//remove from display
			InnerPropGrid.Children.Remove(GetChildren(InnerPropGrid, num, 0));
			InnerPropGrid.Children.Remove(GetChildren(InnerPropGrid, num, 1));


			//move all the other rows UP
			for (int numr = num + 1; numr < InnerPropGrid.RowDefinitions.Count; numr++)
			{
				Grid.SetRow(GetChildren(InnerPropGrid, numr, 0), numr - 1);
				Grid.SetRow(GetChildren(InnerPropGrid, numr, 1), numr - 1);
			}
			InnerPropGrid.RowDefinitions.RemoveAt(num);
			PropDictionary.Remove(PropName); //remove from dictionary
		}

		private void PropGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Console.WriteLine("Changed Combobox selection");
			PropDictionary[((ComboBox)sender).Tag.ToString()] = ((ComboBox)sender).SelectedIndex;
		}

		private void Ctype_Click(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Clicked Checkbox");
			PropDictionary[((CheckBox)sender).Tag.ToString()] = ((CheckBox)sender).IsChecked;
		}

		private void Ctype_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Console.WriteLine("KeyDown on TextBox");
				PropDictionary[((TextBox)sender).Tag.ToString()] = ((TextBox)sender).Text;
			}
		}

		private static UIElement GetChildren(Grid grid, int row, int column)
		{
			foreach (UIElement child in grid.Children)
			{
				if (child is GridSplitter ||  child is Border) continue;
				if (Grid.GetRow(child) == row
							&&
					 Grid.GetColumn(child) == column)
				{
					return child;
				}
			}
			return null;
		}


	}
}
