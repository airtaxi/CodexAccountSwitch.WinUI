using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CliAccountSwitcher.WinUI.ThirdParty.SelectorBarSegmented;

/// <summary>
/// Segmented variant of <see cref="SelectorBar"/> that renders its items as connected pills
/// instead of the default underline indicator.
/// </summary>
[TemplatePart(Name = PART_ItemsView, Type = typeof(ItemsView))]
public class SelectorBarSegmented : SelectorBar
{
    private const string PART_ItemsView = "PART_ItemsView";

    private ItemsView _itemsView;

    /// <summary>
    /// Identifies the <see cref="SelectedIndex"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(SelectorBarSegmented), new PropertyMetadata(-1, OnSelectedIndexChanged));

    /// <summary>
    /// Identifies the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SelectorBarSegmented), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    /// <summary>
    /// Gets or sets the index of the currently selected item, or -1 when nothing is selected.
    /// </summary>
    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets the direction the items are laid out in.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public SelectorBarSegmented()
    {
        DefaultStyleKey = typeof(SelectorBarSegmented);

        var itemStyle = Application.Current.Resources["SelectorBarItemHorizontalStyle"] as Style;
        Resources[typeof(SelectorBarItem)] = itemStyle;

        Loaded += OnSelectorBarSegmentedLoaded;
        SelectionChanged += OnSelectorBarSegmentedSelectionChanged;
    }

    private void OnSelectorBarSegmentedLoaded(object _, RoutedEventArgs __) => ConfigureInnerScrollView();

    private static void OnSelectedIndexChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) => ((SelectorBarSegmented)dependencyObject).UpdateSelectedIndex((int)args.NewValue);

    private static void OnOrientationChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        var control = (SelectorBarSegmented)dependencyObject;

        var styleKey = (Orientation)args.NewValue == Orientation.Vertical ? "SelectorBarItemVerticalStyle" : "SelectorBarItemHorizontalStyle";
        var itemStyle = Application.Current.Resources[styleKey] as Style;

        control.Resources[typeof(SelectorBarItem)] = itemStyle;

        control.UpdateItemsView((Orientation)args.NewValue);
    }

    private void OnSelectorBarSegmentedSelectionChanged(SelectorBar _, SelectorBarSelectionChangedEventArgs __)
    {
        if (SelectedItem is null)
        {
            SelectedIndex = -1;
            return;
        }

        var index = Items.IndexOf(SelectedItem);

        if (index >= 0 && index != SelectedIndex) SelectedIndex = index;
    }

    private void UpdateSelectedIndex(int value)
    {
        if (Items is null || Items.Count == 0) return;

        if (value < 0) SelectedItem = Items.FirstOrDefault(item => item.IsSelected);
        else
        {
            if (value >= Items.Count) return;

            SelectedItem = Items[value];
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _itemsView = GetTemplateChild(PART_ItemsView) as ItemsView;

        ConfigureInnerScrollView();
        UpdateItemsView(Orientation);
        UpdateSelectedIndex(SelectedIndex);
    }

    private void ConfigureInnerScrollView()
    {
        // The ItemsView's inner ScrollView shows its scrollbar indicators during every layout pass,
        // oscillating the control's height. Configure it once the template is applied (and again on
        // Loaded, in case the ScrollView is not created yet at ApplyTemplate time).
        if (_itemsView?.ScrollView is not { } scrollView) return;

        scrollView.ContentOrientation = ScrollingContentOrientation.Horizontal;
        scrollView.HorizontalScrollBarVisibility = ScrollingScrollBarVisibility.Hidden;
        scrollView.VerticalScrollBarVisibility = ScrollingScrollBarVisibility.Hidden;
    }

    private void UpdateItemsView(Orientation orientation)
    {
        if (_itemsView is not null)
        {
            _itemsView.Layout = new StackLayout
            {
                Orientation = orientation
            };
        }
    }
}