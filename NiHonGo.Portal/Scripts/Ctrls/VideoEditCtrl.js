$(function () {

    $.ajax({
        type: 'post',
        url: '/Company/EditInit',
        success: function (data) {
            if (data.IsSuccess) {
                $("#companyname").val(data.ReturnObject.Name);
                $("#companyintroduction").val(data.ReturnObject.Introduction);
                $("#companyurl").val(data.ReturnObject.Url);
                $("#companyphone").val(data.ReturnObject.Phone);
                $("#membercount").val(data.ReturnObject.MemberCount);
                $("#companyaddress").val(data.ReturnObject.Address);
                $("#startyear").val(data.ReturnObject.StartYear);
                if (data.ReturnObject.Photo != null && data.ReturnObject.Photo != "")
                    $('#photo').attr('src', "/MediaUpload/CompanyPhoto/" + data.ReturnObject.Photo);
                else
                    $('#photo').attr('src', "/Content/Images/Trails.jpg");
            } else {
                if (data.ErrorCode == "NoAuthentication")
                    window.location = '/';
                else if (data.ErrorCode == "NoCompany") {
                }
                else {
                    alert(utilsApp.toMessage(data.ErrorCode));
                    if (history.length > 1)
                        history.back();
                    else
                        $location.url("/Company/Master");
                }
            }
        }
    });

    $("#btnAdd").click(
        function () {
            var $btn = $("#btnAdd");

            if ($("#commentForm").valid() == false) {
                return;
            }

            $btn.button("loading");

            $.ajax({
                type: 'post',
                url: '/Company/EditSubmit',
                data: {
                    Name: $("#companyname").val(),
                    Url: $("#companyurl").val(),
                    Phone: $("#companyphone").val(),
                    Address: $("#companyaddress").val(),
                    Introduction: $("#companyintroduction").val(),
                    MemberCount: $("#membercount").val(),
                    StartYear: $("#startyear").val(),
                    Photo: $(".dz-image > img").attr("alt")
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        alert(utilsApp.toMessage("Success"));
                        if (history.length > 1)
                            history.back();
                        else
                            window.location = '/Company/Master';
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