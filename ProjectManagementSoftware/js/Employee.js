$(document).ready(function () {
    GetAll();   
          
    $('.btnCreate').on('click', function () {       
        $.ajax({
            url: 'Employees/Create',
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
            url: 'Employees/Create',
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
            url: 'Employees/Edit',
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
                    url: 'Employees/Delete',
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

    $(document).delegate('.btnSearch', 'click', function () {
        var searchValue = document.getElementById('filterValue').value;
        $.ajax({
            url: 'Employees/Search',
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
        url: 'Employees/GetAll',
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
        url: 'Employees/Search',
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



