$(function () {

    id = utilsApp.getUrlLastSegment();

    $.ajax({
        type: 'post',
        url: '/Job/DetailInit/' + id,
        success: function (data) {
            if (data.IsSuccess) {
                $("#jobtitle").text(data.ReturnObject.Name);
                $("#jobwhat").html(data.ReturnObject.What.replace(/\n/g, "<br />"));
                $("#jobwhy").html(data.ReturnObject.Why.replace(/\n/g, "<br />"));
                $("#jobhow").html(data.ReturnObject.How.replace(/\n/g, "<br />"));
                $("#jobdetail").html(data.ReturnObject.Detail.replace(/\n/g, "<br />"));
                $("#jobtype").text(data.ReturnObject.Type);
                $("#jobstatus").text(data.ReturnObject.Status);
                $("#jobarea").text(data.ReturnObject.Area);
                $("#company").text(data.ReturnObject.CompanyName).attr('href', "/Company/Detail/" + data.ReturnObject.CompanyId);
                if (data.ReturnObject.StartSalary != null || data.ReturnObject.EndSalary != null) {
                    var start = data.ReturnObject.StartSalary;
                    if (start == null)
                        start = "";
                    var end = data.ReturnObject.EndSalary;
                    if (end == null)
                        end = "";
                    $("#slary").text(data.ReturnObject.StartSalary + " ~ " + data.ReturnObject.EndSalary + " 円");
                }
                $("#createdate").text(data.ReturnObject.CreateDate);
                $("#updatedate").text(data.ReturnObject.UpdateDate);
                if (data.ReturnObject.Photo != null && data.ReturnObject.Photo != "")
                    $('#photo').attr('src', "/MediaUpload/JobPhoto/" + data.ReturnObject.Photo);
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

    $("#btnApply").click(
        function () {
            var $btn = $("#btnApply");
            $btn.button("loading");

            $.ajax({
                type: 'post',
                url: '/Job/Apply',
                data: {
                    id: id
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        if (confirm(utilsApp.toMessage("ApplySuccess"))) {
                            window.location = "/User/ProfileEdit";
                        } 
                    } else {
                        alert(utilsApp.toMessage("SendMailFailed"));
                    }
                }
            });
        });
});