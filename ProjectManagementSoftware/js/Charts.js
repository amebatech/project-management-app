function GetPieCharts() {
    alert("data");
        $.ajax({
            url: 'Charts/GetPieCharts',
            type: "GET",
            dataType: "JSON",
            data: { pId: document.getElementById('ProjectId').value },
            success: function (data) {
                DrawPieChart(data);
            },
            error: function (xhr) {  
                alert('error');  
            }  
        });  
    }

    function DrawPieChart(sdata) {
        ////alert(data);
        var seriesData = [];
        //var xCategories = [];
        var i, cat;

        var chart;
        $(document).ready(function () {
            // Build the chart
            chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'container',
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    text: 'Project-wise Task Status'
                },
                legend: {
                    align: 'right',
                    x: -100,
                    verticalAlign: 'top',
                    y: 20,
                    floating: true,
                    backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColorSolid) || 'white',
                    borderColor: '#CCC',
                    borderWidth: 1,
                    shadow: false
                },
                tooltip: {
                    pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: false
                        },
                        showInLegend: true
                    },
                    colorByPoint: true
                },
                series: [{
                    name: '%Task',
                    colorByPoint: true,
                    data: sdata
                }]
                
            });
        });
    }

    function GetStackCharts() {
        alert("data");
        $.ajax({
            url: 'Charts/SubTaskDetails',
            dataType: "json",
            data: { pId: document.getElementById('ProjectId').value },
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            async: true,
            processData: true,
            cache: false,
            delay: 15,
            success: function (data) {
                //alert(data);  
                DrawStackChart(data);
            },
            error: function (xhr) {
                alert('error');
            }
        });
    }

    function DrawStackChart(data) {
        //alert(data);
        var seriesData = [];
        var xCategories = [];
        var i, cat;
        for (i = 0; i < data.length; i++) {
            cat = 'Task Name: ' + data[i].Task;
            if (xCategories.indexOf(cat) === -1) {
                xCategories[xCategories.length] = cat;
            }
        }
        for (i = 0; i < data.length; i++) {
            if (seriesData) {
                var currSeries = seriesData.filter(function (seriesObject) { return seriesObject.name == data[i].Parent; });
                if (currSeries.length === 0) {
                    seriesData[seriesData.length] = currSeries = { name: data[i].Parent, data: [] };
                } else {
                    currSeries = currSeries[0];
                }
                var index = currSeries.data.length;
                currSeries.data[index] = data[i];
            } else {
                seriesData[0] = { name: data[i].Parent, data: [data[i].Days] }
            }
        }

        var chart;
        $(document).ready(function () {
            chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'container',
                    type: 'column'
                },
                title: {
                    text: 'Stacked column chart'
                },
                xAxis: {
                    categories: xCategories
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Project-wise Task Breakup'
                    },
                    stackLabels: {
                        enabled: true,
                        style: {
                            fontWeight: 'bold',
                            color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                        }
                    }
                },
                legend: {
                    align: 'right',
                    x: -100,
                    verticalAlign: 'top',
                    y: 20,
                    floating: true,
                    backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColorSolid) || 'white',
                    borderColor: '#CCC',
                    borderWidth: 1,
                    shadow: false
                },
                tooltip: {
                    formatter: function () {
                        return '<b>' + this.x + '</b><br/>' +
                            this.series.name + ': ' + this.y + '<br/>' +
                            'Total: ' + this.point.stackTotal;
                    }
                },
                plotOptions: {
                    column: {
                        stacking: 'normal'
                    }
                },
                series: seriesData
            });
        });
    }