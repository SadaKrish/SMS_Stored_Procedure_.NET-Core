//load the initail data
$(document).ready(function () {
    function initializeDataTable(data = []) {
        // Check if the DataTable instance already exists
        if ($.fn.DataTable.isDataTable("#studentlist")) {
            $("#studentlist").DataTable().destroy();
        }

        $("#studentlist").DataTable({
            "data": data,
            "columns": [
                { "data": "studentRegNo" }, // Adjusted to match the property names in your JSON objects
                { "data": "firstName" },
                { "data": "middleName" },
                { "data": "lastName" },
                { "data": "displayName" },
                {
                    "data": "email",
                    "render": function (data) {
                        return "<a href='mailto:" + data + "'>" + data + "</a>";
                    }
                },
                { "data": "gender" },
                {
                    "data": "dob",
                    "render": function (data) {
                        if (data) {
                            // Format the date as "YYYY-MM-DD"
                            var date = new Date(data);
                            return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
                        } else {
                            return ''; // Return an empty string if DOB is null or undefined
                        }
                    }
                },
                { "data": "address" },
                {
                    "data": "contactNo",
                    "render": function (data) {
                        return "<a href='tel:" + data + "'>" + data + "</a>";
                    }
                },
                {
                    "data": "isEnable",
                    "render": function (data, type, row) {
                        // Render a checkbox based on the isEnable value
                        var checkbox = '<input type="checkbox" id="studentCheckbox_' + row.studentID + '" ';
                        if (data) {
                            checkbox += 'checked ';
                        }
                        checkbox += 'onclick="ToggleEnable(' + row.studentID + ', this.checked, \'' + row.displayName.replace(/'/g, "\\'") + '\')" />';
                        return checkbox;
                    }
                },
                {
                    "data": "studentID",
                    "render": function (data, type, row) {
                        // Return a placeholder initially
                        var placeholder = "<span id='student-" + data + "'>Loading...</span>";

                        // Make an AJAX call to check the allocation status
                        $.ajax({
                            type: "GET",
                            url: '/Student/CheckStudentAllocationStatus/',
                            data: { studentID: data },
                            success: function (response) {
                                if (response.isAllocated) {
                                    $("#student-" + data).replaceWith("<span class='badge bg-success'>Allocated</span>");
                                } else {
                                    $("#student-" + data).replaceWith(
                                        "<a class='btn' style='background-color: #cbf5ee;' onclick=LoadEditPage(" + data + ")><i class='bi bi-pencil-square'></i></a>  <a class='btn' style='background-color: #cbf5ee;' onclick=Delete(" + data + ")><i class='bi bi-trash' style='color:red;'></i></a>"
                                    );
                                }
                            },
                            error: function () {
                                $("#student-" + data).replaceWith("<span class='badge bg-danger'>Error</span>");
                            }
                        });

                        // Return the placeholder initially
                        return placeholder;
                    },
                    "orderable": false,
                    "searchable": false
                }
            ],
            "pageLength": 5,
            "lengthMenu": [5, 10, 15, 20, 25],
            "searching": false
        });
        //return datatable;
    }

    function searchAllocations() {
        var searchText = $('#customSearchBox').val().trim();
        var searchCategory = $('#searchCategory').val();

        $.ajax({
            url: '/Student/SearchStudents/',
            type: 'POST',
            data: { searchText: searchText, searchCategory: searchCategory },
            success: function (response) {
                if (response.success) {
                    // Reinitialize the DataTable with the new data
                    initializeDataTable(response.data);
                } else {
                    console.error('Search failed:', response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
            }
        });
    }

    // Initial DataTable load
    initializeDataTable();

    // Event handlers for search input and button
    $('#customSearchBox').on('keyup', searchAllocations);
    $('#searchButton').on('click', searchAllocations);
});



//Load create page
function LoadCreatePage() {
    //* /Hide student list and load create new form*/
    $("#studentlist").hide();
    $("#button").hide();
    $('.panel-body').hide();
    //  $("createoredit").show();
    $("#back-to-list").show();
    $('.footer').hide();
    $.ajax({
        type: "GET",
        url: '/Student/AddOrEditStudent',

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
    // Hide student list, button, and panel body
    $("#studentlist").hide();
    $("#button").hide();
    $('.panel-body').hide();
    $("#back-to-list").show(); // Show the "Back to List" button
    $('.footer').hide();
    // Make an AJAX call to retrieve the edit form for the specified student ID
    $.ajax({
        type: "GET",
        url: '/Student/AddOrEditStudent/' + studentId,
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

                // Show the student list, button, panel body, and filter
                $("#studentlist").show();
                $("#button").show();
                $('.panel-body').show();
                $(".filter").show();
                //$('.footer').show();
                // Hide the "Back to List" button
                $("#back-to-list").hide();

                // Reload the data table
                //var datatable = initializeDataTable();
                $("#studentlist").DataTable().ajax.reload();
            }
        });
    } else {
        // Clear previous validation messages
        $('.text-danger').text('');

        // Show validation messages under each field
        $.each(response.errors, function (index, error) {
            $.each(error.Errors, function (key, message) {
                var errorMessage = $('<span>').text(message).addClass('text-danger');
                $('[name="' + error.Key + '"]').after(errorMessage);
            });
        });

        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: response.message
        });
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
    $('#back-to-list').hide(); // Hide the "Back to List" button
    $("#createoredit").empty();
    $('.panel-body').show();
    $('.filter').show();
    //$('.footer').show();

    var datatable = $('#studentlist').DataTable(); 
    datatable.ajax.reload(null, false); // Reload data without resetting pagination
}

function initializeDataTable(status = 'all') {
    // Check if the DataTable instance already exists
    if ($.fn.DataTable.isDataTable("#studentlist")) {
        $("#studentlist").DataTable().destroy();
    }
    var datatable = $("#studentlist").DataTable({
        "ajax": {
            "url": "/Student/GetStudents",
            "type": "GET",
            "datatype": "json",
            "data": {
                status: status
            },
            "dataSrc": function (json) {
                console.log("AJAX response:", json); // Debugging: Log the entire response
                if (json.success) {
                    console.log("Data retrieved successfully:", json.data); // Debugging: Log retrieved data
                    return json.data; // Return the array of student objects
                } else {
                    console.error("Error message:", json.message); // Debugging: Log error message
                    return []; // Return an empty array if there's an error
                }
            },
            "error": function (jqXHR, textStatus, errorThrown) {
                console.error("AJAX error:", textStatus, errorThrown); // Debugging: Log AJAX errors
            }
        },
        "columns": [
            { "data": "studentRegNo" }, // Adjusted to match the property names in your JSON objects
            { "data": "firstName" },
            { "data": "middleName" },
            { "data": "lastName" },
            { "data": "displayName" },
            {
                "data": "email",
                "render": function (data) {
                    return "<a href='mailto:" + data + "'>" + data + "</a>";
                }
            },
            { "data": "gender" },
            {
                "data": "dob",
                "render": function (data) {
                    if (data) {
                        // Format the date as "YYYY-MM-DD"
                        var date = new Date(data);
                        return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
                    } else {
                        return ''; // Return an empty string if DOB is null or undefined
                    }
                }
            },
            { "data": "address" },
            {
                "data": "contactNo",
                "render": function (data) {
                    return "<a href='tel:" + data + "'>" + data + "</a>";
                }
            },
            {
                "data": "isEnable",
                "render": function (data, type, row) {
                    // Render a checkbox based on the isEnable value
                    var checkbox = '<input type="checkbox" id="studentCheckbox_' + row.studentID + '" ';
                    if (data) {
                        checkbox += 'checked ';
                    }
                    checkbox += 'onclick="ToggleEnable(' + row.studentID + ', this.checked, \'' + row.displayName.replace(/'/g, "\\'") + '\')" />';
                    return checkbox;
                }
            },
            {
                "data": "studentID",
                "render": function (data, type, row) {
                    // Return a placeholder initially
                    var placeholder = "<span id='student-" + data + "'>Loading...</span>";

                    // Make an AJAX call to check the allocation status
                    $.ajax({
                        type: "GET",
                        url: '/Student/CheckStudentAllocationStatus/',
                        data: { studentID: data },
                        success: function (response) {
                            if (response.isAllocated) {
                                $("#student-" + data).replaceWith("<span class='badge bg-success'>Allocated</span>");
                            } else {
                                $("#student-" + data).replaceWith(
                                    "<a class='btn' style='background-color: #cbf5ee;' onclick=LoadEditPage(" + data + ")><i class='bi bi-pencil-square'></i></a>  <a class='btn' style='background-color: #cbf5ee;' onclick=Delete(" + data + ")><i class='bi bi-trash' style='color:red;'></i></a>"
                                );
                            }
                        },
                        error: function () {
                            $("#student-" + data).replaceWith("<span class='badge bg-danger'>Error</span>");
                        }
                    });

                    // Return the placeholder initially
                    return placeholder;
                },
                "orderable": false,
                "searchable": false
            }
        ],
        "pageLength": 5,
        "lengthMenu": [5, 10, 15, 20, 25],
        "searching": false
    });
    // return datatable;
}

$(document).ready(function () {
    initializeDataTable();

    $('#filterStatus').on('change', function () {
        var status = $(this).val();
        initializeDataTable(status);
    });
});



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
            console.log(url); // Log the URL to the console

            $.ajax({
                type: "POST",
                url: url,
                success: function (data) {
                    if (data.success) {
                        $("#studentlist").DataTable().ajax.reload();

                        Swal.fire({
                            title: 'Deleted!',
                            text: data.message,
                            icon: 'success'
                        });
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
            // Proceed with toggling the enable status of the student
            $.ajax({
                type: 'POST',
                url: '/Student/ToggleEnableStudent/',
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        $("#studentlist").DataTable().ajax.reload(); // Reload the data table
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