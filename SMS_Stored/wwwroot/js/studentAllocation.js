
$(document).ready(function () {
    loadStudentAllocationList();

    $('#filterStatus').on('change', function () {
        loadStudentAllocationList()();
    });

    $("#search").on("submit", function (event) {
        event.preventDefault();
        searchSubjectAllocations();
    });

    $('#customSearchBox').on('keyup', function () {
        var searchTerm = $(this).val().trim();
        if (searchTerm.length >= 1) {
            searchSubjectAllocationsOptions();
        } else if (searchTerm.length === 0) {
            loadStudentAllocationList()();
        }
        searchSubjectAllocations();
    });

    $('#searchButton').on('click', function () {
        searchSubjectAllocations();
    });


});
function loadStudentAllocationList() {
    $.ajax({
        url: '/Allocation/GetStudentAllocations',
        type: 'GET',

        success: function (data) {
            $('#studentsAllocationList').html(data);
            //initializeDataTable();
            //updateStudentStatuses();
        },
        error: function (error) {
            console.log('Error loading student data: ', error);
        }
    });
}
function searchSubjectAllocationsOptions() {
    $("#customSearchBox").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Allocation/GetSearchSuggestions',
                type: 'GET',
                dataType: 'json',
                data: {
                    term: request.term,
                    category: $("#searchCategory").val()
                },
                success: function (data) {
                    if (Array.isArray(data)) {
                        response(data.map(item => ({
                            label: item.SubjectCode + ' - ' + item.SubjectName,  // Customize label as needed
                            value: item.SubjectCode,  // Customize value as needed
                            data: item  // Optional, if you need to send extra data
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
            $("#customSearchBox").val(ui.item.value);
            searchSubjectAllocations();  // Call your search function here
        }
    });
}

function searchSubjectAllocations() {
    $.ajax({
        url: '/Allocation/SearchSubjectAllocation',
        type: 'POST',
        data: $('#search').serialize(),
        success: function (response) {
            if ($(response).find('tbody tr').length === 0) {
                $('#subjectsAllocationList').html('<p clas="text-center">No matching data found.</p>');
            } else {
                $('#subjectsAllocationList').html(response);
                initializeDataTable();
                // updateTeacherStatuses();
            }
        },
        error: function (xhr, status, error) {
            console.error('Search failed:', error);
        }
    });
}
function initializeDataTable() {
    $('#subjectAllocationList').DataTable({
        "pageLength": 5,
        "lengthMenu": [5, 10, 15, 20, 25],
        "searching": false,
        "ordering": true,
        "paging": true,
        info: true,
        //"drawCallback": function (settings) {
        //    updateStudentStatuses();
        //}
    });
}

function LoadCreatePage() {
    $("#subjectAllocationList").hide();
    $("#button").hide();
    $('.panel-body').hide();
    $("#back-to-list").show();
    $('.footer').hide();

    $.ajax({
        type: "GET",
        url: '/Allocation/UpsertSubjectAllocation',
        data: {},
        cache: false,
        async: false,
        success: function (data) {
            $("#createoredit").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('An error occurred: ' + thrownError); // Changed from aler to alert
        }
    });
}


//load edit page
function LoadEditPage(subjectAllocationId) {
    console.log('Loading edit page for ID:', subjectAllocationId);

    $("#subjectAllocationList").hide();
    $("#button").hide();
    $('.panel-body').hide();
    $("#back-to-list").show();
    $('.footer').hide();

    $.ajax({
        type: "GET",
        url: '/Allocation/UpsertSubjectAllocation/' + subjectAllocationId,
        cache: false,
        success: function (data) {
            $("#createoredit").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Error loading edit form');
        }
    });
}
function addSubjectAllocationSuccess(response) {
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
                $("#subjectAllocationList").show();
                $("#button").show();
                $('.panel-body').show();
                $(".filter").show();
                $('.footer').show();
                $("#back-to-list").hide();

                loadSubjectAllocationList();
            }
        });
    } else {

        alert('An error occurred');

    }
}

function addSubjectAllocationFailure(error) {
    console.error(error);
    Swal.fire('Error!', 'An error occurred while adding the student', 'error');
}
//back button actions

function LoadList() {
    $("#subjectAllocationList").show();
    $('#button').show();
    $('#back-to-list').hide();
    $("#createoredit").empty();
    $('.panel-body').show();
    $('.filter').show();
    $('.footer').show();

}

function Delete(id) {
    console.log('Loading delete for ID:', id);
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
            var url = '/Allocation/DeleteStudentAllocation/' + id;
            // console.log(url); 

            $.ajax({
                type: "POST",
                url: url,
                success: function (data) {
                    if (data.success) {
                        loadSubjectAllocationList();

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