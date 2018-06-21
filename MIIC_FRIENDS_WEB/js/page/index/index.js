Login = function () { }
Login.registerClass("Login");
Login.Login = function login(login_view) {
    $.SimpleAjaxPost("service/UserService.asmx/GetSM2EncryptPassword",
             true,"{password:'"+login_view.RSAPassword+"'}").done(function (json) {
                 login_view.RSAPassword = json.d;
                  $.SimpleAjaxPost("service/UserService.asmx/Login",
                         true,
                        "{loginView:" + $.Serialize(login_view) + "}",
                         function (json) {
                             var result = json.d;
                             if (result.IsLogin == true) {

                                 if (localStorage.getItem("social_code") == undefined
                                     || ($("#txtUserCode").val() != "" && localStorage.getItem("social_code") != undefined)) {
                                     localStorage.setItem("social_code", $("#txtUserCode").val());
                                 }
                                 if (localStorage.getItem("rsa") == undefined) {
                                     localStorage.setItem("rsa",$.Hex2String(objPub.Sm3Hash.sum($("#txtPassword").val())));
                                 }
                                 //设置全局变量
                                 objPub.SetObjPub();
                                 objPub.SsoLoginIn(objPub.UserID).done(function (json) {
                                     var result = json.d;
                                     if (result == true) {
                                         window.location.href = "/biz/main.html";
                                     } else {
                                         console.log("单点登录异常");
                                     }
                                 });
                             }
                             else {
                                 if (result.CheckAdmin == false) {
                                     $("#txtUserCode").addClass("error-input");
                                     $("#txtUserCodeTip").html("非管理人员无法登录本系统").show();
                                 }
                                 else if (result.CheckUserCode == false) {
                                     $("#txtUserCode").addClass("error-input");
                                     $("#txtUserCodeTip").html("账号错误,请重新输入").show();
                                 }
                                 else if (result.CheckPassword == false) {
                                     $("#txtPassword").addClass("error-input");
                                     $("#txtPasswordTip").html("密码错误,请重新输入").show();
                                 } else if (result.CheckValid == false) {
                                     $("#txtUserCode,#txtPassword").addClass("error-input");
                                     $("#txtUserCodeTip").html("用户失效,请联系管理员").show();
                                 }
                             }
                         });
              });
}

Login.LoginEvent = function LoginEvent(event) {
    if ($("#txtUserCode").val() != ""
       && $("#txtPassword").val() != "") {

        var login_view = {
            SocialCode: $("#txtUserCode").val(),
            RSAPassword: $.Hex2String(objPub.Sm3Hash.sum($("#txtPassword").val()))
        };
        Login.Login(login_view);
    } else {
        if ($("#txtUserCode").val() == ""
       && $("#txtPassword").val() == "") {
            Login.IsNotNull("txtUserCode");
            Login.IsNotNull("txtPassword");
        } else if ($("#txtUserCode").val() == "") {
            Login.IsNotNull("txtUserCode");
        } else if ($("#txtPassword").val() == "") {
            Login.IsNotNull("txtPassword");
        }
    }
    return false;
}
Login.LoginKeyPressEvent = function LoginKeyPressEvent(event) {
    if (event.which == 13) {
        if ($("#txtUserCode").val() != ""
             && $("#txtPassword").val() != "") {
            var login_view = {
                SocialCode: $("#txtUserCode").val(),
                RSAPassword:$.Hex2String(objPub.Sm3Hash.sum($("#txtPassword").val()))
            };
            Login.Login(login_view);
        } else {
            if ($("#txtUserCode").val() == "" && $("#txtPassword").val() == "") {
                Login.IsNotNull("txtUserCode");
                Login.IsNotNull("txtPassword");
            } else if ($("#txtUserCode").val() == "") {
                Login.IsNotNull("txtUserCode");
            } else if ($("#txtPassword").val() == "") {
                Login.IsNotNull("txtPassword");
            }
        }
    }
}
Login.RememberMeEvent = function RememberMeEvent(event) {
    if ($("#ckRememberMe").is(":checked") == true) {
        $("#ckRememberMe").attr("checked", true);
        localStorage.setItem("remember_social_code", Enum.YesNo.Yes.toString());
    }
    else {
        $("#ckRememberMe").attr("checked", false);
        if (localStorage.getItem("social_code") != undefined) {
            localStorage.removeItem("social_code");
        }
        if (localStorage.getItem("rsa") != undefined) {
            localStorage.removeItem("rsa");
        }
        localStorage.setItem("remember_social_code", Enum.YesNo.No.toString());
    }
}
Login.FindPassword = function find_password() {
    if (Login.IsNotNull("txtEmail")) {
        $.SimpleAjaxPost("service/UserService.asmx/FindPasswordByEmail",
              true,
             "{myEmail:'" + $("#txtEmail").val() + "'}",
              function (json) {
                  if (json.d == true) {
                      $.Alert({ width: "auto", content: "您的密码已找回，请登录邮箱进行查询！" }, function (event) {
                          $("#txtEmailTip").html("");
                          $("#txtUserCode,#txtPassword").removeClass("error-input");
                          $("#sctLoginForm").show();
                          $("#sctPasswordForm").hide();
                      });
                  }
                  else {
                      $("#txtEmail").addClass("error-input").val("");
                      $("#txtEmailTip").html("找回失败，请检查邮箱").show();
                  }
              });
    }
}
Login.GoEvent = function GoEvent(event) {
    $("#sctLoginForm").hide();
    $("#sctPasswordForm").show();
}

Login.BackEvent = function BackEvent(event) {
    $("#sctLoginForm").show();
    $("#sctPasswordForm").hide();
}
Login.FindPasswordEvent = function FindPasswordEvent(event) {
    Login.FindPassword();
}

Login.FindPasswordKeyPressEvent = function FindPasswordKeyPressEvent(event) {
    if (event.which == 13) {
        Login.FindPassword();
    }
}
Login.IsNotNull = function is_not_null(id) {
    var result = false;
    if ($("#" + id).val().length != 0) {
        switch (id) {
            case "txtUserCode":
                $("#" + id).attr("placeholder", "用户名/已注册手机");
                break;
            case "txtPassword":
                $("#" + id).attr("placeholder", "密码");
                break;
            case "txtEmail":
                $("#" + id).attr("placeholder", "邮箱地址");
                break;
            default:
                tip = "";
                break;
        }

        if (id == "txtEmail") {
            if (!$("#" + id).val().match(/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/)) {
                $("#" + id + "Tip").html("邮箱格式不正确!")
                           .show();
                $("#" + id).addClass("error-input");
            } else {
                $("#" + id + "Tip").hide();
                $("#" + id).removeClass("error-input");
                result = true;
            }
        }
        else {
            $("#" + id + "Tip").hide();
            $("#" + id).removeClass("error-input");
            result = true;
        }
    }
    else {
        var tip = "";
        switch (id) {
            case "txtUserCode":
                tip = "请输入用户名/已注册手机";
                break;
            case "txtPassword":
                tip = "请输入密码";
                break;
            case "txtEmail":
                tip = "请输入邮箱";
                break;
            default:
                tip = "";
                break;
        }
        $("#" + id + "Tip").html(tip).show();
        $("#" + id).addClass("error-input");
        $("#" + id).removeAttr("placeholder");
    }
    return result;
}