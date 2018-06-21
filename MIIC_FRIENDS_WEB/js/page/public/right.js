Right = function () { }
Right.registerClass("Right");
Right.OffenUsedAddressBookCount = 14;
Right.MyGroupShowCount = 8;
Right.MyCommunityShowCount = 8;
Right.CommunityRecommendShowCount = 8;
//右侧悬停全局变量
Right.ScrollTopNum = 0;
//初始化
Right.Init = function () {
    $("#liFriends").on("click", function (event) {
        //初始化首页面
        objPub.InitLeftMain(true);
    });
    objPub.GetServiceList(objPub.UserType == Enum.UserType.Administrator.toString() ? true : false).done(function (json) {
        var result = $.Deserialize(json.d);
        if (result != null) {
            var temp = "";
            $("#ulServiceList li:not(:first)").remove();
            $.each(result, function (index, item) {
                temp += "<li id='liServiceItem" + index + "'><img src='"+item.LogoUrl+"' alt='"+item.ShowName+"' title='"+item.Remark+"' /><div>" + item.ShowName + "</div></li>";
                $(document).off("click", "#liServiceItem" + index);
                $(document).on("click", "#liServiceItem" + index, function (event) {
                    window.open(item.ServiceUrl, "_blank");
                });

            });
           
            $("#liFriends").after(temp);
        }
    });
    //通讯录更多
    $("#aAddressbookMore").on("click", Right.ShowMoreAddressBookEvent);
    //所在行业圈子更多
    $("#aCommunityMore").on("click", Right.ShowMoreCommunityEvent);
    //所在讨论组更多
    $("#aGroupMore").on("click", Right.ShowMoreGroupEvent);
    //推荐圈子更多
    $("#aRecommendCommunityMore").on("click", Right.ShowMoreRecommendCommunityEvent);
    Right.OftenUsedAddressBookBind();
    Right.MyGroupBind();
    Right.MyCommunityBind();
    Right.CommunityRecommendBind(); 
   

}
//更多通讯录加载
Right.ShowMoreAddressBookEvent = function ShowMoreAddressBookEvent(event) {
    $(".main-left").load("../biz/left/addressbook/list.html", function (response, status) {
        if (status == "success") {
            AddressBook.Init();
        }
    });
}
//更多讨论组加载
Right.ShowMoreGroupEvent = function ShowMoreGroupEvent(event) {
    $(".main-left").load("../biz/left/group/manage-list.html", function (response, status) {
        if (status == "success") {
            Group.Manage.Init();
        }
    });
}
//更多行业圈子加载
Right.ShowMoreCommunityEvent = function ShowMoreCommunityEvent(event) {
    $(".main-left").load("../biz/left/community/manage-list.html",function (response, status) {
        if (status == "success") {
            Community.Manage.Init(1);
        }
    });
}
//更多热门行业圈子加载
Right.ShowMoreRecommendCommunityEvent = function ShowMoreRecommendCommunityEvent(event) {
    $(".main-left").load("../biz/left/community/manage-list.html", function (response, status) {
        if (status == "success") {
            Community.Manage.Init(2);
        }
    });
}
//常用通讯录绑定
Right.OftenUsedAddressBookBind = function offen_used_address_book_bind() {
    $.SimpleAjaxPost("service/AddressBookService.asmx/ShowMyOftenUsedAddressBookList",true,
   "{top:" + Right.OffenUsedAddressBookCount + "}",
   function (json) {
       var result = $.Deserialize(json.d);
       var temp = "";
       if (result == null) {
           temp = " <div class='group-nocontent'>暂没有联系人... </div>";
       }
       else {
           temp += " <ul class='clear-fix'>";
           $.each(result, function (index, item) {
               temp += " <li> <img id='imgAddresser"+index+"' src='" + item.AddresserUrl + "' title='" + item.AddresserName + "' /></li>";
               $(document).off("click", "#imgAddresser" + index);
               $(document).on("click", "#imgAddresser" + index, { UserID: item.AddresserID}, Right.ShowOftenUsedAddresserEvent);
           });
           temp += "</ul>";
       }
       $("#divOffenUsedAddressBookList").empty().append(temp);
   });
}
//看通讯录联系人
Right.ShowOftenUsedAddresserEvent = function ShowOftenUsedAddresserEvent(event) {
    var user_id = event.data.UserID;
    $(".main-left").load("../biz/left/moments.html", function (response, status) {
        if (status == "success") {
            objPub.IsMain = true;
            Moments.List.Person.Init(user_id);
        }
    });
}
//我的讨论组绑定
Right.MyGroupBind = function my_group_bind() {
    $.SimpleAjaxPost("service/GroupService.asmx/ShowMyGroupInfoList", true,
     "{top:" + Right.MyGroupShowCount + "}",
     function (json) {
         var result = $.Deserialize(json.d);
         var temp = "";
         if (result == null) {
             temp+="<div class='group-nocontent'>暂没有讨论组...</div>";
         }
         else {
             temp += " <ul class='clear-fix'>";
             $.each(result, function (index, item) {
                 temp += " <li id='liMyGroup" + index + "' title='" + item.Name + "'><div class='right-discuss-cover'><img src='" + item.LogoUrl + "'></div><div class='right-cycle-name' title='" + item.Name + "'>" + (item.Remark != "" ? item.Remark : item.Name) + "</div></li>";
                 $(document).off("click", "#liMyGroup" + index);
                 $(document).on("click", "#liMyGroup" + index, { ID: item.ID, Name: item.Name }, function (event) {
                     var group_id = event.data.ID;
                     var group_name = event.data.Name;
                     $(".main-left").load("../biz/left/group/detail-list.html", function (response, status) {
                         if (status == "success") {
                             $("#divGroupName").text(group_name);
                             Group.List.Init(group_id);
                         }
                     });
                 });
             });
             temp += "</ul>";
         }
         $("#divMyGroupList").empty().append(temp);
     });
    
}
//我的行业圈子绑定
Right.MyCommunityBind = function my_community_bind() {
    $.SimpleAjaxPost("service/CommunityService.asmx/ShowMyCommunityInfoList", true,
      "{top:" + Right.MyCommunityShowCount + "}",
          function (json) {
              var result = $.Deserialize(json.d);
              var temp = "";
              if (result == null) {
                  temp += "<div class='group-nocontent'>暂没有所在行业圈子...</div>";
              }
              else {
                  temp += " <ul class='clear-fix'>";
                  $.each(result, function (index, item) {
                      temp += " <li id='liMyCommunity" + index + "' title='" + item.Name + "'><div class='right-user-cover'><img src='" + item.LogoUrl + "'></div><div class='right-cycle-name'>" + item.Name + "</div></li>";
                      $(document).off("click", "#liMyCommunity" + index);
                      $(document).on("click", "#liMyCommunity" + index, { ID: item.ID ,Name:item.Name}, function (event) {
                          var community_id = event.data.ID;
                          var community_name = event.data.Name;
                          $(".main-left").ReadTemplate(Template.DetailCommunityTpl, function () {
                              $("#divCommunityName").text(community_name);
                              Community.Label.Init(community_id);
                          });
                      });
                  });
                  temp += "</ul>";
              }
              $("#divMyCommunityList").empty().append(temp);
          });
}
//热门行业圈子绑定
Right.CommunityRecommendBind = function community_recommend_bind() {
    $.SimpleAjaxPost("service/CommunityService.asmx/ShowCommunityRecommendInfoList", true,
     "{top:" + Right.CommunityRecommendShowCount + "}",
         function (json) {
             var result = $.Deserialize(json.d);
             var temp = "";
             if (result == null) {
                 temp += "<div class='group-nocontent'>暂没有行业圈子推荐...</div>";
             }
             else {
                 temp += " <ul class='clear-fix'>";
                 $.each(result, function (index, item) {
                     temp += " <li id='liRecommendCommunity" + index + "' title='" + item.Name + "'><div class='right-user-cover'><img src='" + item.LogoUrl + "'></div><div class='right-cycle-name'>" + item.Name + "</div></li>";
                     $(document).off("click", "#liRecommendCommunity" + index);
                     $(document).on("click", "#liRecommendCommunity" + index, { Name: item.Name }, function (event) {
                         var community_name = event.data.Name;
                         $(".main-left").load("../biz/left/community/manage-list.html", function (response, status) {
                             if (status == "success") {
                                 $("#txtKeyword").val(community_name)
                                 Community.Manage.Init(2);
                             }
                         });
                        
                     });
                 });
                 temp += "</ul>";
             }
             $("#divCommunityRecommendList").empty().append(temp);
             Right.ScrollTopNum = $("#sctHotCommunity").position().top;

         });
   
}