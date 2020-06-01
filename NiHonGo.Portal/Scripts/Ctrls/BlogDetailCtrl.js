$(function () {

    id = utilsApp.getUrlLastSegment();

    $.ajax({
        type: 'post',
        url: '/Blog/DetailInit/' + id,
        success: function (data) {
            if (data.IsSuccess) {
                $("#blogtitle").text(data.ReturnObject.Title);
                $("#blogdetail").html(data.ReturnObject.Detail);
                $("#createdate").text(data.ReturnObject.CreateDate);
                $("#catalog").text(data.ReturnObject.Catalog);
                if (data.ReturnObject.Photo != null && data.ReturnObject.Photo != "")
                    $('#photo').attr('src', "/MediaUpload/BlogPhoto/" + data.ReturnObject.Photo);
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
});