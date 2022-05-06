
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
            "url": '/System/LevelSetting_Load/',
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
                { "data": "MemberLevel", "name": "MemberLevel", "autoWidth": false, className: "text-center", "title": "會員等級" },
                { "data": "LevelName", "name": "LevelName", "autoWidth": true, className: "text-center", "title": "等級名稱" },
                { "data": "MemberCount", "name": "MemberCount", "autoWidth": true, className: "text-right", "title": "會員數" },
                { "data": "LevelID", "name": "LevelID", "autoWidth": true, className: "text-center", "title": "操作" }
            ],
        "createdRow": function (row, data, index) { //產生tr td時，可自己調整的部份
            var MemberLink_Format_Str = '<a  name="MemCnt" title="會員數" href="javascript:void(0)" onclick="MemberLink(\'{0}\')">{1}</a>';
            temp_string = String_Format(new Array(MemberLink_Format_Str, data.MemberLevel, data.MemberCount));
            $('td', row).eq(2).html(temp_string);

            var td_str = '<div class="button-group"><input type="button" value="修改" name="EditID" class="btn btn-sm btn-info"  data-toggle="modal" data-target="#mdEdit" onclick="EditClick(' + data.LevelID + ')" />' +
                '<input type="button" name="DelID" value="刪除" style="cursor:not-allowed;" disabled="disabled" class="btn btn-sm btn-secondary" onclick="btnDeleteClick(' + data.LevelID + ')" /></div>';

            $('td', row).last().html(td_str);
        },
        "drawCallback": function (settings, json) {
            if ($("a[name='MemCnt']:last").text() == '0') {
                $("input[name='DelID']:last").css('cursor', 'pointer');
                $("input[name='DelID']:last").attr('disabled', false);
                $("input[name='DelID']:last").removeClass("btn-secondary").addClass("btn-danger");
            }

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

    $('#ModifyTitle').text(Key == "-1" ? "新增 - 會員等級設定" : "修改 - 會員等級設定");
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
            url: '/System/LevelSettingAdd',
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