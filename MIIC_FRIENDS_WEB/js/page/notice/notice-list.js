Notice = function () { }
Notice.registerClass("Notice");
Notice.Init = function init() {
    $("#liMoments").off("click").on("click", Notice.Moments.GetNoticeEvent);
    $("#liGroup").off("click").on("click", Notice.Group.GetNoticeEvent);
    $("#liCommunity").off("click").on("click", Notice.Community.GetNoticeEvent);
 
}




