$(function () {

    $.ajax({
        type: 'post',
        url: '/Company/MasterInit',
        success: function (data) {
            if (data.IsSuccess) {
                $("#companyname").text(data.ReturnObject.Name);
                $("#companyintroduction").html(data.ReturnObject.Introduction.replace(/\n/g, "<br />"));
                $("#companyurl").text(data.ReturnObject.Url).attr("href", data.ReturnObject.Url);
                $("#companyphone").text(data.ReturnObject.Phone);
                $("#membercount").text(data.ReturnObject.MemberCount);
                $("#companyaddress").text(data.ReturnObject.Address);
                $("#startyear").text(data.ReturnObject.StartYear);
                $("#createdate").text(data.ReturnObject.CreateDate);
                $("#updatedate").text(data.ReturnObject.UpdateDate);
                if (data.ReturnObject.Photo != null && data.ReturnObject.Photo != "")
                    $('#photo').attr('src', "/MediaUpload/CompanyPhoto/" + data.ReturnObject.Photo);
                else
                    $('#photo').attr('src', "/Content/Images/Trails.jpg");
            } else {
                if (data.ErrorCode == "NoAuthentication")
                    window.location = '/';
                else if (data.ErrorCode == "NoCompany")
                    window.location = '/Company/Edit';
                else {
                    alert(utilsApp.toMessage(data.ErrorCode));
                    window.location = '/';
                }
            }
        }
    });

    var pageSize = 10;
    Search(0, pageSize);
});

function ShowCompany() {
    $("#CompanyDiv").show();
    $("#CompanyLi").addClass("active");
    $("#JobListDiv").hide();
    $("#JobLi").removeClass("active");
}

function ShowJob() {
    $("#CompanyDiv").hide();
    $("#CompanyLi").removeClass("active");
    $("#JobListDiv").show();
    $("#JobLi").addClass("active");
}

function GoToJobPage(id) {
    window.location = "/Job/Master/" + id;
}

function Search(index, count) {
    $("#joblist").empty();
    $("#pagination").empty();

    $.ajax({
        type: 'post',
        url: '/Company/GetMasterJobList/',
        data: {
            index: index,
            count: count
        },
        success: function (data) {
            // job list
            var list = "";
            for (i = 0; i < data.List.length; i++) {
                list += '<span class="label label-danger">' + data.List[i].Type + '</span>';
                list += '<div class="jumbotron">';
                if (data.List[i].Photo != null && data.List[i].Photo != "")
                    list += '<img id="photo" name="photo" class="img-rounded img-responsive" style="max-height: 200px;" src="/MediaUpload/JobPhoto/' + data.List[i].Photo + '"/>'
                else
                    list += '<img id="photo" name="photo" class="img-rounded img-responsive" style="max-height: 200px;" src="/Content/Images/Trails.jpg">';
                list += '<h3><a class="text-danger" href="#" OnClick="GoToJobPage(' + data.List[i].Id + ')">' + data.List[i].Name + '</a></h3>';
                list += '<h5><span class="glyphicon glyphicon-time"></span><label>&nbsp;' + data.List[i].UpdateDate + '</label></h5>';
                list += '<p class="text-muted">' + data.List[i].Detail.replace(/\n/g, '<br />') + '</p></div>';
            }
            $("#joblist").append(list);

            // pagination
            if (data.Count > 0) {
                var rangeSize = 10;
                var page = '<li><a  role="button" onclick="Search(0,' + count + ')"><<</a></li>';
                var maxPage = Math.ceil(data.Count / count);
                var pageRange = Math.floor(index / rangeSize);
                var firstPage = pageRange * rangeSize;
                var lastPage = pageRange * rangeSize + rangeSize;
                if (maxPage < lastPage)
                    lastPage = maxPage;

                if (pageRange != 0)
                    page += '<li><a role="button" onclick="Search(' + ((pageRange - 1) * rangeSize) + ',' + count + ')">...</a></li>';

                for (i = firstPage; i < lastPage ; i++) {
                    page += '<li id="btnPage' + i + '" name="btnPage' + i + '"><a role="button" OnClick="Search(' + i + ',' + count + ')">' + (i + 1) + '</a></li>'
                }

                if (Math.floor(maxPage / rangeSize) > pageRange)
                    page += '<li><a role="button" onclick="Search(' + ((pageRange + 1) * rangeSize) + ',' + count + ')">...</a></li>';

                page += '<li><a role="button" onclick="Search(' + (maxPage - 1) + ',' + count + ')">>></a></li>';

                $("#pagination").append(page);
                $("#btnPage" + index).addClass("active");
            }
        }
    });
}