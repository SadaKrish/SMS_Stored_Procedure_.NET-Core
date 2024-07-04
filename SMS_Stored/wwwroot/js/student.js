$(document).ready(function () {
    loadStudentList();

    $('#filterStatus').on('change', function () {
        loadStudentList();
    });

    $("#search").on("submit", function (event) {
        event.preventDefault();
        searchStudents();
    });

    $('#customSearchBox').on('keyup', function () {
        searchStudents();
    });

    $('#searchButton').on('click', function () {
        searchStudents();
    });
});

function loadStudentList() {
    var status = $('#filterStatus').val();
    $.ajax({
        url: '/Student/GetStudents',
        type: 'GET',
        data: { status: status },
        success: function (data) {
            $('#studentslist').html(data);
            initializeDataTable();
            updateStudentStatuses();
        },
        error: function (error) {
            console.log('Error loading student data: ', error);
        }
    });
}
function searchStudents() {
    $.ajax({
        url: '/Student/SearchStudents',
        type: 'POST',
        data: $('#search').serialize(),
        success: function (response) {
            if ($(response).find('tbody tr').length === 0) {
                $('#studentslist').html('<p clas="text-center">No matching data found.</p>');
            } else {
                // Directly set the HTML content to the container
                $('#studentslist').html(response);
                initializeDataTable();
                updateStudentStatuses();
            }
        },
        error: function (xhr, status, error) {
            console.error('Search failed:', error);
        }
    });
}

function initializeDataTable() {
    $('#studentlist').DataTable({
        "pageLength": 5,
        "lengthMenu": [5, 10, 15, 20, 25],
        "searching": false,
        "ordering": true,
        "paging": true,
        "drawCallback": function (settings) {
            updateStudentStatuses();
        }
    });
}

//Load create page
function LoadCreatePage() {
    
    $("#studentlist").hide();
    $("#button").hide();
    $('.panel-body').hide();
    $("#back-to-list").show();
    $('.footer').hide();
    $.ajax({
        type: "GET",
        url: '/Student/UpsertStudent',
        data: {},
        cache: false,
        async: false,

        success: function (data) {
            $("#createoredit").html(data)
        },
        error: function (xhr, ajaxOptions, thrownError) {
            aler('');
        }
    });
}

//load edit page
function LoadEditPage(studentId) {
 
    $("#studentlist").hide();
    $("#button").hide();
    $('.panel-body').hide();
    $("#back-to-list").show(); 
    $('.footer').hide();
  
    $.ajax({
        type: "GET",
        url: '/Student/UpsertStudent/' + studentId,
        cache: false,
        success: function (data) {
            $("#createoredit").html(data); 
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Error loading edit form');
        }
    });
}
function addStudentSuccess(response) {
    if (response.success) {
        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: response.message,
            showCancelButton: false,
            confirmButtonText: 'OK'
        }).then((result) => {
            if (result.isConfirmed) {
                $("#createoredit").empty();
                $("#studentlist").show();
                $("#button").show();
                $('.panel-body').show();
                $(".filter").show();
                $('.footer').show();
                $("#back-to-list").hide();
                
                loadStudentList();
            }
        });
    } else {
     
        
    }
}

function addStudentFailure(error) {
    console.error(error);
    Swal.fire('Error!', 'An error occurred while adding the student', 'error');
}
//back button actions

function LoadList() {
    $("#studentlist").show();
    $('#button').show();
    $('#back-to-list').hide(); 
    $("#createoredit").empty();
    $('.panel-body').show();
    $('.filter').show();
    $('.footer').show();
    
}

function updateStudentStatuses() {
    $('#studentlist tbody tr').each(function () {
        var studentID = $(this).data('student-id'); 
       // console.log("Checking status for student ID:", studentID);

        if (studentID) {
            $.ajax({
                type: "GET",
                url: '/Student/CheckStudentAllocationStatus/',
                data: { studentID: studentID },
                success: function (response) {
                  //  console.log("Response for student ID", studentID, ":", response);
                    var statusCell = $('#student-status-' + studentID);
                    if (response.exists) {
                        statusCell.html("<span class='badge bg-success'>Allocated</span>");
                    } else {
                        statusCell.html(
                            "<a class='btn' style='background-color: #cbf5ee;' onclick='LoadEditPage(" + studentID + ")'><i class='bi bi-pencil-square'></i></a> " +
                            "<a class='btn' style='background-color: #cbf5ee;' onclick='Delete(" + studentID + ")'><i class='bi bi-trash' style='color:red;'></i></a>"
                        );
                    }
                },
                error: function () {
                    $('#student-status-' + studentID).html("<span class='badge bg-danger'>Error</span>");
                }
            });
        } else {
            console.error("Student ID not found for row:", $(this));
        }
    });
}


function Delete(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            var url = '/Student/DeleteStudent/' + id;
           // console.log(url); 

            $.ajax({
                type: "POST",
                url: url,
                success: function (data) {
                    if (data.success) {
                        loadStudentList();

                        Swal.fire({
                            title: 'Deleted!',
                            text: data.message,
                            icon: 'success'
                            
                        });
                        refreshStudentList();
                    } else {
                        Swal.fire({
                            title: 'Warning!',
                            text: data.message,
                            icon: 'warning'
                        });
                    }
                }
            });
        }
    });
}


//enable button
function ToggleEnable(id, isChecked, displayName) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'Do you want to toggle the enable status of this student ' + displayName + '?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
       
            $.ajax({
                type: 'POST',
                url: '/Student/ToggleEnableStudent/',
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        loadStudentList();
                        Swal.fire({
                            title: 'Success!',
                            text: response.message,
                            icon: response.message.includes('Warning') ? 'warning' : 'success' 
                        });
                    } else {
                        $('#studentCheckbox_' + id).prop('checked', !isChecked);
                        Swal.fire({
                            title: 'Error!',
                            text: response.message,
                            icon: 'error'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    $('#studentCheckbox_' + id).prop('checked', !isChecked);
                    Swal.fire({
                        title: 'Error!',
                        text: 'An error occurred while processing your request.',
                        icon: 'error'
                    });
                }
            });
        } else {
            $('#studentCheckbox_' + id).prop('checked', !isChecked);
        }
    });
}
function copyDataToClipboard(data, buttonElement) {
    
    var tempTextArea = document.createElement("textarea");
    tempTextArea.value = data;
    document.body.appendChild(tempTextArea);
   
    tempTextArea.select();
    document.execCommand("copy");
   
    document.body.removeChild(tempTextArea);

    var copiedMessage = buttonElement.nextElementSibling;
    if (copiedMessage && copiedMessage.classList.contains('copiedMessage')) {
        copiedMessage.style.display = "block";

        setTimeout(function () {
            copiedMessage.style.display = "none";
        }, 2000);
    } else {
        console.error('Copied message element not found.');
    }
}




