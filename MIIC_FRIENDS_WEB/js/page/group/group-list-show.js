﻿Group.List = function () { }
Group.List.registerClass("Group.List");
Group.List.PageSize = 10;
Group.List.CurrentIndex = 0;
Group.List.CanPageLoad = false;
Group.List.TotalCount = 0;
Group.List.IsCreater = false;
Group.List.Manager = "";
Group.List.Init = function init(group_id,group_manager) {
    Group.List.Manager = group_manager;
    objPub.IsMain = false;
    //返回事件
    $("#aGoBack").off("click").on("click", Group.BackEvent);
   
    $(".discuss-block").remove();
    Group.IsCreater(group_id, objPub.UserID).done(function (json) {
        Group.List.IsCreater = json.d;
        if (Group.List.IsCreater == true) {
            $("#divGroupBar").remove();
        }
        else {
            $("#spnQuitGroup").show().on("click", { ID: group_id }, Group.List.RemoveEvent);
        }
    });
    //返回顶部
    $(window).on("scroll", function () {
        if ($(this).scrollTop() >= 135) {
            $("#filter").css({
                "top": "30px",
                "position": "fixed"
            });
        } else {
            $("#filter").css({
                "top": "135px",
                "position": "absolute"
            });
        }
    });
    var keyword = {
        Keyword: "",
        GroupID: group_id
    };
    var page = { pageStart: 1, pageEnd: Group.List.PageSize * 1 };
    Group.List.Search(keyword, page);
}
//退出讨论组事件
Group.List.RemoveEvent = function RemoveEvent(event) {
    var group_id = event.data.ID;
    var members = new Array();
    members.push(objPub.UserID);
    $.SimpleAjaxPost("service/GroupService.asmx/RemoveMember", true,
        "{groupID:'" + group_id + "',members:" + $.Serialize(members) + "}",
        function (json) {
            var result = json.d;
            if (result == true) {
                $.Alert("退出讨论组成功！", function () {
                    objPub.InitLeftMain(true);
                    Right.MyGroupBind();
                });
            }
            else {
                console.log("退出讨论组失败！");
            }
        });

}
Group.List.Search = function search(keyword, page) {
    Group.List.SearchBind(keyword, page);
    $.SimpleAjaxPost("service/GroupService.asmx/GetTopicSearchCount", true,
        "{searchView:" + $.Serialize(keyword) + "}",
         function (json) {
             Group.List.TotalCount = json.d;
             if (Group.List.TotalCount <= Group.List.PageSize) {
                 Group.List.CanPageLoad = false;
             }
             else {
                 if (Group.List.CurrentIndex == 0) {
                     Group.List.CanPageLoad = true;
                 }
             }
         });
}
Group.List.SearchBind = function search_bind(keyword, page) {
    $.SimpleAjaxPost("service/GroupService.asmx/TopicSearch", true,
      "{searchView:" + $.Serialize(keyword) + ",page:" + $.Serialize(page) + "}",
    function (json) {
        var result = $.Deserialize(json.d);
        var temp = "";
        if (result != null) {
            $.each(result, function (index, item) {
                temp += "<div class='discuss-block'>";
                temp += "<span class='discuss-num'>" + item.MessageCount + "</span>";
                temp += "<a class='discuss-title' href='javascript:void(0);' id='Topic" + index + "'>" + item.Content + "</a>";
                //删除按钮
                $(document).off("click", "#aGroupMessageDel" + index);
                if (item.CreaterID == objPub.UserID || objPub.UserID == Group.List.Manager) {
                    temp += "<a href='javascript:void(0);' style='float:right;height:17px;line-height:17px;' id='aGroupMessageDel" + index + "'><span class='icon-optSet icon-img icon-opt-delete' title='删除'></span></a>";
                    $(document).on("click", "#aGroupMessageDel" + index, { TopicID: item.ID,GroupID: keyword.GroupID }, Group.List.TopicDeleteEvent);
                }
              
                var topic_event_info= {
                    ID: item.ID,
                    Title: item.Content,
                    CreaterID: item.CreaterID,
                    CreaterName:item.CreaterName,
                    CreateTime: item.CreateTime,
                    MessageCount:item.MessageCount
                };
                $(document).off("click", "#Topic" + index);
                $(document).on("click", "#Topic" + index, { Topic: topic_event_info, GroupID: keyword.GroupID }, Group.List.GetMessageListEvent);
                temp += "<div class='discuss-message'>";
                if (item.MessageInfoList != null) {
                    temp += "<ul>";
                    $.each(item.MessageInfoList, function (sub_index, sub_item) {
                        temp += "<li class='clear-fix'>";
                        $(document).off("click", "#divShowGroupListFrom" + index + "_" + sub_index);
                        $(document).off("click", "#aShowGroupListFrom" + index + "_" + sub_index);
                        $(document).off("click", "#aShowGroupListTo" + index + "_" + sub_index);
                        if (sub_item.FromCommenterID != objPub.UserID && sub_item.FromCommenterIsFriend == Enum.YesNo.No.toString()) {
                            temp += "<div class='circle-photo' id='divShowGroupListFrom" + index + "_" + sub_index + "' style='cursor:default;'><img src='" + sub_item.FromCommenterUrl + "' /></div>";
                        } else {
                            temp += "<div class='circle-photo' id='divShowGroupListFrom" + index + "_" + sub_index + "' style='cursor:pointer;'><img src='" + sub_item.FromCommenterUrl + "' /></div>";
                            $(document).on("click", "#divShowGroupListFrom" + index + "_" + sub_index, { UserID: sub_item.FromCommenterID }, Group.List.ShowDetailUserEvent);
                        }
                        temp += "<div class='circle-content'>";
                        temp += "<div class='friend-name'>";
                        if (sub_item.FromCommenterID != objPub.UserID && sub_item.FromCommenterIsFriend == Enum.YesNo.No.toString()) {
                            temp += "<a href='javascript:void(0);' id='aShowGroupListFrom" + index + "_" + sub_index + "' style='cursor:default;'>" + sub_item.FromCommenterName + "</a>";
                        } else {
                            temp += "<a href='javascript:void(0);' id='aShowGroupListFrom" + index + "_" + sub_index + "' style='cursor:pointer;'>" + sub_item.FromCommenterName + "</a>";
                            $(document).on("click", "#aShowGroupListFrom" + index + "_" + sub_index, { UserID: sub_item.FromCommenterID }, Group.List.ShowDetailUserEvent);
                        }

                        if (sub_item.ToCommenterID != null) {
                            temp += "<span>回复</span>";
                            if (sub_item.ToCommenterIsFriend == Enum.YesNo.No.toString()) {
                                temp += "<a href='javascript:void(0);' id='aShowGroupListTo" + index + "_" + sub_index + "' style='cursor:default;'>" + sub_item.ToCommenterName + "</a>";
                            } else {
                                temp += "<a href='javascript:void(0);' id='aShowGroupListTo" + index + "_" + sub_index + "' style='cursor:pointer;'>" + sub_item.ToCommenterName + "</a>";
                                $(document).on("click", "#aShowGroupListTo" + index + "_" + sub_index, { UserID: sub_item.ToCommenterID }, Group.List.ShowDetailUserEvent);
                            }
                        }
                        temp += "</div>";
                        temp += "<div class='circle-content-intro'>" + sub_item.Content + "</div>";
                        if (sub_item.CommentTime.format("yyyy-MM-dd") == new Date().format("yyyy-MM-dd")) {
                            temp += "<div class='circle-content-time'>今天　" + sub_item.CommentTime.format("HH:mm") + "</div>";
                        }
                        else {
                            temp += "<div class='circle-content-time'>" + sub_item.CommentTime.format("yyyy-MM-dd") + "</div>";
                        }
                        temp += "</div>";
                        temp += "</li>";
                    });
                    temp += "</ul>";
                }
                temp += "</div>";
                temp += "</div>";
            });
            $("#divEmptyGroupList").hide();
            $("#sctGroupList").empty().append(temp);
        }
        else {
            $("#divEmptyGroupList").show();
            $(".discuss-block").remove();
        }
    });
}


Group.List.GetMessageListEvent = function GetMessageListEvent(event) {
    Group.Message.Init(event.data.Topic,event.data.GroupID);
}

Group.List.BackEvent = function BackEvent(event) {
    var group_id = event.data.ID;
    Group.List.Init(group_id);
}

Group.List.ShowDetailUserEvent = function ShowDetailUserEvent(event) {
    var user_id = event.data.UserID;
    $(".main-left").load("../biz/left/moments.html", function (response, status) {
        if (status == "success") {
            objPub.IsMain = true;
            Moments.List.Person.Init(user_id);
        }
    });
}

Group.List.TopicDeleteEvent = function TopicDeleteEvent(event) {
    var topic_id = event.data.TopicID;
    var group_id = event.data.GroupID;
    $.Confirm("确认删除此讨论么？", function () {
        $.SimpleAjaxPost("service/GroupService.asmx/DeleteTopic", true,
       "{topicID:'" + topic_id + "'}",
        function (json) {
            var result = json.d;
            if (result == true) {
                if (Group.List.TotalCount == 1) {
                    //如果只有一个讨论则触发返回事件到trends页
                    $("#aGoBack").trigger("click");
                } else {
                    //刷新列表
                    var keyword = {
                        Keyword: "",
                        GroupID: group_id
                    };
                    var page = { pageStart: 1, pageEnd: Group.List.PageSize * 1 };
                    Group.List.Search(keyword, page);
                }
            } else {
                $.Alert("删除讨论失败");
            }
        });
    });
}