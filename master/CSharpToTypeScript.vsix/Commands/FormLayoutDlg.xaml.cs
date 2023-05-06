using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.VisualStudio.PlatformUI;

namespace CSharpToTypeScript.Commands
{
    /// <summary>
    /// Interaction logic for FormLayoutDlg.xaml
    /// </summary>
    public partial class FormLayoutDlg : DialogWindow
    {
        public static readonly DependencyProperty ColCountProperty =
            DependencyProperty.Register(nameof(ColCount), typeof(int), typeof(FormLayoutDlg),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int ColCount
        {
            get { return (int)GetValue(ColCountProperty); }
            set { SetValue(ColCountProperty, value); }
        }


        public FormLayoutDlg()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (Validation.GetErrors(ColCountCtrl).Count > 0)
            {
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
