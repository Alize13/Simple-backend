
var newRowData = [];
var oTable;

$(function () {
    $('input[name="datetimes"]').daterangepicker({
        timePicker: true,
        timePicker24Hour: true,
        startDate: moment().startOf('hour').add(32, 'hour'),
        endDate: moment().startOf('hour'),
        locale: {
            format: 'YYYY/MM/DD hh:mm'
        }
    });

    //***********************************//
    // Tagging support
    //***********************************//
    $("#selectIntertestID").select2({
        tags: true
    });

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
            "url": '/Member/List_Load/',
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
                { "data": "MemberAccount", "name": "MemberAccount", "autoWidth": false, className: "text-center", "title": "帳號" },
                { "data": "MemberName", "name": "MemberName", "autoWidth": true, className: "text-center", "title": "暱稱" },,
                { "data": "MemberStatus", "name": "MemberStatus", "autoWidth": true, className: "text-center", "title": "帳號狀態" },
                { "data": "MemberEmail", "name": "MemberCount", "autoWidth": true, className: "text-center", "title": "E-mail" },
                { "data": "MemberLevel_Str", "name": "MemberLevel_Str", "autoWidth": true, className: "text-center", "title": "會員等級" },
                { "data": "MemberSalaly", "name": "MemberSalaly", "autoWidth": true, className: "text-right", "title": "會員收入" },
                { "data": "MemberInterests_Str", "name": "MemberInterests_Str", "autoWidth": true, className: "text-center", "title": "會員興趣" },
                { "data": "CreateUser", "name": "CreateUser", "autoWidth": true, className: "text-center", "title": "建立者" },
                { "data": "CreateDateTime", "name": "CreateDateTime", "autoWidth": true, className: "text-center", "title": "建立時間" },
                { "data": "MemberID", "name": "MemberID", "autoWidth": true, className: "text-center", "title": "操作" }
            ],
        "columnDefs": [ //RWD Priority 越高，越先隱藏
            { responsivePriority: 1, targets: 0 },
            { responsivePriority: 3, targets: 1 },
            { responsivePriority: 2, targets: 2 },
            { responsivePriority: 9, targets: 3 },
            { responsivePriority: 4, targets: 4 },
            { responsivePriority: 6, targets: 5 },
            { responsivePriority: 7, targets: 6 },
            { responsivePriority: 5, targets: 7 },
            { responsivePriority: 8, targets: 8 },
            { responsivePriority: 0, targets: 9 }
        ],
        "createdRow": function (row, data, index) { //產生tr td時，可自己調整的部份
            //var Status_Format_Str = '<a  name="Enable" title="狀態" href="javascript:void(0)" onclick="StatusSwitch(\'{0}\')">{1}</a>';
            //temp_string = String_Format(new Array(Status_Format_Str, data.MemberID, data.MemberStatus));
            //$('td', row).eq(2).html(temp_string);

            var td_str = '<div class="button-group"><input type="button" value="修改" name="EditID" class="btn btn-sm btn-info"  data-toggle="modal" data-target="#mdEdit" onclick="EditClick(' + data.LevelID + ')" />' +
                '<input type="button" name="DelID" value="刪除" style="cursor:not-allowed;" disabled="disabled" class="btn btn-sm btn-secondary" onclick="btnDeleteClick(' + data.LevelID + ')" /></div>';

            $('td', row).last().html(td_str);
        },
        "drawCallback": function (settings, json) {

            $('#Hidd_Records').val(this.api().data().count());
        }
    });
}

//新增修改
function EditClick(Key) {

    if (parseInt($('#Hidd_Records').val()) > 9 && Key == "-1") {
        showAlert('已達上限', 'danger')
        $('#mdEdit').modal('toggle');
        return false;
    }

    var token = $('input[name="__RequestVerificationToken"]').val();

    $('#ModifyTitle').text(Key == "-1" ? "新增 - 會員資料" : "修改 - 會員資料");
    $('#model_state').text(Key == "-1" ? 'add' : 'edit');
    $('#btnDelete').css('display', 'none');

    if (Key == "-1") {
        var Cnt = parseInt($('#Hidd_Records').val()) + 1;
        $('#LevelID').val("0");
        $('#MemberLevel').val(Cnt);
        $('#LevelName').val("VIP" + Cnt);
    }
    else {
        $('#EditContent').html('載入中...');
        $.ajax({
            url: '/Member/MemberAdd',
            data: {
                __RequestVerificationToken: token,
                key: Key
            },
            success: function (result) {
                $('#EditContent').html(result);
                if ($('#MemberLevel').val() == $('#Hidd_Records').val()) {
                    $('#btnDelete').css('display', 'block');
                }
            }
        });
    }
}

//刪除
function btnDeleteClick(Key) {
    if (confirm('是否要刪除?')) {
        var token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/System/LevelSettingDelete/',    // url位置
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


    if ($("#MemberLevel").val() < 0) {
        showAlert('請輸入正數', 'danger')
        return false;
    }

    if ($("#MemberLevel").val() > 10) {
        showAlert('已達上限', 'danger')
        return false;
    }

    ///等級名稱
    if ($("#LevelName").val() == "") {
        showAlert('請輸入等級名稱', 'danger')
        return false;
    }


    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/System/LevelSettingAdd',
        data: {
            __RequestVerificationToken: token,
            LevelID: $("#LevelID").val(),
            MemberLevel: $("#MemberLevel").val(),
            LevelName: $("#LevelName").val()
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

    //連結至會員列表
    function MemberLink(Level) {
        window.parent.location.href = "../Member/List?lv=" + Level;
    }
}