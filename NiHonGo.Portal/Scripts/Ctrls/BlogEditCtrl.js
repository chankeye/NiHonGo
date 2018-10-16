$(function () {

    id = utilsApp.getUrlLastSegment();
    if (id.toUpperCase() == "EDIT")
        id = "";

    getData(id);

    $("#btnAdd").click(
        function () {
            var $btn = $("#btnAdd");

            if ($("#commentForm").valid() == false) {
                return;
            }

            var oEditor = CKEDITOR.instances["blogdetail"];
            var detail = oEditor.getData();
            if (detail == '') {
                alert(utilsApp.toMessage("BlogDetailIsNull"));
                return;
            }

            $btn.button("loading");

            var data = {
                blogId: id,
                Title: $("#blogtitle").val(),
                Detail: detail,
                Tag: $("#tag").val(),
                Catalog: $("#catalog").val(),
                Photo: $(".dz-image > img").attr("alt")
            };
            var jsonData = JSON.stringify(data);

            $.ajax({
                type: 'post',
                url: '/Blog/EditSubmit',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: jsonData,
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        alert(utilsApp.toMessage("Success"));
                        window.location = '/Blog/Detail/' + data.ReturnObject;
                    } else {
                        if (data.ErrorCode == "NoAuthentication")
                            window.location = '/';
                        else
                            alert(utilsApp.toMessage(data.ErrorCode));
                    }
                }
            });
        });

    $("#btnCancel").click(
    function ($location) {
        if (history.length > 1)
            history.back();
        else
            $location.url("/");
    });
});

function getData(id) {

    $.ajax({
        type: 'post',
        url: '/Blog/EditInit/' + id,
        success: function (data) {
            if (data.IsSuccess) {
                $("#blogtitle").val(data.ReturnObject.Title);
                $("#blogdetail").val(data.ReturnObject.Detail);
                $("#tag").val(data.ReturnObject.Tag);
                $("#catalog").val(data.ReturnObject.Catalog);
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
}