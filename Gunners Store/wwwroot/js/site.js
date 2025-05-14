
var dataTable;

$(document).ready(function () {
    console.log("Document ready, loading DataTable");
    loadDataTable();
});

function loadDataTable() {

    dataTable = $('#tblData').DataTable({
        responsive: true,
        ajax: {
            url: '/admin/category/getall',
            type: 'GET',
            dataType: 'json',
            error: function (xhr, status, error) {
                console.error('DataTable AJAX error:', status, error);
            }
        },
        columns: [
            { data: 'name', width: '40%' },
            { data: 'displayOrder', width: '30%' },
            {
                data: 'id',
                width: '30%',
                render: function (data, type, row) {
                    return `
                        <div class="btn-group" role="group">
                            <a href="/admin/category/edit/${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a href="/admin/category/delete/${data}" class="btn btn-danger mx-2">
                                <i class="bi bi-trash"></i> Delete
                            </a>
                        </div>`;
                }
            }
        ]
    });
}