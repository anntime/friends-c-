User.Setting = function () { }
User.Setting.registerClass("User.Setting");
//读取用户ID
User.Setting.ID = objPub.UserID;
//个人用户读取头像
User.Setting.OldUserUrl = "";
//个人用户新设置头像
User.Setting.NewUserUrl = "";
//读取邮箱
User.Setting.ReadEmail = "";
//读取手机
User.Setting.ReadMobile = "";
User.Setting.GoBack = false;

//初始化数据
User.Setting.Init = function init() {
    //获取用户信息
    User.Setting.GoBack = false;
    if (User.Setting.ID == "") {
        $.Alert("读取用户信息出错");
        return;
    }
    User.Setting.ReadDetailUser();
    $("#txtEmail").off("blur").on("blur", User.Setting.UniqueEmailEvent);
    $("#txtMobile").off("blur").on("blur", User.Setting.UniqueMobileEvent);
    //绑定提交方法
    $("#tbPersonUser").html5Validate(User.Setting.SubmitUserEvent, {
        submitID: "aUpdate",
        formContainer: "table"
    });
    $(document).off("scroll");
    $(window).off("scroll").on("scroll", { WithTimeAxis: false}, window.objPub.ScorllEvent);
    //返回
    $("#aBack").on("click", User.Setting.BackEvent);
}

//返回事件
User.Setting.BackEvent = function BackEvent(event) {
    $.Confirm({width:"auto",content:"返回前您是否需要保存当前用户信息？"}, function () {
        User.Setting.GoBack = true;
        $("#aUpdate").trigger("click");
    }, function () {
        //初始化首页面
        objPub.InitLeftMain(true);
    });
}

//邮箱验证
User.Setting.UniqueEmailEvent = function UniqueEmailEvent(event) {
    var $target = $(event.target);
    var email = $target.val();
    if (email != "" && email != User.Setting.ReadEmail) {
        var url = new Array();
        url.push(objPub.ManageUrl);
        url.push("service/UserService.asmx/UniqueEmail");
        $.SimpleAjaxCors(url, "POST", "{email:'" + email + "'}").done(function (json) {
            var result = json.d;
            if (result != true) {
                $.Alert({width:"auto",content:"邮箱已被注册，请重新输入~"}, function () {
                    $target.val(User.Setting.ReadEmail).focus();
                });
            }
        });
    }
}

//手机验证
User.Setting.UniqueMobileEvent = function UniqueMobileEvent(event) {
    var $target = $(event.target);
    var mobile = $target.val();
    if (mobile != "" && mobile != User.Setting.ReadMobile) {
        var url = new Array();
        url.push(objPub.ManageUrl);
        url.push("service/UserService.asmx/UniqueMobile");
        $.SimpleAjaxCors(url, "POST", "{mobile:'" + $(this).val() + "'}").done(function (json) {
                var result = json.d;
                if (result != true) {
                    $.Alert("手机已被注册，请重新输入", function () {
                        $target.val(User.Setting.ReadMobile).focus();
                    });
                }
            });
    }
}


User.Setting.UploadUserEvent = function UploadUserEvent(event) {
    if ($(event.target).val() != "") {
        $("#fmUser").ajaxSubmit({
            url: objPub.FriendsUrl + "service/UserPhotoCrossDomainUploadService.ashx",
            type: "post",
            dataType: "json",
            timeout: 600000,
            success: function (data, textStatus) {
                if (data.result == true) {
                    User.Setting.NewUserUrl = data.acc.FilePath;
                    $("#imgUserPhoto").attr("src", data.acc.TempPath);
                    $.Confirm({ width: "auto", content: "您是否立刻保存一下您的新头像？" }, function () {
                        //保存
                        $("#aUpdate").trigger("click");
                    },
                    function () {
                        //取消
                    });
                } else {
                    $.Alert({width:"auto",content:data.message});
                }
            },
            error: function (data, status, e) {
                $.Alert("上传失败，错误信息：" + e);
            }
        });
    }
}

User.Setting.ReadDetailUser = function read_detail_user() {
    //上传头像
    $("#divUserAvatar").on("mouseenter", function (event) {
        $(".btnUploadPhoto").show();
    }).on("mouseleave", function (event) {
        $(".btnUploadPhoto").hide();
    });

    objPub.GetUserInfo(User.Setting.ID).done(function (json) {
        var result = json.d;
        if (result != null) {
            //头像
            if (result.MicroUserUrl != "") {
                $("#imgUserPhoto").attr("src", result.MicroUserUrl);
            }
            $("#fileUser").on("change", User.Setting.UploadUserEvent);
            User.Setting.OldUserUrl = User.Setting.NewUserUrl = result.MicroUserUrl;
            //用户名
            $("#tdSocialCode").text(result.SocialCode);
            User.Setting.ReadEmail = result.Email;
            $("#txtEmail").val(result.Email);
            //个人简介
            $("#txtSocialRemark").val(result.Remark);
            if (result.CanSearch == Enum.YesNo.Yes.toString()) {
                $("#txtCanSearch").prop("checked", true);
            }
            else {
                $("#txtCanSearch").prop("checked", false);
            }
        }
    });

    objPub.GetDetailUserInfo(User.Setting.ID).done(function (json) {
        var result = $.Deserialize(json.d);
        if (result != null) {
            var user = result.UserInfo;
            $("#txtName").val(user.RealName);
            $("#txtNick").val(user.UserName);

            //绑定用户性别
            $.each($("input[name='rdUserSex']"), function (index, item) {
                if ($(item).val() == user.Sex) {
                    $(item).prop("checked", true);
                    return false;
                }
            });

            User.Setting.UserNationBind(user.Nation);
            $("#txtQQ").val(user.qq);
            $("#txtTel").val(user.Tel);
            $("#txtFax").val(user.Fax);
            $("#txtMobile").val(user.Mobile);
            User.Setting.ReadMobile = user.Mobile;
            $("#txtOrgName").val(user.OrgName);
            $("#txtDuty").val(user.MajorDuty);
            $("#txtUniversity").val(user.University);
            $("#txtMotto").val(user.Motto);
            $("#txtRemark").val(user.Remark);
        }
    });
}

User.Setting.SubmitUserEvent = function SubmitUserEvent(event) {
    if (User.Setting.ID != "") {
        var person = {
            UserID: User.Setting.ID,
            UserName: $("#txtNick").val(),
            RealName: $("#txtName").val(),
            Sex: $("input[name='rdUserSex']:checked").val(),
            Nation: $("#sltUserNation").val(),
            qq: $("#txtQQ").val(),
            Tel: $("#txtTel").val(),
            Fax: $("#txtFax").val(),
            Mobile: $("#txtMobile").val(),
            Email: $("#txtEmail").val(),
            OrgName: $("#txtOrgName").val(),
            MajorDuty: $("#txtDuty").val(),
            University: $("#txtUniversity").val(),
            Motto: $("#txtMotto").val(),
            Remark: $("#txtRemark").val()
        };
        var social_user_info = {
            ID: User.Setting.ID,
            Mobile: $("#txtMobile").val(),
            Email: $("#txtEmail").val(),
            Remark: $("#txtSocialRemark").val(),
            UpdaterID: objPub.UserID,
            UpdaterName: objPub.UserName,
            CanSearch: $("#txtCanSearch").prop("checked") == true ? Enum.YesNo.Yes.toString() : Enum.YesNo.No.toString()
        };

        //读取头像
        if (User.Setting.NewUserUrl != User.Setting.OldUserUrl) {
            if (User.Setting.NewUserUrl != "") {
                social_user_info.MicroUserUrl = User.Setting.NewUserUrl;
            } else {
                social_user_info.MicroUserUrl = objPub.DefaultPhotoUrl;
            }
        }

        var url = new Array();
        url.push(objPub.ManageUrl);
        url.push("service/UserService.asmx/Submit");
        $.SimpleAjaxCors(url, "POST", "{miicSocialUserInfo:" + $.Serialize(social_user_info) + ",userInfo:" + $.Serialize(person) + "}").done(function (json) {
            var result = json.d;
            if (result == true) {
                    if (User.Setting.GoBack == true) {
                        //初始化首页面
                        objPub.InitLeftMain(true);
                    } else {
                        $.Alert("保存用户信息成功!", function () {
                            User.Setting.OldUserUrl = User.Setting.NewUserUrl = social_user_info.MicroUserUrl;
                        });
                    }
                    //如果已登录人员 修改用户头像
                    $("#imgHeadUserUrl").attr("src", social_user_info.MicroUserUrl);
            } else {
                $.Alert("保存用户信息失败，请联系管理员");
            }
        });
    } else {
        $.Alert("丢失ID，请刷新页面");
    }
}


//民族绑定
User.Setting.UserNationBind = function user_nation_bind(init_nation) {
    var url = new Array();
    url.push(objPub.ManageUrl);
    url.push("service/BasicService.asmx/GetNationInfos");
    $.SimpleAjaxCors(url, "POST",null).done(function (json) {
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
        $("#sltUserNation").empty().append(temp).val(init_nation);
    });
}
