Group.Trends = function () {
//主页讨论组业务逻辑
}
Group.Trends.registerClass("Group.Trends");
Group.Trends.PageSize = 10;
Group.Trends.CurrentIndex = 0;
Group.Trends.CanPageLoad = false;
Group.Trends.TotalCount = 0;
Group.Trends.Search = function search(keyword, page) {
    Group.Trends.SearchBind(keyword, page);
    $.SimpleAjaxPost("service/GroupService.asmx/GetTrendsSearchCount", true,
        "{searchView:" + $.Serialize(keyword) + "}",
         function (json) {
             Group.Trends.TotalCount = json.d;
             if (Group.Trends.TotalCount <= Group.Trends.PageSize) {
                 Group.Trends.CanPageLoad = false;
             }
             else {
                 if (Group.Trends.CurrentIndex == 0) {
                     Group.Trends.CanPageLoad = true;
                 }
             }
             $(document).off("scroll").on("scroll", {Keyword:keyword},Group.Trends.ScrollEvent);
         });
}
//滑轮事件
Group.Trends.ScrollEvent = function ScrollEvent(event) {
    var keyword = event.data.Keyword;
    if (Group.Trends.CanPageLoad == true) {
        if ($(document).scrollTop() >= $(document).height() - $(window).height()) {
            Group.Trends.CurrentIndex = Group.Trends.CurrentIndex + 1;
            var page = {
                pageStart: Group.Trends.CurrentIndex * Group.Trends.PageSize + 1,
                pageEnd: (Group.Trends.CurrentIndex + 1) * Group.Trends.PageSize
            };
            Group.Trends.SearchBind(keyword, page);
            if (page.pageEnd >= Group.Trends.TotalCount) {
                Group.Trends.CanPageLoad = false;
            }
        }
    }
    else if (parseInt(Group.Trends.TotalCount / Group.Trends.PageSize) > objPub.MinTipPage) {
        $.Alert("这已经是最后一页了哦~");
        $(document).off("scroll");
        setTimeout(function () {
            $(".dialog-normal").dialog('close');
        }, 2000);
    }
}
Group.Trends.SearchBind = function search_bind(keyword, page) {
    $.SimpleAjaxPost("service/GroupService.asmx/TrendsSearch", true,
        "{searchView:" + $.Serialize(keyword) + ",page:" + $.Serialize(page) + "}",
      function (json) {
          var result = $.Deserialize(json.d);
          var temp = "";
          if (result != null) {
              $.each(result, function (index, item) {
                  var Index = (Group.Trends.CurrentIndex + 1) * Group.Trends.PageSize + index;
                  temp += "<div class='friend-group'>";
                  temp += "<div class='figure'><span class='icon-optSet icon-img icon-triangle-yellow'></span></div>";
                  temp += "<div class='circle-headline'>";
                  temp += "<div id='divGroup" + Index + "' class='circle-user-name'>";
                  temp += "<span>" + item.Name + "</span>";
                  temp += "<span class='group-num'>" + item.MemberCount + "人</span>";
                  temp += "</div>";
                  temp += "</div>";
                  temp += "<ul>";
                  $.each(item.TopicInfo, function (sub_index, sub_item) {
                      temp += "<li class='clear-fix'>";
                      $(document).off("click", "#divShowTreadListUser" + index + "_" + sub_index);
                      if (sub_item.CreaterID != window.objPub.UserID && sub_item.IsFriend == Enum.YesNo.No.toString()) {
                          temp += "<div class='circle-photo' id='divShowTreadListUser" + index + "_" + sub_index + "' style='cursor:default'> <img src='" + sub_item.CreaterUrl + "'/></div>";
                      } else {
                          temp += "<div class='circle-photo' id='divShowTreadListUser" + index + "_" + sub_index + "' style='cursor:pointer'> <img src='" + sub_item.CreaterUrl + "'/></div>";
                          $(document).on("click", "#divShowTreadListUser" + index + "_" + sub_index, { UserID: sub_item.CreaterID }, Group.Trends.ShowDetailUserEvent);
                      }
                      temp += "<div class='circle-content'>";
                      temp += "<h4>" + sub_item.Content + "</h4>";
                      if (sub_item.CreateTime.format("yyyy-MM-dd") == new Date().format("yyyy-MM-dd")) {
                          temp += "<div class='circle-content-time'>今天　" + sub_item.CreateTime.format("HH:mm") + "</div>";
                      }
                      else {
                          temp += "<div class='circle-content-time'>" + sub_item.CreateTime.format("yyyy-MM-dd") + "</div>";
                      }
                      temp += "<div class='circle-content-info'>";
                      temp += "<span class='info-number'>" + sub_item.MessageCount + "</span>";
                      temp += "<span>条讨论</span>";
                      temp += "</div>";
                      temp += "</div>";
                      temp += "</li>";
                  });
                  temp += "</ul>";
                  temp += "</div>";
                  $(document).off("click", "#divGroup" + Index);
                  $(document).on("click", "#divGroup" + Index, { ID: item.ID, Name: item.Name,Manager:item.Manager }, Group.ShowGroupEvent);
              });
              $("#divTabList").append(temp);
          }
          else {
              temp += "<div class='friend-list-empty'><span>暂没有讨论组最新动态哦~</span></div>";
              $("#divTabList").empty().append(temp);
          }
      });
}

Group.Trends.ShowDetailUserEvent = function ShowDetailUserEvent(event) {
    var user_id = event.data.UserID;
    $(".main-left").load("../biz/left/moments.html", function (response, status) {
        if (status == "success") {
            objPub.IsMain = true;
            Moments.List.Person.Init(user_id);
        }
    });
}