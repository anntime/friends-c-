Chat = function () { }
Chat.registerClass("Chat");
Chat.Sdk = new WSDK();
Chat.AddressBookCount = 0;
Chat.GroupCount = 0;
Chat.RecentGroupList = new Array();
Chat.RecentAddressBookList = new Array();
Chat.MessageType = Enum.ChatMessageType;
Chat.LogNextKey = "";
Chat.LogCount = 1000;
Chat.CurrentLogEndTime =null;
Chat.CurrentIsGroup = false;
Chat.CurrentItem = null;
Chat.AppKey = "";
Chat.AppSecret = "";
//当前开启的ID，可能是用户ID 也可能是TribeID
Chat.CurrentID="";
//查找通讯录列表
Chat.SearchAddressBookList = new Array();
//登录
Chat.Login = function login() {
    $.SimpleAjaxPost("service/BasicService.asmx/GetOpenIMAppKey", false)
        .done(function (json) {
        var result = json.d;
        Chat.AppKey = result.Name;
        Chat.AppSecret = result.Value;
        Chat.Sdk.Base.login({
            uid: objPub.UserID,
            appkey: result.Name,
            credential: result.Value,
            timeout: 5000,
            success: function (data) {
                console.log("login success");
                //在线交流
                $("#aMyIMSpace").off("click").on("click", function (event) {
                    $(".main-left").load(objPub.FriendsUrl + "biz/left/chat/chat.html", function (response, status) {
                        if (status == "success") {
                            Chat.Init();
                        }
                    });
                });
                shareHubProxy.invoke("SendLine");
                Chat.GetMyTotalCount().done(function (json) {
                    var result = json.d;
                    $.each(result, function (index, item) {
                        if (item.Name == "AddressBookCount") {
                            Chat.AddressBookCount = item.Value;
                        }
                        else if (item.Name == "GroupCount") {
                            Chat.GroupCount = item.Value;
                        }
                        else if (item.Name == "CommunityCount") {

                        }
                    });
                    Chat.Sdk.Base.getUnreadMsgCount({
                        count: parseInt(Chat.AddressBookCount) + parseInt(Chat.GroupCount),
                        success: function (data) {
                            var result = data.data;
                            if (result.length > 0) {
                                $.each(result, function (index, item) {
                                    if (item.contact.substring(0, 8) === "chntribe") {
                                        if (item.msgCount > 0) {
                                            var temp = item;
                                            temp.contact = item.contact.substring(8);
                                            Chat.RecentGroupList.push(temp);
                                            $("#spnIMCount").addClass("user-dot");
                                        }
                                    }
                                    else {
                                        if (item.msgCount > 0) {
                                            var temp = item;
                                            temp.contact = item.contact.substring(8);
                                            Chat.RecentAddressBookList.push(temp);
                                            $("#spnIMCount").addClass("user-dot");
                                        }
                                    }

                                });
                            }

                        },
                        error: function (error) {
                            console.log('获取未读消息的条数失败', error);
                          
                        }
                    });
                    
                });

            },
            error: function (error) {
                console.log("login fail", error);
                $("#aMyIMSpace").remove();

            }
        });
       
    });
}
//登出
Chat.Logout = function logout() {
    try {
        Chat.Sdk.Base.logout({
            uid: objPub.UserID.toLowerCase(),
            appkey: Chat.AppKey,
            credential: Chat.AppSecret
        });
        shareHubProxy.invoke("SendOffline");
    }
    catch (e) {
        console.log("Error:" + e);
    }
}
//回收垃圾
Chat.GC = function () {
    Chat.CurrentIsGroup = false;
    Chat.CurrentItem = null;
}
//聊天初始化
Chat.Init = function init() {
    Chat.ReceiveInit();
    Chat.GC();
    if (Chat.RecentAddressBookList.length > 0) {
        $("#spnMyAddressBookChatCount").addClass("user-dot");
    }
    if (Chat.RecentGroupList.length > 0) {
        $("#spnMyGroupChatCount").addClass("user-dot");
    }
    //左侧联系人滚动条
    $(".message-list").mCustomScrollbar({
        theme: "dark",
        mouseWheel: { preventDefault: true },
    });
    //右侧chat内容滚动条
    $("#divMessageContentList").mCustomScrollbar({
        theme: "dark",
        mouseWheel: { preventDefault: true },
        callbacks: {
            alwaysTriggerOffsets: false,
            onTotalScrollBack: function () {
                if (Chat.LogNextKey !== "") {
                    if (Chat.CurrentIsGroup == false) {
                        Chat.GetAddressBookList(Chat.CurrentItem,false);
                    }
                    else {
                        Chat.GetGroupChatList(Chat.CurrentItem,false);
                    }
                }

            }
        }
    });
    //本地存储处理
    $.SimpleAjaxPost("service/AddressBookService.asmx/GetMyAllAddressBookList", true).done(function (json) {
        var result = $.Deserialize(json.d);
        if (result != null) {
            $.each(result, function (index, item) {
                Chat.SearchAddressBookList.push(item);
            });
        }
        //搜索通讯录好友
        $("#spnSearch").off("click").on("click", Chat.SearchEvent);
        $("#txtKeyword").off("keypress").on("keypress", Chat.SearchKeyPressEvent);
    });
    $("#liOftenUsedAddressBookChat").off("click").on("click", function (event) {
        $(this).addClass("selected").siblings().removeClass("selected");
        var tagNum = $(this).index();
        $(".message-list").hide();
        $(".message-list:eq(" + tagNum + ")").show();
        //经常联系人列表
        Chat.GetOftenUsedAddressBookChatList();
        $("li[id^='liMyAddressBookChatItem'],li[id^='liMyGroupChatItem']").removeData("UserID");
    });
    $("#liMyAddressBookChat").off("click").on("click", function (event) {
        $(this).addClass("selected").siblings().removeClass("selected");
        var tagNum = $(this).index();
        $(".message-list").hide();
        $(".message-list:eq(" + tagNum + ")").show();
        //通讯录列表
        Chat.GetMyAllAddressBookList();
        $("li[id^='liOftenUsedAddressBookChatItem'],li[id^='liMyGroupChatItem']").removeData("UserID");
    });
    $("#liMyGroupChat").off("click").on("click", function (event) {
        $(this).addClass("selected").siblings().removeClass("selected");
        var tagNum = $(this).index();
        $(".message-list").hide();
        $(".message-list:eq(" + tagNum + ")").show();
        //讨论组列表
        Chat.GetMyAllGroupList();
        $("li[id^='liOftenUsedAddressBookChatItem'],li[id^='liMyAddressBookChatItem']").removeData("UserID");
    });

    //联系人
    $("#liOftenUsedAddressBookChat").trigger("click");
   
   

    //初始化chat-editor
    $("#txtChatMessage").miicWebEdit({
        id: "txtChatMessage",
        css: "chat-editor-short",
        placeholder: "",
        faceid: "aEmotionSend",
        submit: "btnSendChatMessage",
        facePath: "../../images/arclist/", //表情存放的路径
        charAllowed: 140,
        charWarning: 20,
        charCss: "chat-char-tip",
        charCounterElement: "span"
    });

}
//获取我的统计总数
Chat.GetMyTotalCount = function get_my_total_count() {
    return $.SimpleAjaxPost("service/UserService.asmx/GetMyShareCount", false);

}
//接收消息
Chat.ReceiveInit = function receive_init() {
    var Event = Chat.Sdk.Event;
    Event.on("CHAT.MSG_RECEIVED", function (data) {
        console.log("我能收到成功的单聊消息", data);
        var temp = "";
        var result = data.data;
        var user_id = result.touid.substring(8);
        $.each(Chat.SearchAddressBookList,function(index,item){
            if (item.AddresserID.toLowerCase() == user_id.toLowerCase()) {
                if ($("#liOftenUsedAddressBookChat").hasClass("selected") == true) {
                    $("#spnMyAddressBookChatCount,#spnIMCount").addClass("user-dot");
                    //打开着
                    if (Chat.CurrentID.toLowerCase() == user_id.toLowerCase()) {
                        $.each(result.msgs, function (sub_index, sub_item) {
                            temp += " <li class='friend-say'>";
                            temp += "<div class='say-user'>";
                            temp += "<div class='message-user'>";
                            temp += "<img src='" + item.AddresserUrl + "' alt='" + item.AddresserName + "' title='" + item.AddresserName + "'>";
                            temp += "</div>";
                            temp += "</div>";
                            temp += "<div class='message-text'>";
                            temp += "<p>" + sub_item.msg + "</p>";
                            temp += "<div class='message-time'>" + new Date(sub_item.time).toLocaleTimeString()+ "</div>";
                            temp += "</div>";
                            temp += "</li>";
                        });
                        $("#ulChatMessageContentList").append(temp);
                        $("#divMessageContentList").mCustomScrollbar("scrollTo", "bottom");
                    }
                    else {
                        $("#spnOftenUsedAddressBookChatCount").addClass("user-dot");
                        $.each($("li[id^='liOftenUsedAddressBookChatItem']"), function (index, item) {
                            if ($(item).data("UserID").toLowerCase() == user_id.toLowerCase()) {
                                if($("#divOftenUsedAddressBookChatCount"+index).length>0){
                                    //存在
                                    var num=parseInt($("#divOftenUsedAddressBookChatCount"+index).text());
                                    $("#divOftenUsedAddressBookChatCount"+index).text(num+1);
                                }
                                else{
                                    //不存在
                                    $(item).append("<div id = 'divOftenUsedAddressBookChatCount" + index + "' class='message-dot'>1</div>");
                                }
                                return false;
                            }
                        });
                    }
                }
                else if ($("#liMyAddressBookChat").hasClass("selected") == true) {
                    $("#spnIMCount").addClass("user-dot");
                    if (Chat.CurrentID.toLowerCase() == user_id.toLowerCase()) {
                        $.each(result.msgs, function (sub_index, sub_item) {
                            temp += " <li class='friend-say'>";
                            temp += "<div class='say-user'>";
                            temp += "<div class='message-user'>";
                            temp += "<img src='" + item.AddresserUrl + "' alt='" + item.AddresserName + "' title='" + item.AddresserName + "'>";
                            temp += "</div>";
                            temp += "</div>";
                            temp += "<div class='message-text'>";
                            temp += "<p>" + sub_item.msg + "</p>";
                            temp += "<div class='message-time'>" + new Date(sub_item.time).toLocaleTimeString()+ "</div>";
                            temp += "</div>";
                            temp += "</li>";
                        });
                        $("#ulChatMessageContentList").append(temp);
                        $("#divMessageContentList").mCustomScrollbar("scrollTo", "bottom");
                    }
                    else {
                        $.each($("li[id^='liMyAddressBookChatItem']"), function (index, item) {
                            if ($(item).data("UserID").toLowerCase() == user_id.toLowerCase()) {
                                if ($("#divMyAddressBookChatCount" + index).length > 0) {
                                    //存在
                                    var num = parseInt($("#divMyAddressBookChatCount" + index).text());
                                    $("#divMyAddressBookChatCount" + index).text(num + 1);
                                }
                                else {
                                    //不存在
                                    $(item).append("<div id = 'divMyAddressBookChatCount" + index + "' class='message-dot'>1</div>");
                                }
                                return false;
                            }
                        });
                    }
                }
              
            }
        });
        
    });
    Event.on("TRIBE.MSG_RECEIVED", function (data) {
        console.log("我能收到成功的群聊消息", data);
        var result = data.data;
        //如果为当前的群
        if (Chat.CurrentID.toLowerCase() == result.touid.toLowerCase()) {
            var temp = "";
            $.each(result.msgs, function (index, item) {
                objPub.GetDetailUserInfo(item.from.substring(8)).done(function (json) {
                    var user = $.Deserialize(json.d);
                    temp += " <li class='friend-say'>";
                    temp += "<div class='say-user'>";
                    temp += "<div class='message-user'>";
                    temp += "<img src='" + user.SocialUserInfo.MicroUserUrl + "' alt='" + user.UserInfo.UserName + "' title='" + user.UserInfo.UserName + "'>";
                    temp += "</div>";
                    temp += "</div>";
                    temp += "<div class='message-text'>";
                    temp += "<p>" + item.msg + "</p>";
                    temp += "<div class='message-time'>" + new Date(item.time).toLocaleTimeString() + "</div>";
                    temp += "</div>";
                    temp += "</li>";
                    $("#ulChatMessageContentList").append(temp);
                    $("#divMessageContentList").mCustomScrollbar("scrollTo", "bottom");
                });
                
            });
           
        }
        else {
            $("#spnIMCount,#spnMyGroupChatCount").addClass("user-dot");
            $.each($("li[id^='liMyGroupChatItem']"), function (index, item) {
                if ($(item).data("GroupTribeID").toLowerCase() == result.touid.toLowerCase()) {
                    if ($("#divMyGroupChatCount" + index).length > 0) {
                        //存在
                        var num = parseInt($("#divMyGroupChatCount" + index).text());
                        $("#divMyGroupChatCount" + index).text(num + 1);
                    }
                    else {
                        //不存在
                        $(item).append("<div id = 'divMyGroupChatCount" + index + "' class='message-dot'>1</div>");
                    }
                    return false;
                }
            });
        }
        
    });
    Chat.Sdk.Base.startListenAllMsg();
}
//群发
Chat.SendGroupMessage = function send(tribe_id,message_type) {
    var chat_message = $("#txtChatMessage").getContents();
    if (chat_message == "") {
        $.Alert("请填写待发送信息再发送");
    }
    else {
       
        Chat.Sdk.Tribe.sendMsg({
            tid: tribe_id,
            msg: chat_message,
            msgType: message_type,
            success: function (data) {
                var temp = "";
                temp += " <li class='my-say'>";
                temp += "<div class='message-text'>";
                temp += "<p>" + chat_message + "</p>";
                temp += "<div class='message-time'>" + new Date().toLocaleTimeString() + "</div>";
                temp += "</div>";
                temp += "<div class='say-user'>";
                temp += "<div class='message-user'>";
                temp += "<img src='" + objPub.UserUrl + "' >";
                temp += "</div>";
                temp += "</div>";
                temp += "</li>";
                $("#ulChatMessageContentList").append(temp);
                $("#txtChatMessage").setContents("");
                $("#divMessageContentList").mCustomScrollbar("scrollTo", "bottom");
            },
            error: function (error) {
                console.log("发送群消息失败", error);
            }
        });
    }
}
Chat.SendGroupMessageEvent = function SendGroupMessageEvent(event) {
    Chat.SendGroupMessage(event.data.TribeID,event.data.MessageType);
}
Chat.SendGroupMessagePressEvent = function SendGroupMessagePressEvent(event) {
    //回车或ctrl+回车
    if (event.which == 13 || (event.ctrlKey == true && event.which == 10)) {
        Chat.SendGroupMessage(event.data.TribeID, event.data.MessageType);
    }
}
//点对点发
Chat.SendMessageEvent = function SendMessageEvent(event) {
    Chat.SendMessage(event.data.ToUserID,event.data.MessageType);
}
Chat.SendMessagePressEvent = function SendMessagePressEvent(event) {
    //回车或ctrl+回车
    if (event.which == 13||(event.ctrlKey==true && event.which == 10)) {
        Chat.SendMessage(event.data.ToUserID,event.data.MessageType);
    }
   
}
//发送单聊消息
Chat.SendMessage = function send(to_user_id,message_type) {
    var chat_message = $("#txtChatMessage").getContents();
    if (chat_message == "") {
        $.Alert("请填写待发送信息再发送");
    }
    else {
        Chat.Sdk.Chat.sendMsg({
            touid: to_user_id,
            msg: chat_message,
            msgType: message_type,
            success: function (data) {
                var temp = "";
                temp += " <li class='my-say'>";
                temp += "<div class='message-text'>";
                temp += "<p>" + chat_message + "</p>";
                temp += "<div class='message-time'>" + new Date().toLocaleTimeString() + "</div>";
                temp += "</div>";
                temp += "<div class='say-user'>";
                temp += "<div class='message-user'><img src='" + objPub.UserUrl + "' ></div>";
                temp += "</div>";
                temp += "</li>";
                $("#ulChatMessageContentList").append(temp);
                $("#txtChatMessage").setContents("");
                $("#divMessageContentList").mCustomScrollbar("scrollTo", "bottom");
            },
            error: function (error) {
                console.log("send fail", error);
            }
        });
    }
}
//查询通讯录好友
Chat.SearchEvent = function SearchEvent(event) {
    if ($("#txtKeyword").val() == "") {
        $("#divChatBar").show();
        $("#liOftenUsedAddressBookChat").trigger("click");
    }
    else {
        $("#divChatBar").hide();
        var reg = eval("/" + $("#txtKeyword").val() + "/g");
        var temp = "";
        $(".message-list").hide();
        $(".message-list:eq(3)").show();
        if (Chat.SearchAddressBookList.length > 0) {
            $.each(Chat.SearchAddressBookList, function (index, item) {
                if (reg.test(item.AddresserName) == true) {
                    temp += "<li id='liSearchChatItem" + index + "'>";
                    temp += "<div class='message-user'><img src='" + item.AddresserUrl + "' title='" + item.AddresserName + "'></div>";
                    temp += "<div class='message-name'>" + item.AddresserName.replace(reg, "<b>" + $("#txtKeyword").val() + "</b>") + "</div>";
                    temp += "</li>";
                    $(document).off("click", "#liSearchChatItem" + index);
                    $(document).on("click", "#liSearchChatItem" + index, { UserItem: item }, Chat.AddressBookChatItemChangeEvent);
                }
            });
            $("#ulSearchChatList").empty().append(temp);
            $("#liSearchChatItem0").trigger("click");
        }
        else {
            $("#ulSearchChatList").empty();
        }
    }
}
//查询通讯录好友
Chat.SearchKeyPressEvent = function SearchKeyPressEvent(event) {
    if (event.which == 13) {
        if ($("#txtKeyword").val() == "") {
            $("#divChatBar").show();
            $("#liOftenUsedAddressBookChat").trigger("click");
        }
        else {
            $("#divChatBar").hide();
            var reg = eval("/" + $("#txtKeyword").val() + "/g");
            var temp = "";
            $(".message-list").hide();
            $(".message-list:eq(3)").show();
            if (Chat.SearchAddressBookList.length > 0) {
                $.each(Chat.SearchAddressBookList, function (index, item) {
                    if (reg.test(item.AddresserName) == true) {
                        temp += "<li id='liSearchChatItem" + index + "'>";
                        temp += "<div class='message-user'><img src='" + item.AddresserUrl + "' alt='" + item.AddresserName + "' title='" + item.AddresserName + "'></div>";
                        temp += "<div class='message-name'>" + item.AddresserName.replace(reg, "<b>" + $("#txtKeyword").val() + "</b>") + "</div>";
                        temp += "</li>";
                        $(document).off("click", "#liSearchChatItem" + index);
                        $(document).on("click", "#liSearchChatItem" + index, { UserItem: item }, Chat.AddressBookChatItemChangeEvent);
                    }
                });
                $("#ulSearchChatList").empty().append(temp);
                $("#liSearchChatItem0").trigger("click");
            }
            else {
                $("#ulSearchChatList").empty();
            }
        }
        
    }
}
//获取我的通讯录列表
Chat.GetMyAllAddressBookList = function get_my_all_address_book_list() {
    $.SimpleAjaxPost("service/AddressBookService.asmx/GetMyAllAddressBookList", true,
     function (json) {
         var result = $.Deserialize(json.d);
         var temp = "";
         if (result != null) {
             var dim = Math.ceil(result.length / 30);
             var user_list = new Array(dim);
             $.each(user_list, function (index, item) {
                 user_list[index] = new Array();
             });
             var i = 0;
             $.each(result, function (index, item) {
                 temp += "<li id='liMyAddressBookChatItem" + index + "'>";
                 $.each(Chat.RecentAddressBookList, function (sub_index, sub_item) {
                     if (sub_item.contact.toLowerCase() === item.AddresserID.toLowerCase()&&sub_item.msgCount>0) {
                         temp += "<div id = 'divMyAddressBookChatCount" + index + "' class='message-dot'>" + sub_item.msgCount + "</div>";
                         return false;
                     }
                 });
                
                 temp += "<div class='message-user'>";
                 temp += "<img src='" + item.AddresserUrl + "' alt='" + item.AddresserName + "' title='" + item.AddresserName + "' id='imgOffUser"+ index +"'>";
                 temp += "</div>";
                 temp += "<div class='message-name'>" + item.AddresserName + "</div>";
                 temp += "</li>";
                 
                 $(document).off("click", "#liMyAddressBookChatItem" + index);
                 $(document).on("click", "#liMyAddressBookChatItem" + index, { UserItem: item }, Chat.AddressBookChatItemChangeEvent);
                 if (index % 30 == 0 && index != 0) { i++; }
                 user_list[i].push(item.AddresserID);
             });
             $("#ulMyAddressBookChatList").empty().append(temp);
             $.each(user_list, function (index, item) {
                 var lower_items = new Array();
                 $.each(item, function (lower_index, lower_item) {
                     lower_items.push(lower_item.toLowerCase());
                 });
                    Chat.Sdk.Chat.getUserStatus({
                        uids: lower_items,
                        hasPrefix: false,
                        appkey: Chat.AppKey,
                        success: function (result) {
                            var status = result.data.status;
                            $.each(status, function (sub_index, sub_item) {
                                if (Enum.ChatUserStatus.Offline == sub_item) {
                                    $("#liMyAddressBookChatItem" + (index * 30 + sub_index)).append("<div class='offline'><img src='../images/off.png'></div>");
                                }
                            });


                        },
                        error: function () {
                            console.log("批量获取经常联系好友在线状态失败");
                        }
                    });
                });
             $.each(result, function (index, item) {
                 $("#liMyAddressBookChatItem" + index).data("UserID", item.AddresserID);
                 $.each(Chat.RecentAddressBookList, function (sub_index, sub_item) {
                     if (sub_item.contact.toLowerCase() === item.AddresserID.toLowerCase()) {
                         $("#divMyAddressBookChatCount" + index).data("UserID", item.AddresserID);
                         return false;
                     }
                 });
             });
             $("#liMyAddressBookChatItem0").trigger("click");
         }
         else {

             $("#ulMyAddressBookChatList").empty();
         }

     });
}
//获取我的讨论组列表
Chat.GetMyAllGroupList = function get_my_all_group_list() {
    $.SimpleAjaxPost("service/GroupService.asmx/GetMyAllGroupList", true,
    function (json) {
        var result = $.Deserialize(json.d);
        var temp = "";
        if (result != null) {
            $.each(result, function (index, item) {
                temp += "<li id='liMyGroupChatItem" + index + "'>";
                $.each(Chat.RecentGroupList, function (sub_index, sub_item) {
                    if (sub_item.contact === item.TribeID) {
                        temp += "<div id = 'divMyGroupChatCount" + index + "' class='message-dot'>" + sub_item.msgCount + "</div>";
                        return false;
                    }
                });
                temp += "<div class='message-user'><img src='" + item.LogoUrl + "' alt='"+item.Name+"' title='" + item.Name + "'></div>";
                temp += "<div class='message-name'>" + (item.Remark == "" ? item.Name : item.Remark) + "</div>";
                temp += "</li>";
                $(document).off("click", "#liMyGroupChatItem" + index);
                $(document).on("click", "#liMyGroupChatItem" + index, { GroupItem: item, Index: index }, Chat.GroupChatItemChangeEvent);
            });
            $("#ulMyGroupChatList").empty().append(temp);
            $.each(result, function (index, item) {
                $("#liMyGroupChatItem" + index).data("GroupTribeID", item.TribeID);
            });
            $("#liMyGroupChatItem0").trigger("click");
        }
        else {
            $("#ulMyGroupChatList").empty();
        }


    });
}
//获取经常联系人列表
Chat.GetOftenUsedAddressBookChatList = function get_often_used_addressbook_chat_list() {
    $.SimpleAjaxPost("service/AddressBookService.asmx/GetMyAllOftenUsedAddressBookList", true,
  function (json) {
      var result = $.Deserialize(json.d);
      var temp = "";
      if (result != null) {
          var dim = Math.ceil(result.length / 30);
          var user_list = new Array(dim);
          $.each(user_list, function (index, item) {
              user_list[index] = new Array();
          });
          $.each(result, function (index, item) {
              $.each(Chat.RecentAddressBookList, function (sub_index, sub_item) {
                  if (sub_item.contact.toLowerCase() === item.AddresserID.toLowerCase()) {
                      $("#spnOftenUsedAddressBookChatCount").addClass("user-dot");
                      return false;
                  }
              });
              if ($("#spnOftenUsedAddressBookChatCount").hasClass("user-dot") == true) {
                  return false;
              }
          });
         
          var i=0;
          $.each(result, function (index, item) {
              temp += "<li id='liOftenUsedAddressBookChatItem" + index + "' " + (index == 0 ? "class='selected'" : "") + ">";
              $.each(Chat.RecentAddressBookList, function (sub_index, sub_item) {
                  if (sub_item.contact.toLowerCase() === item.AddresserID.toLowerCase()&&sub_item.msgCount>0) {
                      temp += "<div id='divOftenUsedAddressBookChatCount" + index + "' class='message-dot'>" + sub_item.msgCount + "</div>";
                      return false;
                  }
              });
              temp += "<div class='message-user'><img id='imgOffOftenUsedUser" + index + "' src='" + item.AddresserUrl + "' alt='" + item.AddresserName + "' title='" + item.AddresserName + "'></div>";
              temp += "<div class='message-name'>" + item.AddresserName + "</div>";
              temp += "<div id='divOffenUsedAddressBookChatChatClose" + index + "' class='message-user-del'><img src='../images/close.png'></div>";
              temp += "</li>";
              $(document).off("click", "#divOffenUsedAddressBookChatChatClose" + index);
              $(document).on("click", "#divOffenUsedAddressBookChatChatClose" + index, { Index: index }, Chat.OffenUsedAddressBookChatChatCloseEvent);
              $(document).off("click", "#liOftenUsedAddressBookChatItem" + index);
              $(document).on("click", "#liOftenUsedAddressBookChatItem" + index, { UserItem: item }, Chat.AddressBookChatItemChangeEvent);
              if (index % 30 == 0 && index != 0) { i++; }
              user_list[i].push(item.AddresserID);
          });
          $.each(user_list, function (index, item) {
              var lower_items = new Array();
              $.each(item, function (lower_index, lower_item) {
                  lower_items.push(lower_item.toLowerCase());
              });
                Chat.Sdk.Chat.getUserStatus({
                    uids: lower_items,
                    hasPrefix: false,
                    appkey: Chat.AppKey,
                    success: function (result) {
                        var status = result.data.status;
                        $.each(status, function (sub_index, sub_item) {
                            if (Enum.ChatUserStatus.Offline == sub_item) {
                                $("#liOftenUsedAddressBookChatItem" + (index * 30 + sub_index)).append("<div class='offline'><img src='../images/off.png'></div>");
                            }
                        });
                    },
                    error: function () {
                        console.log('批量获取经常联系好友在线状态失败');
                    }
                });
            });
          $("#ulOftenUsedAddressBookChatList").empty().append(temp);
          $.each(result, function (index, item) {
              $("#liOftenUsedAddressBookChatItem" + index).data("UserID", item.AddresserID);
              $.each(Chat.RecentAddressBookList, function (sub_index, sub_item) {
                  if (sub_item.contact.toLowerCase() === item.AddresserID.toLowerCase()) {
                      $("#divOftenUsedAddressBookChatCount" + index).data("UserID", item.AddresserID);
                      return false;
                  }
              });
          });
          $("#liOftenUsedAddressBookChatItem0").trigger("click");

      }
  });

}
Chat.GetGroupChatList = function get_group_chat_list(group_info,is_first) {
    Chat.CurrentIsGroup = true;
    Chat.CurrentItem = group_info;
    Chat.CurrentLogEndTime = new Date();
    var year = Chat.CurrentLogEndTime.getFullYear();
    var month = Chat.CurrentLogEndTime.getMonth()+1;
    var day = Chat.CurrentLogEndTime.getDate();
    var request = {
        TribeID: group_info.TribeID,
        BeginTime: year + "-" + month + "-" + (day - 1) + " 16:00:00",
        EndTime: Chat.CurrentLogEndTime,
        Count: Chat.LogCount,
        NextKey: Chat.LogNextKey
    };
    $.SimpleAjaxPost("service/GroupService.asmx/GetChatLogs", true, "{chatLogsRequestInfo:" + $.Serialize(request) + "}")
      .done(function (json) {
          var result = json.d;
          if (result != null) {
              var temp = "";
              Chat.LogNextKey = result.NextKey;
              $.each(result.Messages, function (index, item) {
                  if (item.FromUserID == objPub.UserID) {
                      temp += " <li class='my-say'>";
                      temp += "<div class='message-text'>";
                      temp += "<p>" + item.Content[0].Value + "</p>";
                      if (new Date(item.Time * 1000).format("yyyy-MM-dd") === new Date().format("yyyy-MM-dd")) {
                          temp += "<div class='message-time'>" + new Date(item.Time * 1000).toLocaleTimeString() + "</div>";
                      }
                      else {
                          temp += "<div class='message-time'>" + new Date(item.Time * 1000).toLocaleString()+ "</div>";
                      }

                      temp += "</div>";
                      temp += "<div class='say-user'>";
                      temp += "<div class='message-user'>";
                      temp += "<img src='" + objPub.UserUrl + "' >";
                      temp += "</div>";
                      temp += "</div>";
                      temp += "</li>";
                  }
                  else {
                      temp += " <li class='friend-say'>";
                      temp += "<div class='say-user'>";
                      temp += "<div class='message-user'>";
                      temp += "<img src='" + item.FromUserUrl + "' title='" + item.FromUserName + "'>";
                      temp += "</div>";
                      temp += "</div>";
                      temp += "<div class='message-text'>";
                      temp += "<p>" + item.Content[0].Value + "</p>";
                      if (new Date(item.Time * 1000).format("yyyy-MM-dd") === new Date().format("yyyy-MM-dd")) {
                          temp += "<div class='message-time'>" + new Date(item.Time * 1000).toLocaleTimeString() + "</div>";
                      }
                      else {
                          temp += "<div class='message-time'>" + new Date(item.Time * 1000).toLocaleString() + "</div>";
                      }

                      temp += "</div>";
                      temp += "</li>";
                  }
              });
              $("#ulChatMessageContentList").prepend(temp);
              if (is_first == true) {
                  $("#divMessageContentList").mCustomScrollbar("scrollTo", "bottom");
              }
          }

      });
}
//切换讨论组事件
Chat.GroupChatItemChangeEvent = function GroupChatItemChangeEvent(event) {
    var group_info = event.data.GroupItem;
    var index = event.data.Index;
    Chat.CurrentID = group_info.TribeID;
    $(this).addClass("selected").siblings().removeClass("selected");
    Chat.Sdk.Chat.setReadState({
        touid: "chntribe" + group_info.TribeID,
        hasPrefix: true,
        timestamp: Math.ceil(new Date() / 1000),
        success: function (data) {
            if (data.resultText == "SUCCESS") {
                $("#divMyGroupChatCount" + index).remove();
                Array.remove(Chat.RecentGroupList, "chntribe" + group_info.TribeID);
                if ($("div[id^='divMyGroupChatCount']").length == 0) {
                    $("#spnMyGroupChatCount").removeClass("user-dot");
                }
                if (Chat.RecentGroupList.length == 0 && Chat.RecentGroupList.length == 0) {
                    $("#spnIMCount").removeClass("user-dot");
                }
            }
        },
        error: function (error) {
            console.log("设置已读失败", error);
        }
    });
    $("#ulChatMessageContentList").empty();
    $("#btnSendChatMessage").off("click").on("click", { TribeID: group_info.TribeID, MessageType: Chat.MessageType.Text }, Chat.SendGroupMessageEvent);
    $("#txtChatMessage").off("keypress").on("keypress", { TribeID: group_info.TribeID, MessageType: Chat.MessageType.Text }, Chat.SendGroupMessagePressEvent);
    $("#fileImageSend").off("change").on("change", { ChatType: Enum.BusinessType.Group }, Chat.UploadImageEvent);
    Chat.GetGroupChatList(group_info,true);
}
Chat.GetAddressBookList = function get_address_book_list(user,is_first) {
    Chat.CurrentIsGroup = false;
    Chat.CurrentItem = user;
    var to_user_id = user.ToUserID;
    var to_user_name = user.ToUserName;
    var to_user_url = user.ToUserUrl;
    Chat.CurrentLogEndTime = new Date();
    var year = Chat.CurrentLogEndTime.getFullYear();
    var month = Chat.CurrentLogEndTime.getMonth()+1;
    var day = Chat.CurrentLogEndTime.getDate();
    var request = {
        FromUser: objPub.UserID,
        ToUser: to_user_id,
        BeginTime: year + "-" + month + "-" + (day - 1) + " 16:00:00",
        EndTime: Chat.CurrentLogEndTime,
        Count: Chat.LogCount,
        NextKey: Chat.LogNextKey
    };
    $.SimpleAjaxPost("service/AddressBookService.asmx/GetChatLogs", true, "{chatLogsRequestInfo:" + $.Serialize(request) + "}")
        .done(function (json) {
            var result = json.d;
            if (result != null) {
                var temp = "";
                Chat.LogNextKey = result.NextKey;
                $.each(result.Messages, function (index, item) {
                    if (item.Direction == 0) {
                        temp += " <li class='my-say'>";
                        temp += "<div class='message-text'>";
                        temp += "<p>" + item.ContentList[0].Value + "</p>";
                        if (new Date(item.Time * 1000).format("yyyy-MM-dd") === new Date().format("yyyy-MM-dd")) {
                            temp += "<div class='message-time'>" + new Date(item.Time * 1000).toLocaleTimeString() + "</div>";
                        }
                        else {
                            temp += "<div class='message-time'>" + new Date(item.Time * 1000).toLocaleString()+ "</div>";
                        }

                        temp += "</div>";
                        temp += "<div class='say-user'>";
                        temp += "<div class='message-user'>";
                        temp += "<img src='" + objPub.UserUrl + "' >";
                        temp += "</div>";
                        temp += "</div>";
                        temp += "</li>";
                    }
                    else {
                        temp += " <li class='friend-say'>";
                        temp += "<div class='say-user'>";
                        temp += "<div class='message-user'>";
                        temp += "<img src='" + to_user_url + "' title='" + to_user_name + "'>";
                        temp += "</div>";
                        temp += "</div>";
                        temp += "<div class='message-text'>";
                        temp += "<p>" + item.ContentList[0].Value + "</p>";
                        if (new Date(item.Time * 1000).format("yyyy-MM-dd") === new Date().format("yyyy-MM-dd")) {
                            temp += "<div class='message-time'>" + new Date(item.Time * 1000).toLocaleTimeString() + "</div>";
                        }
                        else {
                            temp += "<div class='message-time'>" + new Date(item.Time * 1000).toLocaleString()+ "</div>";
                        }

                        temp += "</div>";
                        temp += "</li>";
                    }

                });
                $("#ulChatMessageContentList").prepend(temp);
                if (is_first == true) {
                    $("#divMessageContentList").mCustomScrollbar("scrollTo", "bottom");
                }
            }

        });
}
//切换通讯录事件
Chat.AddressBookChatItemChangeEvent = function AddressBookChatItemChangeEvent(event) {
    var user = event.data.UserItem;
    var to_user_id = user.AddresserID;
    var to_user_url = user.AddresserUrl;
    var to_user_name = user.AddresserName;
    $(this).addClass("selected").siblings().removeClass("selected");
    Chat.CurrentID = to_user_id;
    $("#ulChatMessageContentList").empty();
    $("#btnSendChatMessage").off("click").on("click", { ToUserID: to_user_id, MessageType: Chat.MessageType.Text }, Chat.SendMessageEvent);
    $("#txtChatMessage").off("keypress").on("keypress", { ToUserID: to_user_id, MessageType: Chat.MessageType.Text }, Chat.SendMessagePressEvent);
    //上传图片事件
    $("#fileImageSend").off("change").on("change", { ChatType: Enum.BusinessType.Moments }, Chat.UploadImageEvent);
    Chat.Sdk.Chat.setReadState({
        touid: to_user_id,
        hasPrefix:false,
        timestamp: Math.ceil(new Date() / 1000),
        success: function (data) {
            if (data.resultText == "SUCCESS") {
                $.each($("div[id^='divOftenUsedAddressBookChatCount'],div[id^='divMyAddressBookChatCount']"), function (index, item) {
                    if ($(item).data("UserID") == to_user_id) {
                        $(item).removeData("UserID").remove();
                        $.each(Chat.RecentAddressBookList, function (index, item) {
                            if (item.contact == to_user_id) {
                                Array.remove(Chat.RecentAddressBookList, item);
                                return false;
                            }
                        });
                       
                    }
                });
                if (Chat.RecentAddressBookList.length == 0) {
                    $("#spnOftenUsedAddressBookChatCount,#spnMyAddressBookChatCount").removeClass("user-dot");
                }
                else if ($("div[id^='divOftenUsedAddressBookChatCount']").length == 0) {
                    $("#spnOftenUsedAddressBookChatCount").removeClass("user-dot");
                }
                else if ($("div[id^='divMyAddressBookChatCount']").length == 0) {
                    $("#spnMyAddressBookChatCount").removeClass("user-dot");
                }
                if (Chat.RecentGroupList.length == 0 && Chat.RecentAddressBookList.length == 0) {
                    $("#spnIMCount").removeClass("user-dot");
                }
            }
        },
        error: function (error) {
            console.log("设置已读失败", error);
        }
    });
    var user = {
        ToUserID: to_user_id,
        ToUserName: to_user_name,
        ToUserUrl: to_user_url
    };

    Chat.GetAddressBookList(user,true);
}
Chat.OffenUsedAddressBookChatChatCloseEvent = function OffenUsedAddressBookChatChatCloseEvent(event) {
    $(event.target).parents("li").remove();
    var index = event.data.Index;
    var length = $(event.target).parents("li").length;
    if ($("#liOftenUsedAddressBookChatItem" + index).hasClass("selected") == true) {
        $("#ulOffenUsedAddressBookChatList>li:eq(0)").addClass("selected");
        $("#liOftenUsedAddressBookChatItem0").trigger("click");
    }
}
//上传图片
Chat.UploadImageEvent = function UploadImageEvent(event) {
    var type = event.data.ChatType;
    $("#fmImageSend").ajaxSubmit({
        url: objPub.FriendsUrl + "service/ChatImageUploadService.ashx?Type=" + type,
        type: "post",
        dataType: "json",
        timeout: 600000,
        success: function (data, textStatus) {
            if (data.result == true) {
                $("#txtChatMessage").insertImage(data.url);
            }
            else {
                $.Alert("上传图片失败");
            }
        },
        error: function (data, status, e) {
            console.log("上传失败，错误信息：" + e);
        }
    });
}