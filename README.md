This is an interesting question with a lot of nuances. You'll have to forgive me if my example or approach isn't an "exact" fit for what you're doing. However, you asked it this way:

>Is there any better way to get this done?

If I'm understanding your code and intent, then yes the better way would be making sure that the _document_ (the data) is being properly decoupled from the view that displays it. You shouldn't have to clone anything, but especially not framework elements. Everything you are doing seems to rely on an implied data record that could be represented as:

~~~
partial class UniqueNumberItem : ObservableObject
{
    [ObservableProperty]
    string wNR = string.Empty; 

    [ObservableProperty]
    string uniqueNumber = string.Empty;

    [ObservableProperty]
    string matnr = string.Empty;
}
~~~

___

You may be doing this in some form already, but the idea is to have a portable ViewModel that implements `INotifyPropertyChanged`, and have the observable Items be sourced in a single location. You can clear this list, or add and remove items from it, but cloning or copying it should not be necessary.

~~~
class MainWindowViewModel
{
    // THE SOURCE LIST - With 15 items initially populated for test.
    public IList Items { get; } =
        new ObservableCollection<UniqueNumberItem>(
            Enumerable
            .Range(1, 15)
            .Select(_ => new UniqueNumberItem
            {
                WNR = GenerateName("WNR", 10),
                UniqueNumber = GenerateName("UN", 7),
                Matnr = GenerateName("MAT-", 7),
            }));

    // Utility for test name generation
    private static string GenerateName(string prefix, int length) =>
        $"{prefix}{Guid.NewGuid().ToString().Replace("-", string.Empty
                ).Substring(0,length).ToUpper()}";
}
~~~
___

Now we just need a reusable way to display one of these records, the data template named `PrintTemplate`, which can be put into App.xaml so that it's visible to all. If we want to have a linear `listbox10` we can use the data template to popolate it. If you want 6-up on a page for a print preview, we can use the data template to populate it.

~~~
<Application x:Class="six_up_xps_page.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:six_up_xps_page"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <local:StringToQRCodeConverter x:Key="StringToQRCodeConverter" />
        <DataTemplate
            x:Key="PrintTemplate">
            <Border 
            BorderThickness="2" 
            BorderBrush="Black" 
            CornerRadius="10" 
            Padding="10" 
            Margin="10">
                <Grid Margin="10">
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition Height="Auto"/>
                    </Grid.RowDefinitions>
                    <Image Source="{Binding UniqueNumber, Converter={StaticResource StringToQRCodeConverter}}" 
                        Width="150" Height="150" 
                        HorizontalAlignment="Center" 
                        Grid.Row="0" />
                    <TextBlock Text="{Binding WNR}" 
                            FontSize="16" 
                            HorizontalAlignment="Center" 
                            Grid.Row="1"/>
                    <TextBlock Text="{Binding UniqueNumber}" 
                            FontSize="16" 
                            FontWeight="Bold" 
                            HorizontalAlignment="Center" 
                            Grid.Row="2"/>
                    <TextBlock Text="{Binding Matnr}" 
                            FontSize="16" 
                            FontStyle="Italic" 
                            HorizontalAlignment="Center" 
                            Grid.Row="3"/>
                </Grid>
            </Border>
        </DataTemplate>         
    </Application.Resources>
</Application>
~~~

___

**Why it Matters**

The _same list_ can be a source for a `DataGrid` for editing _and_ the scrolling list beside it, which reflects changes in real time as they are made.

~~~
<Window x:Class="six_up_xps_page.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:six_up_xps_page"
        mc:Ignorable="d"
        Title="MainWindow" Height="600" Width="1000"
        WindowStartupLocation="CenterScreen">
    <Window.DataContext>
        <local:MainWindowViewModel/>
    </Window.DataContext>
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="2*"/>
            <ColumnDefinition Width="Auto"/>
            <ColumnDefinition Width="Auto"/>
        </Grid.ColumnDefinitions>
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="*"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>
            <DataGrid
                Grid.Column="0" AutoGenerateColumns="True" ColumnWidth="*" 
                ItemsSource="{Binding Items}" Margin="10" IsReadOnly="False"
                CanUserAddRows="False"/>
            <Button
                Grid.Row="1" Content="Print Preview" Width="150" Height="50"
                Margin="10" Background="DarkBlue" Foreground="White" FontSize="16"
                FontWeight="Bold" Padding="5" BorderThickness="2" BorderBrush="Black"
                Click="OnPrintPreview"/>
        </Grid>
        <ListBox
            x:Name="listBox10" Grid.Column="1"
            ItemTemplate="{StaticResource PrintTemplate}" ItemsSource="{Binding Items}"
            ScrollViewer.VerticalScrollBarVisibility="Auto" Margin="10"/>
    </Grid>
</Window>
~~~

___

**Print Preview**

The _same list_ can be passed to the print preview boxes, shown here with some basic page navigation.

~~~
<Window x:Class="six_up_xps_page.PrintPreviewWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Print Preview" Height="1100" Width="850" 
        WindowStartupLocation="CenterScreen">
    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <Grid x:Name="PreviewGrid">
            <Grid.RowDefinitions>
                <RowDefinition Height="*"/>
                <RowDefinition Height="*"/>
                <RowDefinition Height="*"/>
            </Grid.RowDefinitions>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <ContentPresenter x:Name="Tile1" Grid.Row="0" Grid.Column="0" ContentTemplate="{StaticResource PrintTemplate}" />
            <ContentPresenter x:Name="Tile2" Grid.Row="0" Grid.Column="1" ContentTemplate="{StaticResource PrintTemplate}" />
            <ContentPresenter x:Name="Tile3" Grid.Row="1" Grid.Column="0" ContentTemplate="{StaticResource PrintTemplate}" />
            <ContentPresenter x:Name="Tile4" Grid.Row="1" Grid.Column="1" ContentTemplate="{StaticResource PrintTemplate}" />
            <ContentPresenter x:Name="Tile5" Grid.Row="2" Grid.Column="0" ContentTemplate="{StaticResource PrintTemplate}" />
            <ContentPresenter x:Name="Tile6" Grid.Row="2" Grid.Column="1" ContentTemplate="{StaticResource PrintTemplate}" />
        </Grid>

        <StackPanel Grid.Row="1" Orientation="Horizontal" HorizontalAlignment="Center" Margin="10">
            <Button Content="◀" Width="150" Height="50" Margin="10,0" 
                    Background="DarkBlue" Foreground="White" FontSize="16" 
                    FontWeight="Bold" Padding="5" Click="OnPageDownClick"/>

            <Button Content="▶" Width="150" Height="50" Margin="10,0" 
                    Background="DarkBlue" Foreground="White" FontSize="16" 
                    FontWeight="Bold" Padding="5" Click="OnPageUpClick"/>

            <Button Content="Print" Width="150" Height="50" Margin="10,0" 
                    Background="DarkGreen" Foreground="White" FontSize="16" 
                    FontWeight="Bold" Padding="5" Click="OnPrintClick"/>
        </StackPanel>
    </Grid>
</Window>
~~~