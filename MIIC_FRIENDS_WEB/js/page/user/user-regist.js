User.Register = function () { }
User.Register.registerClass("User.Register");
User.Register.UserUrl = "";
User.Register.Init = function init() {
    //民族绑定
    User.Register.NationBinding();
    //注册类型绑定
    User.Register.RegisterTypeBinding();
    $("#tbPersonUser").show()
         .html5Validate(User.Register.RegistEvent, {
             submitID: "aRegister",
             formContainer: "table"
         });
    $("#txtSocialCode").off("blur").on("blur", User.Register.UniqueEvent);
    $("#txtEmail").off("blur").on("blur", User.Register.UniqueEmailEvent);
    $("#txtMobile").off("blur").on("blur", User.Register.UniqueMobileEvent);
    $("#fileUser").on("change", User.Register.UploadEvent);
    //上传头像
    $("#divUserAvatar").on("mouseenter", function (event) {
        $(".btnUploadPhoto").show();
    }).on("mouseleave", function (event) {
        $(".btnUploadPhoto").hide();
    });
    $("#txtOrgName").on("blur", function (event) {
        if ($("#txtRemark").val() == "") {
            $("#txtRemark").val($("#txtOrgName").val());
        }
    });
}
//上传头像事件
User.Register.UploadEvent = function UploadEvent(event) {
    if ($(this).val() != "") {
        $("#fmUser").ajaxSubmit({
            url: objPub.FriendsUrl + "service/UserPhotoCrossDomainUploadService.ashx",
            type: "post",
            dataType: "json",
            timeout: 600000,
            success: function (data, textStatus) {
                if (data.result == true) {
                    User.Register.UserUrl = data.acc.FilePath;
                    $("#imgUserPhoto").attr("src", data.acc.TempPath);
                } else {
                    $.Alert({
                        content: data.message,
                        width: "auto"
                    });
                }
            },
            error: function (data, status, e) {
                console.log("上传失败，错误信息：" + e);
            }
        });
    }
}

//用户名验证
User.Register.UniqueEvent = function UniqueEvent(event) {
    if ($(this).val() != "") {
        var _this = this;
        $.SimpleAjaxPost("service/UserService.asmx/UniqueSocialCode",
            true,
            "{socialCode:'" + $(this).val() + "'}",
            function (json) {
                var result = json.d;
                if (result != true) {
                    $.Alert({ content: "用户名已被注册，请重新输入!", width: "auto" }, function () {
                        $(_this).val("").focus();
                    });
                }
            });
    }
}

//邮箱验证
User.Register.UniqueEmailEvent = function UniqueEmailEvent(event) {
    if ($(this).val() != "") {
        var _this = this;
        $.SimpleAjaxPost("service/UserService.asmx/UniqueEmail",
            true,
            "{email:'" + $(this).val() + "'}",
            function (json) {
                var result = json.d;
                if (result != true) {
                    $.Alert({ content: "邮箱已被注册，请重新输入!", width: "auto" }, function () {
                        $(_this).val("").focus();
                    });
                }
            });
    }
}

//手机验证
User.Register.UniqueMobileEvent = function UniqueMobileEvent(event) {
    if ($(this).val() != "") {
        var _this = this;
        $.SimpleAjaxPost("service/UserService.asmx/UniqueMobile",
            true,
            "{mobile:'" + $(this).val() + "'}",
            function (json) {
                var result = json.d;
                if (result != true) {
                    $.Alert({ content: "手机已被注册，请重新输入!", width: "auto" }, function () {
                        $(_this).val("").focus();
                    });
                }
            });
    }
}

User.Register.RegistEvent = function RegistEvent(event) {
    //读取
    var social_user_info = {
        ID: $.NewGuid(),
        SocialCode: $("#txtSocialCode").val(),
        Mobile: $("#txtMobile").val(),
        Email: $("#txtEmail").val(),
        UserType: $("#sltUserType option:selected").val(),
        MicroUserUrl: User.Register.UserUrl == "" ? objPub.DefaultPhotoUrl : User.Register.UserUrl,
        Remark: $("#txtSocialRemark").val(),
        MobileBind: Enum.YesNo.Yes.toString()
    };
    var user_info ={
        UserID: social_user_info.ID,
        UserName: $("#txtNick").val(),
        RealName: $("#txtName").val(),
        Sex: $("input[name='rdUserSex']:checked").val(),
        Nation: $("#sltNation").val(),
        Mobile: $("#txtMobile").val(),
        Email: $("#txtEmail").val(),
        OrgName: $("#txtOrgName").val(),
        Remark: $("#txtRemark").val(),
        CanSeeQQ: Enum.YesNo.Yes.toString(),
        CanSeeMobile: Enum.YesNo.Yes.toString(),
        CanSeeTel: Enum.YesNo.Yes.toString()
    };
    var url = new Array();
    url.push(objPub.ManageUrl);
    url.push("service/UserService.asmx/Submit");
    $.SimpleAjaxCors(url, "POST", "{miicSocialUserInfo:" + $.Serialize(social_user_info) + ",userInfo:" + $.Serialize(user_info) + "}").done(function (json) {
        var result = json.d;
        if (result == true) {
            $.Alert({
                content: "恭喜您，注册用户成功,请耐心等待管理员为您激活~~",
                width: "auto"
            }, function () {
                window.location.href = objPub.FriendsUrl+"index.html";
            });
        }
    });

}


//民族绑定
User.Register.NationBinding = function nation_binding() {
    objPub.GetNationList().done(function (json) {
        var result = json.d;
        var temp = "<option  value=''>--- 请选择 ---</option>";
        if ($.isArray(result) && result.length != 0) {
            $.each(result, function (index, item) {
                temp += "<option value='" + item.Name + "'>" + item.Value + "</option>";
            });
        }
        $("#sltNation").empty().append(temp).val("1");
    });
}

//注册类型绑定
User.Register.RegisterTypeBinding = function register_type_binding() {
    $.SimpleAjaxPost("service/UserService.asmx/GetRegisterUserTypes",true,function (json) {
        var result = json.d;
        var temp = "";
        if ($.isArray(result) == true && result.length != 0) {
            $.each(result, function (index, item) {
                temp += "<option value='" + item.Name + "'>" + item.Value + "</option>";
            });
        }
        else {
            temp += "<option value=''>暂无数据</option>";
        }
        $("#sltUserType").empty().append(temp);
    });
}

User.Register.ReSet = function reset() {
    User.Register.UserUrl = "";
    $("#txtSocialCode,#txtNick,#txtName,#txtMobile,#txtEmail,#txtOrgName,#txtRemark").val("");
    $("input[name='rdUserSex'][value='0']").prop("checked", true);
    $("#sltNation").val("");
}