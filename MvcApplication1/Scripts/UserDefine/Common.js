
var keepUserReadDateNow = 1000;         //讀取時間
var StopReadDateNow;

var LastClickTime = null;                               //紀錄最後點擊時間

//以下Produce_JqGrid使用
var filters = {
    groupOp: "AND",
    rules: []
};
var isSearch = false;


$(function () {
    StopReadDateNow = setInterval("NowDateTime();", keepUserReadDateNow);          //讀取時間    
});


function NowDateTime() {
    //時鐘
    var today = new Date();
    var year = today.getFullYear() + " / " + (today.getMonth() + 1) + " / " + today.getDate();
    var h = today.getHours();
    var m = today.getMinutes();
    var s = today.getSeconds();
    h = (parseInt(h) < 10) ? "0" + h.toString() : h;
    m = (parseInt(m) < 10) ? "0" + m.toString() : m;
    s = (parseInt(s) < 10) ? "0" + s.toString() : s;
    $("#nowDateTime").html(year + " " + h + ":" + m + ":" + s);
}

//alert提示
function showAlert(message, type) {

    var $cont = $("#alerts-container");

    if ($cont.length == 0) {
        // alerts-container does not exist, create it
        $cont = $('<div id="alerts-container">')
            .css({
                position: "fixed"
                , width: "50%"
                , left: "25%"
                , top: "10%"
            })
            .appendTo($("body"));
    }

    // default to alert-info; other options include success, warning, danger
    type = type || "info";

    // create the alert div
   
    var alert = $('<div>')
        .addClass("fade in show alert alert-dismissible alert-" + type + " text-" + type)
        .append('<div class="d-flex align-items-center font-medium me-3 me-md-0"><i class="mdi mdi-information-outline front-24"></i>　' + message +'</div>');

    // add the alert div to top of alerts-container, use append() to add to bottom
    $cont.prepend(alert);

    //set a timeout to close the alert
    window.setTimeout(function () { alert.alert("close") }, 5000);
}


//是否連續點擊
function DoubleClick() {
    var NowTime = new Date();                           //現在時間
    if (LastClickTime != null) {
        TimeD = (NowTime - LastClickTime);              //紀錄誤差毫秒 
        if (0 < TimeD && TimeD < 800) {                 //小於800毫秒 代表重復點擊
            alertify.error("已點擊觸發！稍後再試" + "...");
            return true;
        }
        else {
            LastClickTime = NowTime;
            return false;
        }
    }
    LastClickTime = NowTime;
    return false;
}

//檢查是否有特殊符號
function ChangeSpecial(e) {
    var SpecialStr = new RegExp("[\\`\\~\\@\\#\\$\\%\\^\\&\\*\\)\\(\\_\\+\\-\\[\\]\\{\\}\\:\\'\\\"\\;\\/\\<\\>\\\\\|]", "g");
    e.val(e.val().replace(SpecialStr, ""))
    return false;
}

//不能輸入特殊符號
function SpecialKeyPress() {
    var code;
    var character;
    var SpecialStr = new RegExp("[\\`\\~\\@\\#\\$\\%\\^\\&\\*\\)\\(\\_\\+\\-\\[\\]\\{\\}\\:\\'\\\"\\;\\/\\<\\>\\\\\|]");
    if (document.all) //判断是否是IE浏览器
    {
        code = window.event.keyCode;
    }
    else {
        code = arguments.callee.caller.arguments[0].which;
    }
    var character = String.fromCharCode(code);
    //特殊字符正则表达式
    if (SpecialStr.test(character)) {
        if (document.all) {
            window.event.returnValue = false;
        }
        else {
            arguments.callee.caller.arguments[0].preventDefault();
        }
    }
}

//字串Format
function String_Format(arguments) {
    var s = arguments[0];
    if (s == null) return "";
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = getStringFormatPlaceHolderRegEx(i);
        s = s.replace(reg, (arguments[i + 1] == null ? "" : arguments[i + 1]));
    }
    return cleanStringFormatResult(s);

}

//讓輸入的字串可以包含{}
function getStringFormatPlaceHolderRegEx(placeHolderIndex) {
    return new RegExp('({)?\\{' + placeHolderIndex + '\\}(?!})', 'gm')
}

//清理字串各式結果
function cleanStringFormatResult(txt) {
    if (txt == null) return "";
    return txt.replace(getStringFormatPlaceHolderRegEx("\\d+"), "");
}

//JqGrid 產生表格統一
function Produce_JqGrid(id, url, userdata, rowNum, colNames, colModel, caption, footerrow, pageload) {
    //var filters = {
    //    groupOp: "AND",
    //    rules: []
    //};//定義在最上方
    if (pageload == undefined) { pageload = false; }
    start_time = new Date();
    jQuery("#" + id).jqGrid({
        url: url,
        datatype: 'json',
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            id: "Lst_Index",
            repeatitems: false,
            userdata: userdata
        },
        mtype: 'GET',
        autowidth: false,
        shrinkToFit: true,
        height: '100%',
        viewrecords: true,
        hidegrid: false,        //啟用用或者禁用控制表格顯示、隐藏的按钮
        cmTemplate: { sortable: false },
        rowNum: rowNum,
        colNames: colNames,
        colModel: colModel,
        caption: caption,
        footerrow: footerrow,
        search: isSearch, //定義在最上方
        postData: { "filters": JSON.stringify(filters) },
        loadComplete: function () { RComplete(); },
        gridComplete: function () {            
            if (pageload == true) {
                jqGridExecTime(start_time); //jqGrid執行秒數
            }
        }
    });
}

//JQGrid 頁面搜尋
function JQGrid_PageList(Num) {
    var grid = $("#jqList");

    $("#Page_Up_Records").text(grid.getGridParam('records'));       //上頁顯示資料總筆數
    $("#Page_Up_Lastpage").text(grid.getGridParam('lastpage'));     //上頁顯示資料總頁數
    $("#Page_Down_Records").text(grid.getGridParam('records'));     //下頁顯示資料總筆數
    $("#Page_Down_Lastpage").text(grid.getGridParam('lastpage'));   //下頁顯示資料總頁數
    $("#Hidd_Records").val(grid.getGridParam('records'));           //存放資料總筆數
    $("#Hidd_Lastpage").val(grid.getGridParam('lastpage'));         //存放總頁數
    $("#Hidd_Temp_RowNum").val(grid.getGridParam('rowNum'));        //存放每頁顯示幾筆資料
    $("#Hidd_Temp_Page").val(grid.getGridParam('page'));            //存放後端取得目前所在位置頁數
    var temp_page = $("#Hidd_Temp_Page").val();                     //第幾頁

    //每頁顯示筆數
    Page_Select_Page($("#Hidd_Temp_Page").val(), $("#Hidd_Lastpage").val(), "Page_Up_Page");
    Page_Select_Page($("#Hidd_Temp_Page").val(), $("#Hidd_Lastpage").val(), "Page_Down_Page");

    //初始顯示幾筆
    Page_Select_RowNum(Num, $("#Hidd_Temp_RowNum").val(), "Page_Up_RowNum");
    Page_Select_RowNum(Num, $("#Hidd_Temp_RowNum").val(), "Page_Down_RowNum");
}

//DataTables 產生表格統一
function Produce_DataTables(id, url, rowNum, colNames, pageload) {

    //1、生成動態列

    //dataTables的json返回的列名
    var columnList = [];      //顯示列對應的json字段

    for (var i = 0; i < colNames.length; i++) {
        var obj = {};
        obj['data'] = colNames[i];
        columnList.push(obj);
    }


    //2、設置dataTables配置項

    var settings = {
        "processing": true, // for show progress bar
        "serverSide": true, // //開啟服務端請求模式
        "ajax": {
            "url": url,
            "type": "POST",
            "datatype": "json",
            data: function (d) {
                d.__RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
                d.filters = JSON.stringify(GetFilters());
            }, "complete": function (json) {
                console.info(json);
                //$("#Hidd_Records").val(json.recordsTotal);
            }, dataSrc: function (d) {
                newRowData = d.data;
                return d.data;
            }
        },
        "retrieve": true,
        "columns": columnList
    };



    //3、dataTables創建並且生成（這麼寫的原因：每次動態列字符串改變，dataTables也會跟着改變）

    //這樣寫就能把datatable完全重新載入
    $('#' + id).DataTable().clear();
    $('#' + id).DataTable().destroy();
    $('#' + id).DataTable(settings);



    if (pageload == undefined) { pageload = false; }
    start_time = new Date();
    var oDataTable;
    var oRowData = [];
    oDataTable = $("#" + id).dataTable({
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
        "preDrawCallback": function (settings) {    //ajax之前的判斷
            preDrawCallback(settings); //各頁面撰寫做設定
        },
        "ajax": {
            "url": url,
            "type": "POST",
            "datatype": "json",
            data: function (d) {
                d.__RequestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
                d.filters = JSON.stringify(GetFilters());
            }, "complete": function (json) {
                //console.info(json);
                //InitPayMentList(json);
            }, dataSrc: function (d) {
                //console.info("dataSrc");
                // TODO: Insert your code
                //InitPayMentList(d.data);
                newRowData = d.data;
                return d.data;
            }
        },
        "Columns": colNames,
        "createdRow": function (row, data, index) { //產生tr td時，可自己調整的部份
            FomatRow(row, data, index);
        },
        "drawCallback": function (settings, json) {
            //console.info("initComplete" + newRowData);
            //InitPayMentList(newRowData);
        }
    });

    //jQuery("#" + id).jqGrid({
    //    url: url,
    //    datatype: 'json',
    //    jsonReader: {
    //        root: "rows",
    //        page: "page",
    //        total: "total",
    //        records: "records",
    //        id: "Lst_Index",
    //        repeatitems: false,
    //        userdata: userdata
    //    },
    //    mtype: 'GET',
    //    autowidth: false,
    //    shrinkToFit: true,
    //    height: '100%',
    //    viewrecords: true,
    //    hidegrid: false,        //啟用用或者禁用控制表格顯示、隐藏的按钮
    //    cmTemplate: { sortable: false },
    //    rowNum: rowNum,
    //    colNames: colNames,
    //    colModel: colModel,
    //    caption: caption,
    //    footerrow: footerrow,
    //    search: isSearch, //定義在最上方
    //    postData: { "filters": JSON.stringify(filters) },
    //    loadComplete: function () { RComplete(); },
    //    gridComplete: function () {
    //        //使用於是否啟動 Excel
    //        if ($("#hid_ExportToExcel").val() == "true" && $("#hid_StepExcel").val() == "2") {
    //            $("#hid_ExportToExcel").val("false");
    //            $("#hid_StepExcel").val("0");
    //            var reUrl = location.href.replace(location.protocol + "//", "").replace(location.host, "").replace("#", "");
    //            window.location = "/Report/ReportDownLoadCreate/?Url=" + reUrl + "&ReportName=" + $("#hid_ExcelName").val();
    //        }
    //        if (pageload == true) {
    //            jqGridExecTime(start_time); //jqGrid執行秒數
    //        }
    //    }
    //});
}


//JQDataTables 頁面搜尋
function JQDataTables_PageList(Num) {
    var grid = $("#jqList");

    $("#Page_Up_Records").text(grid.getGridParam('records'));       //上頁顯示資料總筆數
    $("#Page_Up_Lastpage").text(grid.getGridParam('lastpage'));     //上頁顯示資料總頁數
    $("#Page_Down_Records").text(grid.getGridParam('records'));     //下頁顯示資料總筆數
    $("#Page_Down_Lastpage").text(grid.getGridParam('lastpage'));   //下頁顯示資料總頁數
    $("#Hidd_Records").val(grid.getGridParam('records'));           //存放資料總筆數
    $("#Hidd_Lastpage").val(grid.getGridParam('lastpage'));         //存放總頁數
    $("#Hidd_Temp_RowNum").val(grid.getGridParam('rowNum'));        //存放每頁顯示幾筆資料
    $("#Hidd_Temp_Page").val(grid.getGridParam('page'));            //存放後端取得目前所在位置頁數
    var temp_page = $("#Hidd_Temp_Page").val();                     //第幾頁

    //每頁顯示筆數
    Page_Select_Page($("#Hidd_Temp_Page").val(), $("#Hidd_Lastpage").val(), "Page_Up_Page");
    Page_Select_Page($("#Hidd_Temp_Page").val(), $("#Hidd_Lastpage").val(), "Page_Down_Page");

    //初始顯示幾筆
    Page_Select_RowNum(Num, $("#Hidd_Temp_RowNum").val(), "Page_Up_RowNum");
    Page_Select_RowNum(Num, $("#Hidd_Temp_RowNum").val(), "Page_Down_RowNum");
}

//判斷上下頁
function Page_Up_Dow(Up_Dow) {
    var All_Page = parseInt($("#Hidd_Lastpage").val());
    var Page = parseInt($("#Page_Up_Page").val());
    if (Up_Dow == 1) {
        if (All_Page != Page) {
            Page = Page + 1;
            $("#Hidd_Temp_Page").val(Page);
            dataChange();
        } else {

            return false;
        }
    } else {
        if (Page != 1) {
            Page = Page - 1;
            $("#Hidd_Temp_Page").val(Page);
            dataChange();
        } else {

            return false;
        }
    }
}


//每頁顯示
function Page_RowNum(record) {
    $("Page_Down_Page")
    $("#Hidd_Temp_RowNum").val(record);
    $("#Hidd_Temp_Page").val(1);
    dataChange();
}


//轉到幾頁
function Page_Turn(page) {
    $("#Hidd_Temp_Page").val(page);
    dataChange();
}

//分頁顯示頁數
function Page_Select_Page(Page, Lastpage, id) {
    $("#" + id).text("");
    for (var i = 1; i <= Lastpage; i++) {
        if (i == Page) {
            $("#Hidd_Temp_Page").val(i);
            $("#" + id).append('<option id=option_' + i + ' selected>' + i + '</option>')
            continue;
        }
        $("#" + id).append('<option id=option_' + i + '>' + i + '</option>');
    }
}


//分頁顯示筆數
function Page_Select_RowNum(Num, RowNum, id) {
    $("#" + id).text("");
    for (var i = 0; i < Num.length; i++) {
        if (RowNum == Num[i]) {
            $("#" + id).append('<option  selected>' + Num[i] + '</option>');
            continue;
        }
        $("#" + id).append('<option>' + Num[i] + '</option>');
    }
}

