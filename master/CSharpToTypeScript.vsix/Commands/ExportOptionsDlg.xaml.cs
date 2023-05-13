using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
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
    /// Interaction logic for ExportOptionsDlg.xaml
    /// </summary>
    public partial class ExportOptionsDlg : DialogWindow
    {
        public bool SetColCount { get; private set; }

        public IReadOnlyList<ComboBox> TypeParamCtrls { get; private set; }

        private static readonly List<DependencyProperty> TypeParamProperties = new();

        public IReadOnlyList<string> TypeParams { get; private set; } = new List<string>();

        private static readonly IReadOnlyList<string> _ParamOptions = new List<string>()
        {
            "System.String",
            "System.Int32",
            "System.Decimal",
            "System.Int16",
            "System.DateTimeOffset",
            "System.Boolean",
            "System.Int64",
            "System.DateTime",
            "System.Guid",
            "System.TimeSpan",
            "System.Double",
            "System.Single",
            "System.Char",
            "System.Byte",
            "System.SByte",
            "System.UInt32",
            "System.UInt64",
            "System.UInt16"
        };

        public static readonly DependencyProperty ColCountProperty =
            DependencyProperty.Register(nameof(ColCount), typeof(int), typeof(ExportOptionsDlg),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int ColCount
        {
            get { return (int)GetValue(ColCountProperty); }
            set { SetValue(ColCountProperty, value); }
        }


        public ExportOptionsDlg(bool showColumnCount, IReadOnlyList<string> paramNames)
        {
            InitializeComponent();

            if (!showColumnCount)
            {
                LayoutGrid.RowDefinitions[0].Height = new GridLength(0);
            }
            else
            {
                LayoutGrid.RowDefinitions[0].Height = GridLength.Auto;
                SetColCount = showColumnCount;
            }

            AddTypeParamSelections(paramNames);
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in TypeParamCtrls)
            {
                item.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
            }

            if (Validation.GetErrors(ColCountCtrl).Count > 0
                || TypeParamCtrls.Any(tc => Validation.GetErrors(tc).Count > 0))
            {
                return;
            }

            TypeParams = TypeParamProperties.Select(tp => ((string)GetValue(tp)).Trim()).ToList();

            DialogResult = true;

            Close();
        }

        private void AddTypeParamSelections(IReadOnlyList<string> paramNames)
        {
            var typeParamCtrls = new List<ComboBox>(paramNames.Count);
            TypeParamCtrls = typeParamCtrls;

            if (paramNames.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < paramNames.Count; ++i)
            {
                LayoutGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }
            Grid.SetRow(Buttons, paramNames.Count + 1);

            var typeParamValidationRule = new TypeParamValidationRule();

            for (var i = 0; i < paramNames.Count; ++i)
            {
                DependencyProperty typeParamProperty = GetTypeParamProperty("TypeParam" + i);
                var paramName = paramNames[i];
                var rowIndex = i + 1;
                var comboBox = new ComboBox()
                {
                    Name = typeParamProperty.Name + "Ctrl",
                    Margin = new Thickness(0, 0, 0, 10),
                    Padding = new Thickness(3),
                    IsEditable = true,
                    Style = (Style)this.FindResource("ComboBoxStyle"),
                };

                foreach (var paramOption in _ParamOptions)
                {
                    comboBox.Items.Add(paramOption);
                }
                comboBox.SelectedIndex = 0;

                Binding binding = new(typeParamProperty.Name)
                {
                    Source = this,
                    UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                    ValidatesOnDataErrors = true,
                    Mode = BindingMode.TwoWay,
                };
                binding.ValidationRules.Add(typeParamValidationRule);
                comboBox.SetBinding(ComboBox.TextProperty, binding);

                typeParamCtrls.Add(comboBox);

                Grid.SetColumn(comboBox, 1);
                Grid.SetRow(comboBox, rowIndex);

                LayoutGrid.Children.Add(comboBox);

                var label = new Label()
                {
                    Content = "Type Param " + rowIndex + " (" + paramName + "):",
                    HorizontalContentAlignment = HorizontalAlignment.Right
                };
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, rowIndex);

                LayoutGrid.Children.Add(label);
            }
        }

        private static DependencyProperty GetTypeParamProperty(string typeParamName)
        {
            var typeParamProperty = TypeParamProperties.FirstOrDefault(p => p.Name == typeParamName);
            if (typeParamProperty == null)
            {
                typeParamProperty = DependencyProperty.Register(typeParamName, typeof(string), typeof(ExportOptionsDlg),
                    new FrameworkPropertyMetadata(_ParamOptions[0], FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
                TypeParamProperties.Add(typeParamProperty);
            }

            return typeParamProperty;
        }
    }
}
