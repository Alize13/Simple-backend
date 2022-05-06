
var newRowData = [];
var oTable;

$(function () {
    pageLoad();
});

function pageLoad() {
    oTable = $('#MainTable').dataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // //開啟服務端請求模式
        "filter": false, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ordering": false,
        "paging": false,
        "responsive": false,
        "bPaginate": false,
        "destroy": true,
        "info": false, //去左下角
        "preDrawCallback": function (settings) { },//ajax之前的判斷
        "ajax": {
            "url": '/System/InterestSetting_Load/',
            "type": "POST",
            "datatype": "json",
            data: function (d) {
                d.__RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
            },
            "complete": function (json) {
                //console.log(json);
            },
            dataSrc: function (d) {
                newRowData = d.data;
                return d.data;
            }
        },
        "aoColumns":
            [
                { "data": "InterestID", "name": "Lst_Index", "autoWidth": false, className: "text-center", "title": "#" },
                { "data": "Interest", "name": "Interest", "autoWidth": true, className: "text-center", "title": "興趣" },
                { "data": "InterestID", "name": "InterestID", "autoWidth": true, className: "text-center", "title": "操作" }
            ],
        "createdRow": function (row, data, index) { //產生tr td時，可自己調整的部份
            $('td', row).first().html(index+1);
            var td_str = '<div class="button-group"><input type="button" value="修改" name="EditID" class="btn btn-sm btn-info"  data-toggle="modal" data-target="#mdEdit" onclick="EditClick(' + data.InterestID + ')" />' +
                '<input type="button" name="DelID" value="刪除" class="btn btn-sm btn-danger" onclick="btnDeleteClick(' + data.InterestID + ')" /></div>';

            $('td', row).last().html(td_str);
        },
        "drawCallback": function (settings, json) {
            //console.info("initComplete" + newRowData);
        }
    });
}
//新增修改
function EditClick(Key) {

    $('#ModifyTitle').text(Key == "-1" ? "新增 - 興趣設定" : "修改 - 興趣設定");
    $('#model_state').text(Key == "-1" ? 'add' : 'edit');

    var token = $('input[name="__RequestVerificationToken"]').val();

    if (Key == "-1") {
        $('#InterestID').val("0");
        $('#Interest').val("");
        $('#btnDelete').css('display', 'none');
    }
    else {
        $('#EditContent').html('載入中...');
        $.ajax({
            url: '/System/InterestSettingAdd',
            data: {
                __RequestVerificationToken: token,
                key: Key
            },
            success: function (result) {
                $('#EditContent').html(result);
                console.log(result);
            }
        });
    }
}

//刪除
function btnDeleteClick(Key) {
    if (confirm('是否要刪除?')) {
        var token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/System/InterestSettingDelete/',    // url位置
            type: 'Post',                   // post/get
            dataType: 'json',
            data: {
                __RequestVerificationToken: token,
                Key: Key
            },       // 輸入的資料
            success: function (response) {
                if (response == "0") {
                    showAlert('刪除成功！', 'success');
                    $(oTable).DataTable().draw();
                }
                else {
                    showAlert(response);
                }
            },
            error: function (xhr) { showAlert('刪除失敗！  ' + xhr, 'danger'); }
        });
    }
}


//新增修改按鈕
function btnAddSaveClick() {
    //不重新觸發
    if (DoubleClick()) { return false; }

   
    if ($("#Interest").val() == "") {
        showAlert('請輸入興趣名稱', 'warning');
        return false;
    }


    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/System/InterestSettingAdd',
        data: {
            __RequestVerificationToken: token,
            InterestID: $("#InterestID").val(),
            Interest: $("#Interest").val()
        },
        dataType: "json", 
        success: function (response) {
            if (response == "0") {
                showAlert('設定成功！', 'success');
                $('#mdEdit').modal('toggle');
                $(oTable).DataTable().draw();
            } else {
                showAlert(response);
            }
        },
        error: function (xhr) { showAlert('設定失敗！  ' + xhr, 'danger'); },
        type: 'POST'
    });
}