$(document).ready(function () {
    $('#pieChart').hide();
    $('#barChart').hide();

    var today = new Date();
    var formattedToday = formatDateToDDMMYY(today);

 
    $("#startDate, #endDate").val(formattedToday);

   
    $("#startDate").datepicker({
        dateFormat: "dd/mm/yy",
        onSelect: function (selectedDate) {
            var dateParts = selectedDate.split('/');
            var startDate = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]); 
            $("#endDate").datepicker("option", "minDate", startDate);
            loadLogList();
        }
    });

    $("#endDate").datepicker({
        dateFormat: "dd/mm/yy",
        onSelect: function (selectedDate) {
            var dateParts = selectedDate.split('/');
            var endDate = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]); 
            $("#startDate").datepicker("option", "maxDate", endDate);
            loadLogList();
        }
    });

    $('#logLevel').on('change', function () {
        loadLogList();
    });

    $('#startDate').on('change', function () {
        loadLogList();
    });

    $('#endDate').on('change', function () {
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

    $('#downloadPdfBtn').click(function (event) {
        event.preventDefault();

      
        const formDataArray = $('#AllFilterForm').serializeArray();
        const formData = {};

        formDataArray.forEach(item => {
            formData[item.name] = item.value;
        });

        
        if (formData.startDate) {
            formData.startDate = convertDateFormat(formData.startDate);
        }
        if (formData.endDate) {
            formData.endDate = convertDateFormat(formData.endDate);
        }

     
        const queryString = $.param(formData);

        $.ajax({
            url: `/Log/ExportToPdf?${queryString}`,
            type: 'GET',
            xhrFields: {
                responseType: 'blob'
            },
            success: function (blob) {
                var link = document.createElement('a');
                var url = window.URL.createObjectURL(blob);
                link.href = url;
                link.download = `LogList_${new Date().toISOString().slice(0, 19).replace(/[:T]/g, '')}.pdf`; 
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
            },
            error: function (error) {
                console.log('Error exporting PDF: ', error.responseText || error.statusText);
            }
        });
    });

    loadLogList();
});
function getFormData(formSelector) {
    const formDataArray = $(formSelector).serializeArray();
    const formData = {};

    formDataArray.forEach(item => {
        formData[item.name] = item.value;
    });

    if (formData.startDate) {
        formData.startDate = convertDateFormat(formData.startDate);
    }
    if (formData.endDate) {
        formData.endDate = convertDateFormat(formData.endDate);
    }

    return formData;
}
//Formating the date 
function formatDateToDDMMYY(date) {
    var day = String(date.getDate()).padStart(2, '0');
    var month = String(date.getMonth() + 1).padStart(2, '0'); 
    var year = date.getFullYear();
    return day + '/' + month + '/' + year;
}
//converting date format
function convertDateFormat(date) {
    var dateParts = date.split('/');
    return dateParts[2] + '-' + dateParts[1] + '-' + dateParts[0];
}

// Load the log table
function loadLogList() {
  
    const formData = getFormData('#AllFilterForm');

   
    //var startDateObj = formData.find(obj => obj.name === 'startDate');
    //var endDateObj = formData.find(obj => obj.name === 'endDate');

    //if (startDateObj && startDateObj.value) {
    //    startDateObj.value = convertDateFormat(startDateObj.value);
    //}

    //if (endDateObj && endDateObj.value) {
    //    endDateObj.value = convertDateFormat(endDateObj.value);
    //}

    $.ajax({
        url: '/Log/GetLogs',
        type: 'GET',
        data: $.param(formData),
        success: function (data) {
            if (data && data.length) {
                $('#logsView').html(data);
                initializeDataTable();
            } else {
                $('#tableBody').html('<td colspan="12">No records found.</td>');
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


function formatDateToDDMMYY(date) {
    var day = String(date.getDate()).padStart(2, '0');
    var month = String(date.getMonth() + 1).padStart(2, '0'); 
    var year = date.getFullYear();
    return day + '/' + month + '/' + year;
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
      
        const formData = getFormData('#AllFilterForm');
        //const formData = {};

        //formDataArray.forEach(item => {
        //    formData[item.name] = item.value;
        //});

        //if (formData.startDate) {
        //    formData.startDate = convertDateFormat(formData.startDate);
        //}
        //if (formData.endDate) {
        //    formData.endDate = convertDateFormat(formData.endDate);
        //}

        const queryString = $.param(formData);

        fetch(`/Log/GetDailyLogCounts?${queryString}`)
            .then(response => response.json())
            .then(data => {
                const chartData = [['Date', 'Info', 'Warn', 'Error', 'Debug']];

                const groupedData = data.reduce((acc, item) => {
                    const date = new Date(item.logDate).toISOString().split('T')[0];
                    if (!acc[date]) {
                        acc[date] = { Info: 0, Warn: 0, Error: 0, Debug: 0 };
                    }
                    acc[date][item.level] = item.logCount;
                    return acc;
                }, {});

                for (const [date, counts] of Object.entries(groupedData)) {
                    const total = counts.Info + counts.Warn + counts.Error + counts.Debug;
                    if (total > 0) {
                        chartData.push([date, counts.Info, counts.Warn, counts.Error, counts.Debug]);
                    }
                }

                const dataTable = google.visualization.arrayToDataTable(chartData);
                const options = {
                    title: 'Daily Log Counts',
                    hAxis: { title: 'Date', format: 'dd-MM-yyyy' },
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
        
        const formData = getFormData('#AllFilterForm');
        //const formData = {};

        //formDataArray.forEach(item => {
        //    formData[item.name] = item.value;
        //});

      
        //if (formData.startDate) {
        //    formData.startDate = convertDateFormat(formData.startDate);
        //}
        //if (formData.endDate) {
        //    formData.endDate = convertDateFormat(formData.endDate);
        //}

       
        const queryString = $.param(formData);

      
        fetch(`/Log/GetLogCountsForChart?${queryString}`)
            .then(response => response.json())
            .then(data => {
                //console.log('Fetched chart data:', data); 

                
                const chartData = [['Log Level', 'Count']];

                const colorMap = {
                    'Info': 'blue',
                    'Warn': 'orange',
                    'Error': 'red',
                    'Debug': 'purple'
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


