$(document).ready(function () {
    loadSubjectList();

    $('#filterStatus').on('change', function () {
        loadSubjectList();
    });

    $("#search").on("submit", function (event) {
        event.preventDefault();
        searchSubjects();
    });

    $('#customSearchBox').on('keyup', function () {
        var searchTerm = $(this).val().trim();
        if (searchTerm.length >= 1) {
            searchSubjectsOptions();
        } else if (searchTerm.length === 0) {
            loadSubjectList();
        }
    });

    $('#searchButton').on('click', function () {
        searchSubjects();
    });


});

function loadSubjectList() {
    var status = $('#filterStatus').val();
    $.ajax({
        url: '/Subject/GetSubjects',
        type: 'GET',
        data: { status: status },
        success: function (data) {
            $('#subjectslist').html(data);
            initializeDataTable();
            //updateTeacherStatuses();
        },
        error: function (error) {
            console.log('Error loading subject data: ', error);
        }
    });
}
function searchTeachersOptions() {
    $("#customSearchBox").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Subject/GetSearchSuggestions',
                type: 'GET',
                dataType: 'json',
                data: {
                    term: request.term,
                    category: $("#searchCategory").val()
                },
                success: function (data) {
                    console.log(data);
                    if (Array.isArray(data)) {
                        response(data.map(item => ({
                            label: item.label,
                            value: item.value,
                            data: item.data
                        })));
                    } else {
                        console.error('Expected an array but got:', data);
                    }
                },
                error: function () {
                    console.error('Error fetching search suggestions.');
                }
            });

        },
        minLength: 1,
        select: function (event, ui) {
            //console.log('Selected: ' + ui.item.label);
            //console.log(ui.item.data);
            $("#customSearchBox").val(ui.item.value);
            searchSubjects();
        }
    });
}
function searchSubjects() {
    $.ajax({
        url: '/Subject/SearchSubjects',
        type: 'POST',
        data: $('#search').serialize(),
        success: function (response) {
            if ($(response).find('tbody tr').length === 0) {
                $('#subjectslist').html('<p clas="text-center">No matching data found.</p>');
            } else {
                $('#subjectslist').html(response);
                initializeDataTable();
                updateTeacherStatuses();
            }
        },
        error: function (xhr, status, error) {
            console.error('Search failed:', error);
        }
    });
}

function initializeDataTable() {
    $('#subjectlist').DataTable({
        "pageLength": 5,
        "lengthMenu": [5, 10, 15, 20, 25],
        "searching": false,
        "ordering": true,
        "paging": true,
        "drawCallback": function (settings) {
            updateSubjectStatuses();
        }
    });
}

//Load create page
function LoadCreatePage() {

    $("#subjectlist").hide();
    $("#button").hide();
    $('.panel-body').hide();
    $("#back-to-list").show();
    $('.footer').hide();
    $.ajax({
        type: "GET",
        url: '/Subject/UpsertSubject',
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
function LoadEditPage(subjectId) {

    $("#subjectlist").hide();
    $("#button").hide();
    $('.panel-body').hide();
    $("#back-to-list").show();
    $('.footer').hide();

    $.ajax({
        type: "GET",
        url: '/Teacher/UpsertSubject/' + subjectId,
        cache: false,
        success: function (data) {
            $("#createoredit").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Error loading edit form');
        }
    });
}
function addSubjectSuccess(response) {
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
                $("#subjectlist").show();
                $("#button").show();
                $('.panel-body').show();
                $(".filter").show();
                $('.footer').show();
                $("#back-to-list").hide();

                loadSubjectList();
            }
        });
    } else {


    }
}

function addSubjectFailure(error) {
    console.error(error);
    Swal.fire('Error!', 'An error occurred while adding the student', 'error');
}
//back button actions

function LoadList() {
    $("#subjectlist").show();
    $('#button').show();
    $('#back-to-list').hide();
    $("#createoredit").empty();
    $('.panel-body').show();
    $('.filter').show();
    $('.footer').show();

}

function updateSubjectStatuses() {
    $('#subjectlist tbody tr').each(function () {
        var subjectID = $(this).data('subject-id');
        // console.log("Checking status for student ID:", studentID);

        if (subjectID) {
            $.ajax({
                type: "GET",
                url: '/Subject/CheckSubjectAllocationStatus/',
                data: { subjectID: subjectID },
                success: function (response) {
                    //  console.log("Response for student ID", studentID, ":", response);
                    var statusCell = $('#subject-status-' + subjectID);
                    if (response.exists) {
                        statusCell.html("<span class='badge bg-success'>Allocated</span>");
                    } else {
                        statusCell.html(
                            "<a class='btn' style='background-color: #cbf5ee;' onclick='LoadEditPage(" + subjectID + ")'><i class='bi bi-pencil-square'></i></a> " +
                            "<a class='btn' style='background-color: #cbf5ee;' onclick='Delete(" + subjectID + ")'><i class='bi bi-trash' style='color:red;'></i></a>"
                        );
                    }
                },
                error: function () {
                    $('#subject-status-' + subjectID).html("<span class='badge bg-danger'>Error</span>");
                }
            });
        } else {
            console.error("Subject ID not found for row:", $(this));
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
            var url = '/Subject/DeleteSubject/' + id;
            // console.log(url); 

            $.ajax({
                type: "POST",
                url: url,
                success: function (data) {
                    if (data.success) {
                        loadSubjectList();

                        Swal.fire({
                            title: 'Deleted!',
                            text: data.message,
                            icon: 'success'

                        });
                        // refreshTeacherList();
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
function ToggleEnable(id, isChecked, Name) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'Do you want to toggle the enable status of this Subject ' + Name + '?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            // Proceed with toggling the enable status of the teacher
            $.ajax({
                type: 'POST',
                url: '/Subject/ToggleEnableSubject/',
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        loadSubjectList();
                        Swal.fire({
                            title: 'Success!',
                            text: response.message,
                            icon: 'success'
                        });
                    } else {
                        $('#subjectCheckbox_' + id).prop('checked', !isChecked);
                        Swal.fire({
                            title: 'Warning!',
                            text: response.message,
                            icon: 'warning'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        title: 'Error!',
                        text: 'An error occurred while processing your request.',
                        icon: 'error'
                    });
                }
            });
        } else {
            $('#subjectCheckbox_' + id).prop('checked', !isChecked);
        }
    });
}

//function copyDataToClipboard(data, buttonElement) {

//    var tempTextArea = document.createElement("textarea");
//    tempTextArea.value = data;
//    document.body.appendChild(tempTextArea);

//    tempTextArea.select();
//    document.execCommand("copy");

//    document.body.removeChild(tempTextArea);

//    var copiedMessage = buttonElement.nextElementSibling;
//    if (copiedMessage && copiedMessage.classList.contains('copiedMessage')) {
//        copiedMessage.style.display = "block";

//        setTimeout(function () {
//            copiedMessage.style.display = "none";
//        }, 2000);
//    } else {
//        console.error('Copied message element not found.');
//    }
//}




