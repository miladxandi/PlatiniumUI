using System.Collections.ObjectModel;
using System.Collections.Specialized;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;
using PlatiniumUI.Fluent.DesignSystem.Icons;

namespace PlatiniumUI.Fluent.Controls.Layout;

/// <summary>
/// Accordion expand mode options.
/// </summary>
public enum FlAccordionExpandMode
{
    /// <summary>Only one item can be expanded at a time</summary>
    Single,
    /// <summary>Multiple items can be expanded simultaneously</summary>
    Multiple
}

/// <summary>
/// Represents a single item in the FlAccordion control.
/// </summary>
public class FlAccordionItem : BindableObject
{
    public static readonly BindableProperty HeaderProperty =
        BindableProperty.Create(nameof(Header), typeof(string), typeof(FlAccordionItem), string.Empty);

    public static readonly BindableProperty ContentProperty =
        BindableProperty.Create(nameof(Content), typeof(View), typeof(FlAccordionItem), null);

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(FlAccordionItem), false,
            BindingMode.TwoWay);

    public static readonly BindableProperty IsEnabledProperty =
        BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(FlAccordionItem), true);

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public View? Content
    {
        get => (View?)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public bool IsEnabled
    {
        get => (bool)GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }
}

/// <summary>
/// Fluent Design accordion control with multiple expandable/collapsible sections.
/// Supports single or multiple expansion modes with smooth animations.
/// </summary>
public class FlAccordion : ContentView
{
    // ─── Bindable Properties ────────────────────────────────────────────────

    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(nameof(Items), typeof(ObservableCollection<FlAccordionItem>), typeof(FlAccordion), null,
            propertyChanged: OnItemsChanged);

    public static readonly BindableProperty ExpandModeProperty =
        BindableProperty.Create(nameof(ExpandMode), typeof(FlAccordionExpandMode), typeof(FlAccordion), FlAccordionExpandMode.Single,
            propertyChanged: OnExpandModeChanged);

    public static readonly BindableProperty ExpandAnimationDurationProperty =
        BindableProperty.Create(nameof(ExpandAnimationDuration), typeof(uint), typeof(FlAccordion), FluentAnimations.ExpandDuration);

    // ─── Properties ─────────────────────────────────────────────────────────

    public ObservableCollection<FlAccordionItem>? Items
    {
        get => (ObservableCollection<FlAccordionItem>?)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public FlAccordionExpandMode ExpandMode
    {
        get => (FlAccordionExpandMode)GetValue(ExpandModeProperty);
        set => SetValue(ExpandModeProperty, value);
    }

    public uint ExpandAnimationDuration
    {
        get => (uint)GetValue(ExpandAnimationDurationProperty);
        set => SetValue(ExpandAnimationDurationProperty, value);
    }

    // ─── Private Fields ─────────────────────────────────────────────────────

    private readonly VerticalStackLayout _container;
    private readonly Dictionary<FlAccordionItem, View> _itemViews = new();

    // ─── Constructor ────────────────────────────────────────────────────────

    public FlAccordion()
    {
        _container = new VerticalStackLayout
        {
            Spacing = FluentSpacing.S
        };

        Content = _container;

        this.ObserveThemeChanged(ApplyTheme);
    }

    // ─── Property Changed Handlers ──────────────────────────────────────────

    private static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var accordion = (FlAccordion)bindable;
        
        if (oldValue is ObservableCollection<FlAccordionItem> oldCollection)
        {
            oldCollection.CollectionChanged -= accordion.OnCollectionChanged;
        }

        if (newValue is ObservableCollection<FlAccordionItem> newCollection)
        {
            newCollection.CollectionChanged += accordion.OnCollectionChanged;
        }

        accordion.RebuildItems();
    }

    private static void OnExpandModeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var accordion = (FlAccordion)bindable;
        
        // If switching to Single mode, collapse all but the first expanded item
        if (accordion.ExpandMode == FlAccordionExpandMode.Single && accordion.Items != null)
        {
            var firstExpanded = accordion.Items.FirstOrDefault(i => i.IsExpanded);
            foreach (var item in accordion.Items)
            {
                if (item != firstExpanded)
                {
                    item.IsExpanded = false;
                }
            }
        }
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildItems();
    }

    // ─── Item Building ──────────────────────────────────────────────────────

    private void RebuildItems()
    {
        _container.Children.Clear();
        _itemViews.Clear();

        if (Items == null || Items.Count == 0)
            return;

        foreach (var item in Items)
        {
            var itemView = CreateItemView(item);
            _container.Children.Add(itemView);
            _itemViews[item] = itemView;

            // Subscribe to IsExpanded changes
            item.PropertyChanged += OnItemPropertyChanged;
        }
    }

    private View CreateItemView(FlAccordionItem item)
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        var headerBorder = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Small)
            },
            Stroke = new BrushSetter().SetBrush(theme.BorderSubtle),
            BackgroundColor = theme.Surface,
            Padding = new Thickness(FluentSpacing.M),
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                Children =
                {
                    new Label
                    {
                        Text = item.Header,
                        FontSize = FluentTypography.BodyStrongSize,
                        TextColor = theme.TextPrimary,
                        VerticalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = FluentIcons.chevron_down_16_regular,
                        FontFamily = FluentIcons.FontFamily,
                        FontSize = 12,
                        TextColor = theme.TextSecondary,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.End
                    }.WithColumn(1)
                }
            }
        };

        // Add tap gesture to header
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => ToggleItem(item);
        headerBorder.GestureRecognizers.Add(tapGesture);

        var contentContainer = new ContentView
        {
            Content = item.Content,
            IsVisible = item.IsExpanded,
            Padding = new Thickness(FluentSpacing.M, 0, FluentSpacing.M, FluentSpacing.M)
        };

        var container = new VerticalStackLayout
        {
            Spacing = 0,
            Children = { headerBorder, contentContainer }
        };

        return container;
    }

    private void OnItemPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FlAccordionItem.IsExpanded) && sender is FlAccordionItem item)
        {
            UpdateItemExpansion(item);
        }
    }

    private void ToggleItem(FlAccordionItem item)
    {
        if (!item.IsEnabled)
            return;

        if (ExpandMode == FlAccordionExpandMode.Single)
        {
            // Collapse all other items
            if (Items != null)
            {
                foreach (var otherItem in Items)
                {
                    if (otherItem != item && otherItem.IsExpanded)
                    {
                        otherItem.IsExpanded = false;
                    }
                }
            }
        }

        item.IsExpanded = !item.IsExpanded;
    }

    private void UpdateItemExpansion(FlAccordionItem item)
    {
        if (!_itemViews.TryGetValue(item, out var itemView))
            return;

        if (itemView is VerticalStackLayout stack && stack.Children.Count >= 2)
        {
            var contentContainer = stack.Children[1] as ContentView;
            if (contentContainer != null)
            {
                if (item.IsExpanded)
                {
                    contentContainer.IsVisible = true;
                    // Animate expansion
                    contentContainer.TranslateToAsync(0, 0, ExpandAnimationDuration, FluentAnimations.Standard);
                }
                else
                {
                    // Animate collapse
                    contentContainer.TranslateToAsync(0, -contentContainer.Height, ExpandAnimationDuration, FluentAnimations.Standard)
                        .ContinueWith(_ =>
                        {
                            Dispatcher.Dispatch(() => contentContainer.IsVisible = false);
                        });
                }
            }
        }
    }

    // ─── Theme ──────────────────────────────────────────────────────────────

    private void ApplyTheme()
    {
        // Rebuild to apply new theme
        RebuildItems();
    }
}

// ─── Extension Methods ──────────────────────────────────────────────────────

internal static class AccordionExtensions
{
    public static T WithColumn<T>(this T view, int column) where T : View
    {
        Grid.SetColumn(view, column);
        return view;
    }
}

internal class BrushSetter
{
    private Color? _color;

    public BrushSetter SetBrush(Color color)
    {
        _color = color;
        return this;
    }

    public static implicit operator Brush(BrushSetter setter)
    {
        return new SolidColorBrush(setter._color ?? Colors.Transparent);
    }
}
