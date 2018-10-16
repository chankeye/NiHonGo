$(function () {

    id = utilsApp.getUrlLastSegment();

    $.ajax({
        type: 'post',
        url: '/Job/MasterInit/' + id,
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
                $("#tag").text(data.ReturnObject.Tag);
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
                $("#btnBackToCompany").attr("href", "/Company/Master/" + data.ReturnObject.CompanyId);
                if (data.ReturnObject.Photo != null && data.ReturnObject.Photo != "")
                    $('#photo').attr('src', "/MediaUpload/JobPhoto/" + data.ReturnObject.Photo);
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
        window.location = "/Job/Edit/" + id;
    });
});