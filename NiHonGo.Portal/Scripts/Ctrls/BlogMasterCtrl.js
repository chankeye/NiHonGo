$(function () {

    id = utilsApp.getUrlLastSegment();

    $.ajax({
        type: 'post',
        url: '/Blog/MasterInit/' + id,
        success: function (data) {
            if (data.IsSuccess) {
                $("#blogtitle").text(data.ReturnObject.Title);
                $("#blogdetail").html(data.ReturnObject.Detail);
                $("#tag").text(data.ReturnObject.Tag);
                $("#catalog").text(data.ReturnObject.Catalog);
                $("#createdate").text(data.ReturnObject.CreateDate);
                if (data.ReturnObject.Photo != null && data.ReturnObject.Photo != "")
                    $('#photo').attr('src', "/MediaUpload/BlogPhoto/" + data.ReturnObject.Photo);
                else
                    $('#photo').attr('src', "/Content/Images/Trails.jpg");
            } else {
                if (data.ErrorCode == "NoAuthentication")
                    window.location = '/';
                else {
                    alert(utilsApp.toMessage(data.ErrorCode));
                    if (history.length > 1)
                        history.back();
                    else
                        window.location = '/';
                }
            }
        }
    });

    $("#btnEdit").click(
    function () {
        window.location = "/Blog/Edit/" + id;
    });
});