var DataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
            }
        }
    }
});

function loadDataTable(status) {
    DataTable = $('#tblData').DataTable({
        responsive: true,
        "ajax": {
            url: '/admin/order/getall?status=' + status,
            type: 'GET'
        },
        "columns": [
            { data: 'id', "width": "10%" },
            { data: 'name', "width": "11%" },
            { data: 'applicationUser.email', "width": "12%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderDate', "width": "13%" },
            {
                data: 'id', 
                render: function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2">
                        <i class="bi bi-pencil-square"></i> Edit</a>
                    </div>`;
                },
                "width": "13%"
            },
        ]
    });
}


