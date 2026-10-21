using System.Collections.ObjectModel;
using PlatiniumUI.Fluent.Controls.Charts;

namespace PlatiniumUI.Sample.Pages;

public partial class ChartsPage : ContentPage
{
    public ChartsPage()
    {
        InitializeComponent();

        // Bar chart data
        var barSeries = new ObservableCollection<FlChartSeries>
        {
            new FlChartSeries
            {
                Name = "Sales",
                Values = new List<double> { 30, 50, 40, 70, 60, 90, 80 }
            }
        };
        barChart.Series = barSeries;

        // Line chart data
        var lineSeries = new ObservableCollection<FlChartSeries>
        {
            new FlChartSeries
            {
                Name = "Trend",
                Values = new List<double> { 20, 35, 30, 50, 45, 60, 55, 70, 65, 80 }
            }
        };
        lineChart.Series = lineSeries;

        // Pie chart data
        var pieSeries = new ObservableCollection<FlChartSeries>
        {
            new FlChartSeries
            {
                Name = "Distribution",
                Values = new List<double> { 35, 25, 20, 15, 5 }
            }
        };
        pieChart.Series = pieSeries;

        // Spark chart data
        sparkChart.Values = new List<double> { 10, 15, 12, 18, 20, 16, 22, 25, 23, 28, 30 };
    }
}
