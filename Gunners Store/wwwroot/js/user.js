var dataTable;

$(document).ready(function () {
    console.log("Document ready, loading DataTable");
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        responsive: true,
        ajax: {
            url: '/admin/user/getall',
            type: 'GET',
            dataType: 'json',
            error: function (xhr, status, error) {
                console.error('DataTable AJAX error:', status, error);
            }
        },
        columns: [
            { data: 'name', width: '20%' },
            { data: 'email', width: '20%' },
            { data: 'role', width: '15%' },
            {
                data: 'id',
                width: '15%',
                render: function (data, type, row) {
                    console.log('Row data:', row);  
                    var isLocked = row.lockoutEnd && new Date(row.lockoutEnd) > new Date();
                    var btnColor = isLocked ? 'btn-success' : 'btn-danger';
                    var btnText = isLocked ? '<i class="bi bi-unlock"></i> Unlock' : '<i class="bi bi-lock"></i> Lock';

                    return `
                        <div class="text-center">
                            <button onclick="toggleLock('${data}')" 
                                    class="btn ${btnColor}" 
                                    style="width:120px;">
                                ${btnText}
                            </button>
                        </div>`;
                }
            }
        ]
    });
}

function toggleLock(id) {
    $.ajax({
        type: 'POST',
        url: '/admin/user/togglelock',
        data: JSON.stringify(id),
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                toastr.success('User status updated successfully!');
                dataTable.ajax.reload(null, false);
            } else {
                toastr.error('Failed to update user status: ' + (data.message || 'Unknown error'));
            }
        },
        error: function (xhr, status, error) {
            toastr.error('Error: ' + status + ' - ' + error);
            console.error('ToggleLock AJAX error:', status, error);
        }
    });
}