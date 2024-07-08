$(document).ready(function () {
    $('#pieChart').hide();
    $('#barChart').hide();
    loadLogList();

   
    $('#logLevel').on('change', function () {
        loadLogList();
    });

    $('#startDate').on('change', function () {
        loadLogList();
    });
    function validateDates() {
        const startDate = new Date($('#startDate').val());
        const endDate = new Date($('#endDate').val());

        if (!isNaN(startDate.getTime()) && !isNaN(endDate.getTime())) {
            if (endDate < startDate) {
                $('#dateError').text('End date must be greater than or equal to start date.');
            } else {
                $('#dateError').text('');
            }
        }
    }
    $('#endDate').on('change', function () {
        validateDates();
        loadLogList();

    });

    $("#search").on("submit", function (event) {
        event.preventDefault();
        searchLogs();
    });

    $('#customSearchBox').on('keyup', function () {
        searchLogs();
    });

    $('#searchButton').on('click', function () {
        searchLogs();
    });
});
//Load the log table
function loadLogList() {
    var logLevel = $('#logLevel').val();
    var startDate = $('#startDate').val();
    var endDate = $('#endDate').val();

    $.ajax({
        url: '/Log/GetLogs',
        type: 'GET',
        data: $('#AllFilterForm').serialize(),
        success: function (data) {
            if (data && data.length) {
                $('#logsView').html(data);
                initializeDataTable();
            } else {
                $('#tableBody').html(' <td colspan="12">No records found.</td>');
            }
        },
        error: function (error) {
            console.log('Error loading log data: ', error);
        }
    });
}
//DataTable
function initializeDataTable() {
    if (!$.fn.DataTable.isDataTable('#logList')) {
        $('#logList').DataTable({
            "pageLength": 10,
            "lengthMenu": [5, 10, 15, 20, 25],
            "searching": true,
            "ordering": true,
            "paging": true
        });
    }
}


//Back button
function LoadList() {
    $("#logsView").show();
    $("#logLevelPieChart").empty();
    $('#logDailyCountChart').empty();
    $(".back-to-list").hide(); 
    $("#button").show();
    $("#buttonBar").show();
    $('.footer').show();
   // $('#downloadPieChart').hide();
    //$('#downloadBarChart').hide();
    $('#barChart').hide();
    $('#pieChart').hide();
    $('#downloadPdfBtn').show();
    //$('#Regeneratebutton').hide();
    //$('#RegeneratePieChart').hide();
    $('html, body').animate({ scrollTop: 0 }, 'fast');
}

// Line Chart
function drawLineChart() {
    $("#logsView").hide();
    $("#button").hide();
    $("#buttonBar").hide();
    $(".back-to-list").show();
    $('#downloadBarChart').show();
    $('#pieChart').hide();
    $("#barChart").show();
    $('#Regeneratebutton').show();
    $('#downloadPdfBtn').hide();
    google.charts.load('current', { packages: ['corechart'] });
    google.charts.setOnLoadCallback(fetchAndDrawLineChart);

    function fetchAndDrawLineChart() {
        const formData = $('#AllFilterForm').serialize();

        fetch(`/Log/GetDailyLogCounts?${formData}`)
            .then(response => response.json())
            .then(data => {
                //console.log('Fetched chart data:', data);
                const chartData = [['Date', 'Info', 'Warn', 'Error', 'Critical']];

                const groupedData = data.reduce((acc, item) => {
                    const date = new Date(item.logDate).toISOString().split('T')[0];
                    if (!acc[date]) {
                        acc[date] = { Info: 0, Warn: 0, Error: 0, Critical: 0 };
                    }
                    acc[date][item.level] = item.logCount;
                    return acc;
                }, {});

                for (const [date, counts] of Object.entries(groupedData)) {
                    chartData.push([date, counts.Info, counts.Warn, counts.Error, counts.Critical]);
                }

                //console.log('Chart Data:', chartData);
                const dataTable = google.visualization.arrayToDataTable(chartData);
                const options = {
                    title: 'Daily Log Counts',
                    hAxis: { title: 'Date', format: 'yyyy-MM-dd' },
                    vAxis: { title: 'Count' },
                    legend: { position: 'top' },
                    colors: ['#3366CC', '#FF9900', '#DC3912', '#990099'],
                    tooltip: { isHtml: true },
                    series: {
                        0: { color: '#3366CC' }, 
                        1: { color: '#FF9900' }, 
                        2: { color: '#DC3912' }, 
                        3: { color: '#990099' }  
                    },
                    lineWidth: 2,
                    pointSize: 5
                };
                const chart = new google.visualization.LineChart(document.getElementById('logDailyCountChart'));
                chart.draw(dataTable, options);
            })
            .catch(error => {
                console.error('Error fetching chart data:', error);
            });
    }
}



//Pie Chart
function drawPieChart() {
    $("#logsView").hide();
    $("#button").hide();
    $("#buttonBar").hide();
    $(".back-to-list").show();
   // $('#downloadPieChart').show();
   // $('#RegeneratePieChart').show();
    $('#barChart').hide();
    $('#downloadPdfBtn').hide();
    $('#pieChart').show();

    google.charts.load('current', { packages: ['corechart'] });
    google.charts.setOnLoadCallback(fetchAndDrawChart);

    function fetchAndDrawChart() {
        
        const formData = $('#AllFilterForm').serialize();
        //console.log('Serialized Form Data:', formData); 

      
        fetch(`/Log/GetLogCountsForChart?${formData}`)
            .then(response => response.json())
            .then(data => {
                //console.log('Fetched chart data:', data); 

                
                const chartData = [['Log Level', 'Count']];

                const colorMap = {
                    'Info': 'blue',
                    'Warn': 'orange',
                    'Error': 'red',
                    'Critical': 'purple'
                };
                data.forEach((item, index) => {
                    chartData.push([item.logLevel, Number(item.count)]);
                });

                //console.log('Chart Data:', chartData); 
                
                const dataTable = google.visualization.arrayToDataTable(chartData);

                const slices = {};
                data.forEach((item, index) => {
                    slices[index] = { color: colorMap[item.logLevel] || 'gray' }; 
                });

                const options = {
                    title: 'Log Levels Count',
                    pieSliceText: 'label',
                    width: 900,
                    height: 500,
                    legend: { position: 'right' },
                    tooltip: { isHtml: true },
                    slices: slices
                };

                //console.log('Chart Options:', options); 

                const chart = new google.visualization.PieChart(document.getElementById('logLevelPieChart'));
                chart.draw(dataTable, options);
            })
            .catch(error => {
                console.error('Error fetching chart data:', error);
            });
    }
}





function searchLogs() {
    $.ajax({
        url: '/Log/SearchLogs',
        type: 'POST',
        data: $('#search').serialize(),
        success: function (response) {
            if ($(response).find('tbody tr').length === 0) {
                $('#logsView').html('<p clas="text-center">No matching data found.</p>');
            } else {
               
                $('#LogsView').html(response);
                initializeDataTable();
               
            }
        },
        error: function (xhr, status, error) {
            console.error('Search failed:', error);
        }
    });
}
//download the pie chart as pdf
async function downloadPieChartAsPDF() {
    const { jsPDF } = window.jspdf;
    const chartElement = document.getElementById('logLevelPieChart');

    
    const canvas = await html2canvas(chartElement);
    const imageData = canvas.toDataURL('image/png');

   
    const pdf = new jsPDF('landscape');
   
   
    pdf.addImage(imageData, 'PNG', 10, 10, 280, 150); 

    pdf.save('LogPieChart.pdf');
}

//download the line chart as pdf
async function downloadLineChartAsPDF() {
    const { jsPDF } = window.jspdf;
    const chartElement = document.getElementById('logDailyCountChart');

   
    const canvas = await html2canvas(chartElement);
    const imageData = canvas.toDataURL('image/png');

  
    const pdf = new jsPDF('landscape');

    
    pdf.addImage(imageData, 'PNG', 10, 10, 280, 150); 

   
    pdf.save('Daily Report.pdf');
}

//Download the log data table as pdf
async function downloadTableAsPDF() {
    try {
        await loadLogListToDownload();
        setTimeout(async function () {
            const { jsPDF } = window.jspdf;
            const tableContainer = document.getElementById('logList'); 

            if (!tableContainer || !tableContainer.innerHTML.trim()) {
                throw new Error('No data to export.');
            }

            const canvas = await html2canvas(tableContainer, { scale: 2, useCORS: true });
            const imageData = canvas.toDataURL('image/png');

            console.log('Image Data:', imageData); 

           
            if (!imageData.startsWith('data:image/png')) {
                throw new Error('Image data is not valid PNG.');
            }

         
            const pdf = new jsPDF('p', 'mm', 'a4'); 

            const imgWidth = 210; 
            const imgHeight = canvas.height * imgWidth / canvas.width; 
            const pageHeight = 295; 
            let heightLeft = imgHeight;
            let position = 0;

           
            pdf.addImage(imageData, 'PNG', 0, position, imgWidth, imgHeight);
            heightLeft -= pageHeight;

            while (heightLeft > 0) {
                position = heightLeft - imgHeight;
                pdf.addPage();
                pdf.addImage(imageData, 'PNG', 0, position, imgWidth, imgHeight);
                heightLeft -= pageHeight;
            }
            pdf.save('AllDataTable.pdf');
            console.log("Downloading all table data as PDF...");
            //reload the pgae with pagination
            loadLogList();
        }, 1000); 
    } catch (error) {
        console.error('Error generating PDF:', error);
    }
    
}
//load list without datatable
function loadLogListToDownload() {
    var logLevel = $('#logLevel').val();
    var startDate = $('#startDate').val();
    var endDate = $('#endDate').val();

    return new Promise((resolve, reject) => {
        $.ajax({
            url: '/Log/GetLogs',
            type: 'GET',
            data: {
                logLevel: logLevel,
                startDate: startDate,
                endDate: endDate
            },
            success: function (data) {
                if (data && data.length) {
                    $('#logsView').html(data);
                    resolve(); 
                } else {
                    $('#tableBody').html('<td colspan="12">No records found.</td>');
                    resolve(); 
                }
            },
            error: function (error) {
                console.log('Error loading log data: ', error);
                reject(error); 
            }
        });
    });
}


