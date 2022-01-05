const width_threshold = 480;

function drawLineChart(date, time) {
    if ($("#lineChart").length) {
        ctxLine = document.getElementById("lineChart").getContext("2d");
        optionsLine = {
            scales: {
                yAxes: [
                    {
                        scaleLabel: {
                            display: true,
                            labelString: "Number of Order by date"
                        }
                    }
                ]
            }
        };
        // Set aspect ratio based on window width
        optionsLine.maintainAspectRatio =
                $(window).width() < width_threshold ? false : true;
        configLine = {
            type: "line",
            data: {
                labels: date,
                datasets: [
                    {
                        label: "Orders",
                        data: time,
                        fill: true,
                        borderColor: "rgba(153, 102, 255, 1)",
                        cubicInterpolationMode: "monotone",
                        pointRadius: 1
                    }
                ]
            },
            options: optionsLine
        };

        lineChart = new Chart(ctxLine, configLine);
    }
}
function drawLineChart2(dateMoney, money) {
    if ($("#lineChart2").length) {
        ctxLine = document.getElementById("lineChart2").getContext("2d");
        optionsLine = {
            scales: {
                yAxes: [
                    {
                        scaleLabel: {
                            display: true,
                            labelString: "Revenue by date"
                        }
                    }
                ]
            }
        };
        // Set aspect ratio based on window width
        optionsLine.maintainAspectRatio =
                $(window).width() < width_threshold ? false : true;
        configLine = {
            type: "line",
            data: {
                labels: dateMoney,
                datasets: [
                    {
                        label: "Money",
                        data: money,
                        fill: true,
                        borderColor: "#18dcff",
                        cubicInterpolationMode: "monotone",
                        pointRadius: 1
                    }
                ]
            },
            options: optionsLine
        };
        lineChart2 = new Chart(ctxLine, configLine);
    }
}
function drawBarChart(categoryName, numProductOfCate, colors) {
    var getColors = colors.slice(0, categoryName.length);
    if ($("#barChart").length) {
        ctxBar = document.getElementById("barChart").getContext("2d");
        optionsBar = {
            responsive: true,
            scales: {
                yAxes: [
                    {
                        barPercentage: 0.2,
                        ticks: {
                            beginAtZero: true
                        },
                        scaleLabel: {
                            display: true,
                            labelString: "Hits"
                        }
                    }
                ]
            }
        };

        optionsBar.maintainAspectRatio =
                $(window).width() < width_threshold ? false : true;

        /**
         * COLOR CODES
         * Red: #F7604D
         * Aqua: #4ED6B8
         * Green: #A8D582
         * Yellow: #D7D768
         * Purple: #9D66CC
         * Orange: #DB9C3F
         * Blue: #3889FC
         */

        configBar = {
            type: "horizontalBar",
            data: {
                labels: categoryName,
                datasets: [
                    {
                        label: "# Product",
                        data: numProductOfCate,
                        backgroundColor: getColors,
                        borderWidth: 0
                    }
                ]
            }
            //,
            //options: optionsBar
        };

        barChart = new Chart(ctxBar, configBar);
    }
}
function drawPieChart(totalFoods, kinds, colors) {
    var getColors = colors.slice(colors.length - kinds.length, colors.length);
    if ($("#pieChart").length) {
        var chartHeight = 300;

        $("#pieChartContainer").css("height", chartHeight + "px");

        ctxPie = document.getElementById("pieChart").getContext("2d");

        optionsPie = {
            responsive: true,
            maintainAspectRatio: false,
            layout: {
                padding: {
                    left: 10,
                    right: 10,
                    top: 10,
                    bottom: 5
                }
            },
            legend: {
                position: "top"
            }
        };


        var a = totalFoods;
        configPie = {
            type: "pie",
            data: {
                datasets: [
                    {
                        data: a,
                        backgroundColor: getColors,
                        label: "Storage"
                    }
                ],
                labels: kinds
            },
            options: optionsPie
        };

        pieChart = new Chart(ctxPie, configPie);
    }
}
function updateLineChart() {
    if (lineChart) {
        lineChart.options = optionsLine;
        lineChart.update();
    }
}

function updateBarChart() {
    if (barChart) {
        barChart.options = optionsBar;
        barChart.update();
    }
}
