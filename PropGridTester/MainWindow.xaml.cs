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
using PropertyGridEditor;

namespace PropGridTester
{
	public enum EObjectType
	{
		None,
		Folder,
		File,
		Sprite,
		GameEvent 
	};
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		PropGrid pg;
		List<String> ss = new List<String>();
		public ObservableDictionary<String, object> PropDictionary = new ObservableDictionary<string, object>();
		public MainWindow()
		{
			
			InitializeComponent();
			pg = ttt;


			ss.Add("A");
			ss.Add("B");
			ss.Add("C");

			EObjectType e = (EObjectType)Enum.Parse(typeof(EObjectType), "1");
			
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//ttt.AddRow();
			foreach (String s in ss)
			{
				ttt.AddProperty(s , new TextBox(), "hhhhhhhhnnnnnnggggg");
				
			}
			ttt.AddProperty("isActive?", new CheckBox(), true, CheckBox_Click );
			List<String> name = Enum.GetNames(typeof(EObjectType)).ToList();
			ttt.AddProperty("Choice Time!", new ComboBox(), name);
			this.PropDictionary = ttt.PropDictionary; //update...
		}

		public void CheckBox_Click(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Custom Event Fires!");
		}

		private void Button1_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Button2_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
