$(document).ready(function () {
    GetAll();

    $('.btnCreate').on('click', function () {
        $.ajax({
            url: 'Projects/Create',
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

    $(document).delegate('.btnCreate', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('edit_', '').trim();
        $.ajax({
            url: 'Projects/Create',
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

    $(document).delegate('.btnEdit', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('edit_', '').trim();
        $.ajax({
            url: 'Projects/Edit',
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
            url: 'Projects/Details',
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
                    url: 'Projects/Delete',
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

    $(document).delegate('.btnaddTask', 'click', function () {
        var getId = $(this).attr('id');
        var studentId = getId.replace('addTask_', '').trim();
        $.ajax({
            url: 'Tasks/AddTask',
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

    $(document).delegate('.btnSearchProj', 'click', function () {
        var searchValue = document.getElementById('filterValue').value;
        $.ajax({
            url: 'Projects/Search',
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
        url: 'Projects/GetAll',
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
        url: 'Projects/Search',
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

