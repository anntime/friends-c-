var sm3 = require("sm.js").sm3;
var hash = new sm3();
$(document).ready(function () {
    $.CreateCorsLocalStorage(objPub.Cors);
    objPub.CheckSsoLogin(objPub.UserID).done(function (json) {
        var result = json.d;
        if (result == true) {
            //设置全局变量
            objPub.SetObjPub();
            window.location.href = "/biz/main.html";
        } else {
            if (localStorage.getItem("remember_social_code") != undefined) {
                if (localStorage.getItem("remember_social_code") == Enum.YesNo.Yes.toString()) {
                    $("#ckRememberMe").attr("checked", true);
                    if (localStorage.getItem("social_code") != undefined && localStorage.getItem("social_code") != null) {
                        $("#txtUserCode").val(localStorage.getItem("social_code").toString());
                    }
                }
                else {
                    $("#ckRememberMe").attr("checked", false);
                }
            }
            if (localStorage.getItem("rsa") != undefined && localStorage.getItem("rsa") != null
               && localStorage.getItem("social_code") != undefined && localStorage.getItem("social_code") != null) {
                $.PostCorsMessage(objPub.Cors, { MessageType: "PostInfo", rsa: localStorage.getItem("rsa"), social_code: localStorage.getItem("social_code") });
                var login_view = {
                    SocialCode: localStorage.getItem("social_code").toString(),
                    RSAPassword: localStorage.getItem("rsa").toString()
                };
                Login.Login(login_view);
            }
            $("#ckRememberMe").on("change", Login.RememberMeEvent);
            //点击系统登录
            $("#aLogin").on("click", Login.LoginEvent);
            $("#sctLoginForm").on("keypress", Login.LoginKeyPressEvent);

            //点击“找回密码”按钮
            $("#aGoForgetPw").on("click", Login.GoEvent);
            //点击返回的“直接登录”按钮
            $("#aBackLogin").on("click", Login.BackEvent);
            //找回密码事件
            $("#aFindPassword").on("click", Login.FindPasswordEvent);
            $("#sctPasswordForm").on("keypress", Login.FindPasswordKeyPressEvent);
            $("#txtUserCode").blur(function () {
                Login.IsNotNull($(this).attr("id"));
            }).focus(function () {
                $("#" + $(this).attr("id") + "Tip").hide();
            }).keyup(function () {
                if (this.value != "") {
                    $("#" + $(this).attr("id") + "Tip").hide();
                } else {
                    $(this).addClass("error-input");
                    $("#" + $(this).attr("id") + "Tip").show();
                }
            });

            $("#txtPassword").blur(function () {
                Login.IsNotNull($(this).attr("id"));
            }).focus(function () {
                $("#" + $(this).attr("id") + "Tip").hide();
            }).keyup(function () {
                if (this.value != "") {
                    $("#" + $(this).attr("id") + "Tip").hide();
                } else {
                    $(this).addClass("error-input");
                    $("#" + $(this).attr("id") + "Tip").show();
                }
            });

            $("#txtEmail").blur(function () {
                Login.IsNotNull($(this).attr("id"));
            }).focus(function () {
                $("#" + $(this).attr("id") + "Tip").hide();
            });
        }
    });
});
