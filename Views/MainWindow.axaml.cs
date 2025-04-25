using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using plato.Models;
using System;
using plato.ViewModels;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace plato.Views;

public partial class MainWindow : Window
{
    private TreeView? CollectionsTreeView;
    private TabControl? RequestsTabControl;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        
        CollectionsTreeView = this.FindControl<TreeView>("CollectionsTreeView");
        RequestsTabControl = this.FindControl<TabControl>("RequestsTabControl");

        // Subscribe to tree item double-click
        if (CollectionsTreeView != null)
        {
            CollectionsTreeView.DoubleTapped += TreeItem_DoubleTapped;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void TreeItem_DoubleTapped(object sender, TappedEventArgs e)
    {
        // Get the TreeItemViewModel that was double-tapped
        if (sender is StackPanel panel && panel.DataContext is TreeItemViewModel treeItem)
        {
            // If it's a folder, toggle expanded state
            if (treeItem.IsFolder)
            {
                treeItem.IsExpanded = !treeItem.IsExpanded;
            }
            // If it's a regular item, load the request
            else
            {
                // This will be handled through binding to SelectedTreeItem
            }

            // Mark the event as handled
            e.Handled = true;
        }
    }

    private void AddTab_Click(object? sender, RoutedEventArgs e)
    {
        AddNewTab("New Request");
    }

    private void CloseTab_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button closeButton && 
            closeButton.Parent is DockPanel headerPanel && 
            headerPanel.Parent is TabItem tabItem && 
            RequestsTabControl != null)
        {
            // Make sure we don't remove the "+" tab
            if (RequestsTabControl.Items.IndexOf(tabItem) < RequestsTabControl.Items.Count - 1)
            {
                RequestsTabControl.Items.Remove(tabItem);
            }
        }
    }

    private void AddNewTab(string tabName)
    {
        if (RequestsTabControl == null) return;

        // Create a close button for the tab
        var closeButton = new Button
        {
            Content = "✕",
            Padding = new Thickness(4, 1),
            Margin = new Thickness(5, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            Background = new SolidColorBrush(Colors.Transparent),
            CornerRadius = new CornerRadius(10)
        };
        closeButton.Click += CloseTab_Click;

        // Create the tab header panel
        var headerPanel = new DockPanel();
        DockPanel.SetDock(closeButton, Dock.Right);
        headerPanel.Children.Add(closeButton);
        headerPanel.Children.Add(new TextBlock
        {
            Text = tabName,
            VerticalAlignment = VerticalAlignment.Center
        });

        // Get content from an existing tab (excluding the "+" tab)
        var contentTemplate = RequestsTabControl.Items[0];
        
        // Create the new tab
        var newTab = new TabItem
        {
            Header = headerPanel,
            Content = contentTemplate is TabItem existingTabItem ? existingTabItem.Content : null,
            Padding = new Thickness(10, 5),
            MinWidth = 120
        };

        // Add the new tab before the "+" tab
        int lastIndex = RequestsTabControl.Items.Count - 1;
        RequestsTabControl.Items.Insert(lastIndex, newTab);
        
        // Select the new tab
        RequestsTabControl.SelectedIndex = lastIndex;
    }
}