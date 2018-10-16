$(function () {

    id = utilsApp.getUrlLastSegment();
    if (id.toUpperCase() == "EDIT")
        id = "";

    init(id);

    $("#btnAdd").click(
        function () {
            var $btn = $("#btnAdd");

            if ($("#commentForm").valid() == false) {
                return;
            }

            if ($("#selOptionsTypes").val() == "") {
                alert(utilsApp.toMessage('JobTypeIsNull'));
                return;
            }

            if ($("#selOptionsStatuses").val() == "") {
                alert(utilsApp.toMessage('JobStatusIsNull'));
                return;
            }

            if ($("#selOptionsAreas").val() == "") {
                alert(utilsApp.toMessage('AreaIsNull'));
                return;
            }

            $btn.button("loading");

            $.ajax({
                type: 'post',
                url: '/Job/EditSubmit',
                data: {
                    jobId: id,
                    Name: $("#jobtitle").val(),
                    What: $("#jobwhat").val(),
                    Why: $("#jobwhy").val(),
                    How: $("#jobhow").val(),
                    Detail: $("#jobdetail").val(),
                    TypeId: $("#selOptionsTypes").val(),
                    StatusId: $("#selOptionsStatuses").val(),
                    AreaId: $("#selOptionsAreas").val(),
                    Tag: $("#tag").val(),
                    StartSalary: $("#slarystart").val(),
                    EndSalary: $("#slaryend").val(),
                    Photo: $(".dz-image > img").attr("alt")
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        alert(utilsApp.toMessage("Success"));
                        window.location = '/Job/Master/' + data.ReturnObject;
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

function init(id) {

    var ajax1 = $.ajax({
        type: 'post',
        url: '/System/AreaList/',
        success: function (data) {
            var list = '<option value="" selected="selected">地域</option>';
            for (i = 0; i < data.length; i++) {
                list += '<option value="' + data[i].Value + '">' + data[i].Display + '</option>';
            }
            $("#selOptionsAreas").append(list);
        }
    });

    var ajax2 = $.ajax({
        type: 'post',
        url: '/System/JobTypeList/',
        success: function (data) {
            var list = '<option value="" selected="selected">職種</option>';
            for (i = 0; i < data.length; i++) {
                list += '<option value="' + data[i].Value + '">' + data[i].Display + '</option>';
            }
            $("#selOptionsTypes").append(list);
        }
    });

    var ajax3 = $.ajax({
        type: 'post',
        url: '/System/JobStatusList/',
        async: false,
        success: function (data) {
            var list = '<option value="" selected="selected">募集状態</option>';
            for (i = 0; i < data.length; i++) {
                list += '<option value="' + data[i].Value + '">' + data[i].Display + '</option>';
            }
            $("#selOptionsStatuses").append(list);
        }
    });

    var der = $.when(ajax1, ajax2, ajax3);
    der.done(getData(id));
}

function getData(id) {
    
    $.ajax({
        type: 'post',
        url: '/Job/EditInit/' + id,
        success: function (data) {
            if (data.IsSuccess) {
                $("#jobtitle").val(data.ReturnObject.Name);
                $("#jobwhat").val(data.ReturnObject.What);
                $("#jobwhy").val(data.ReturnObject.Why);
                $("#jobhow").val(data.ReturnObject.How);
                $("#jobdetail").val(data.ReturnObject.Detail);
                $("#tag").val(data.ReturnObject.Tag);
                $("#slarystart").val(data.ReturnObject.StartSalary);
                $("#slaryend").val(data.ReturnObject.EndSalary);
                if (data.ReturnObject.Photo != null && data.ReturnObject.Photo != "")
                    $('#photo').attr('src', "/MediaUpload/JobPhoto/" + data.ReturnObject.Photo);
                else
                    $('#photo').attr('src', "/Content/Images/Trails.jpg");
                $("#selOptionsAreas").children().each(function () {
                    if ($(this).val() == data.ReturnObject.AreaId) {
                        $(this).attr("selected", true);
                        this.selected = true;
                    }
                });
                $("#selOptionsTypes").children().each(function () {
                    if ($(this).val() == data.ReturnObject.StatusId) {
                        $(this).attr("selected", true);
                        this.selected = true;
                    }
                });
                $("#selOptionsStatuses").children().each(function () {
                    if ($(this).val() == data.ReturnObject.TypeId) {
                        $(this).attr("selected", true);
                        this.selected = true;
                    }
                });
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