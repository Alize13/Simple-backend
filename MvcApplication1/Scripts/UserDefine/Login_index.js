
$(function () {
    pageLoad();
});

function pageLoad() {
    $('.preloader').fadeOut();
    // ---------------------------
    // Login and Recover Password
    // ---------------------------
    $('#to-recover').on('click', function () {
        $('#loginform').hide();
        $('#recoverform').fadeIn();
    });

    $('#to-login').on('click', function () {
        $('#loginform').fadeIn();
        $('#recoverform').hide();
    });

    $('#to-register').on('click', function () {
        $('#loginform').hide();
        $('#registerform').fadeIn();
    });

    $('#to-login2').on('click', function () {
        $('#loginform').fadeIn();
        $('#registerform').hide();
    });
}


function login() {

    console.log("login");
    $.ajax({
        url: '/Login/Login/',           // url位置
        type: 'Post',                   // post/get
        dataType: 'json',
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            Account: $('#txt_account').val(),
            Password: $('#txt_password').val()
        },      // 錯誤後執行的函數
        success: function (response) {
            if (response == '登入成功') {
                location.href = '../Operations';
            }
            showAlert(response);
        }
    });
}

