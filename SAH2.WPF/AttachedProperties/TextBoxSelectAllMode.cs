/***********************************************************************************************************
 ***********************************************************************************************************
 ***********************************************************************************************************
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***                                   Explanations and Example                                          ***
 ***                          http://stackoverflow.com/a/42288384/2374053                                ***
 ***                       https://www.bilalafsar.com/Posts.aspx?PostID=323                              ***
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***********************************************************************************************************
 ***********************************************************************************************************
 **********************************************************************************************************/


using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SAH2.WPF.AttachedProperties
{
    public enum SelectAllMode
    {
        /// <summary>
        ///     On first focus, it selects all then leave off textbox and doesn't check again
        /// </summary>
        OnFirstFocusThenLeaveOff = 0,

        /// <summary>
        ///     On first focus, it selects all then never selects
        /// </summary>
        OnFirstFocusThenNever = 1,

        /// <summary>
        ///     Selects all on every focus
        /// </summary>
        OnEveryFocus = 2,

        /// <summary>
        ///     Never selects text (WPF's default attitude)
        /// </summary>
        Never = 4
    }

    public class TextBox : DependencyObject
    {
        public static readonly DependencyProperty SelectAllModeProperty = DependencyProperty.RegisterAttached(
            "SelectAllMode",
            typeof(SelectAllMode?),
            typeof(TextBox),
            new PropertyMetadata(SelectAllModePropertyChanged));

        private static void SelectAllModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Controls.TextBox)
            {
                var textBox = (System.Windows.Controls.TextBox)d;

                if (e.NewValue != null)
                {
                    textBox.GotKeyboardFocus += OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                }
                else
                {
                    textBox.GotKeyboardFocus -= OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                }
            }
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = GetParentFromVisualTree(e.OriginalSource);

            if (dependencyObject == null)
                return;

            var textBox = (System.Windows.Controls.TextBox)dependencyObject;
            if (!textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }

        private static DependencyObject GetParentFromVisualTree(object source)
        {
            DependencyObject parent = source as UIElement;
            while (parent != null && !(parent is System.Windows.Controls.TextBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }

        private static void OnKeyboardFocusSelectText(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = e.OriginalSource as System.Windows.Controls.TextBox;
            if (textBox == null) return;

            var selectAllMode = GetSelectAllMode(textBox);

            if (selectAllMode == SelectAllMode.Never)
            {
                textBox.SelectionStart = 0;
                textBox.SelectionLength = 0;
            }
            else
                textBox.SelectAll();

            if (selectAllMode == SelectAllMode.OnFirstFocusThenNever)
                SetSelectAllMode(textBox, SelectAllMode.Never);
            else if (selectAllMode == SelectAllMode.OnFirstFocusThenLeaveOff)
                SetSelectAllMode(textBox, null);
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.TextBox))]
        public static SelectAllMode? GetSelectAllMode(DependencyObject @object)
        {
            return (SelectAllMode?)@object.GetValue(SelectAllModeProperty);
        }

        public static void SetSelectAllMode(DependencyObject @object, SelectAllMode? value)
        {
            @object.SetValue(SelectAllModeProperty, value);
        }
    }
}
