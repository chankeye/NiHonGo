$(function () {

    id = utilsApp.getUrlLastSegment();

    $.ajax({
        type: 'post',
        url: '/Video/DetailInit/' + id,
        success: function (data) {
            if (data.IsSuccess) {
                $("#videoname").text(data.ReturnObject.Name);
                $("#videointroduction").html(data.ReturnObject.Introduction.replace(/\n/g, "<br />"));
                $("#videourl").text(data.ReturnObject.Url).attr("href", data.ReturnObject.Url);
                $("#videophone").text(data.ReturnObject.Phone);
                $("#membercount").text(data.ReturnObject.MemberCount);
                $("#videoaddress").text(data.ReturnObject.Address);
                $("#startyear").text(data.ReturnObject.StartYear);
                $("#createdate").text(data.ReturnObject.CreateDate);
                $("#updatedate").text(data.ReturnObject.UpdateDate);
                if (data.ReturnObject.Photo !== null && data.ReturnObject.Photo !== "")
                    $('#photo').attr('src', "/MediaUpload/VideoPhoto/" + data.ReturnObject.Photo);
                else
                    $('#photo').attr('src', "/Content/Images/Trails.jpg");
            } else {
                alert(utilsApp.toMessage(data.ErrorCode));
                if (history.length > 1)
                    history.back();
                else
                    window.location = '/';
            }
        }
    });

    var pageSize = 10;
    Search(id, 0, pageSize);
});

function ShowVideo() {
    $("#VideoDiv").show();
    $("#VideoLi").addClass("active");
    $("#JobListDiv").hide();
    $("#JobLi").removeClass("active");
}