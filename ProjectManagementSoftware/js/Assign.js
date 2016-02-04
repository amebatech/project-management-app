$(document).ready(function () {
    GetAll();

    $('.btnCreate').on('click', function () {
        $.ajax({
            url: 'Assigns/Create',
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            success: function (data) { 
                $('#modelContent').html(data);
                $('#myModal').modal('show');
            },
            error: function (data) {
                alert('Error in creating records');
            }
        });
    });

    $(document).delegate('.btnBack', 'click', function () {
        $.ajax({
            url: 'Assigns/GetAll',
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $('#divList').html(data);
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnAddTask', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('addTask_', '').trim();
        $.ajax({
            url: 'Assigns/AddTask',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId },
            success: function (data) {
                $('#modelContent').html(data);
                $('#myModal').modal('show');
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnAddSubTask', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('addSubTask_', '').trim();
        $.ajax({
            url: 'Assigns/AddSubTask',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId },
            success: function (data) {
                $('#modelContent').html(data);
                $('#myModal').modal('show');
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnAddManager', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('addManager_', '').trim();
        $.ajax({
            url: 'Assigns/AddManager',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId },
            success: function (data) {
                //$('#modelContent').html(data);
                //$('#myModal').modal('show');
                $('#divList').html(data);
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnAddEmployee', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('addEmployee_', '').trim();
        $.ajax({
            url: 'Assigns/AddEmployee',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId },
            success: function (data) {
                //$('#modelContent').html(data);
                //$('#myModal').modal('show');
                $('#divList').html(data);
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnCreateTask', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('edit_', '').trim();
        $.ajax({
            url: 'Tasks/Create',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId },
            success: function (data) {
                $('#modelContent').html(data);
                $('#myModal').modal('show');
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnAssignTask', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('assignTask_', '').trim();
        var gettId = document.getElementById('hdnTaskId').value;
        $.ajax({
            url: 'Assigns/AssignTask',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId, tId: gettId },
            success: function (data) {
                //$('#modelContent').html(data);
                //$('#myModal').modal('show');
                $('#divList').html(data);
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnAssignManager', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('assignManager_', '').trim();
        var getPId = document.getElementById('hdnProjectId').value;
        $.ajax({
            url: 'Assigns/AssignManager',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId, pId: getPId },
            success: function (data) {
                //$('#modelContent').html(data);
                //$('#myModal').modal('show');
                $('#divList').html(data);
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnDeleteEmployee', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('deleteEmployee_', '').trim();
        var gettId = document.getElementById('hdnTaskId').value;
        $.ajax({
            url: 'Assigns/DeleteEmployee',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId, tId: gettId },
            success: function (data) {
                //$('#modelContent').html(data);
                //$('#myModal').modal('show');
                $('#divList').html(data);
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnMonitor', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('monitor_', '').trim();
        var gettId = document.getElementById('hdnTaskId').value;
        //alert(studentId);
        $.ajax({
            url: 'Assigns/Monitor',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId, tId: gettId },
            success: function (data) {
                $('#modelContent').html(data);
                $('#myModal').modal('show');
                //$('#divList').html(data);
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnControl', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('monitor_', '').trim();
        alert(studentId);
        $.ajax({
            url: 'Assigns/Monitor',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId },
            success: function (data) {
                $('#modelContent').html(data);
                $('#myModal').modal('show');
                //$('#divList').html(data);
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnDeleteManager', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('deleteManager_', '').trim();
        var getPId = document.getElementById('hdnProjectId').value;
        $.ajax({
            url: 'Assigns/DeleteManager',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId, pId: getPId },
            success: function (data) {
                //$('#modelContent').html(data);
                //$('#myModal').modal('show');
                $('#divList').html(data);
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnEdit', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('edit_', '').trim();
        $.ajax({
            url: 'Assigns/Edit',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data:{Id:studentId},
            success: function (data) {
                $('#modelContent').html(data);
                $('#myModal').modal('show');
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnDetails', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('delails_', '').trim();
        $.ajax({
            url: 'Assigns/Details',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId },
            success: function (data) {
                $('#modelContent').html(data);
                $('#myModal').modal('show');
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnProjectDetails', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('projDelails_', '').trim();
        $.ajax({
            url: 'Assigns/ProjectDetails',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId },
            success: function (data) {
                $('#modelContent').html(data);
                $('#myModal').modal('show');
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnDetailsManager', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('delailsManager_', '').trim();
        $.ajax({
            url: 'Employees/Details',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId },
            success: function (data) {
                $('#modelContent').html(data);
                $('#myModal').modal('show');
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnDetailsEmployee', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('delailsEmployee_', '').trim();
        $.ajax({
            url: 'Employees/Details',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { Id: studentId },
            success: function (data) {
                $('#modelContent').html(data);
                $('#myModal').modal('show');
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    $(document).delegate('.btnDelete', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('delete_', '').trim();
        bootbox.confirm("Are you sure want to delete this?", function (result) {
            if (result) {
                $.ajax({
                    url: 'Assigns/Delete',
                    type: 'get',
                    contentType: 'application/json; charset=utf-8',
                    data: { Id: studentId },
                    success: function (data) {
                        GetAll();
                    },
                    error: function (data) {
                        alert('Error in getting result');
                    }
                });
            }
        });

    });

    $(document).delegate('.btnSearchTask', 'click', function () {
        var searchValue = document.getElementById('filterValue').value;
        $.ajax({
            url: 'Tasks/Search',
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            data: { searchVal: searchValue },
            success: function (data) {
                Search();
            },
            error: function (data) {
                alert('Error in getting result');
            }
        });
    });

    if (!Modernizr.inputtypes.date) {
        $(function () {
            $(".datefield").datepicker();
        });
    }
});

function GetAll()
{
    $.ajax({
        url: 'Assigns/GetAll',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $('#divList').html(data);
        },
        error: function (data) {
            alert('Error in getting result');
        }
    });
}

function Search() {
    var searchValue = document.getElementById('filterValue').value;
    $.ajax({
        url: 'Tasks/Search',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        data: { searchVal: searchValue },
        success: function (data) {
            $('#divList').html(data);
        },
        error: function (data) {
            alert('Error in getting result');
        }
    });
}

function closeModal() {
    $('#myModal').modal('hide');
}