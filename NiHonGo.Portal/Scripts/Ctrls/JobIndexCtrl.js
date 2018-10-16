$(function () {

    $.ajax({
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

    $.ajax({
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

    var pageSize = 10;
    Search(0, pageSize);

    $("#btnSearch").click(
        function () {
            var $btn = $("#btnSearch");

            $btn.button("loading");
            Search(0, pageSize);
            $btn.button("reset");
        });
});

function GoToJobPage(id) {
    window.location = "/Job/Detail/" + id;
}

function GoToCompanyPage(id) {
    window.location = "/Company/Detail/" + id;
}

function Search(index, count) {
    var typeList = new Array();
    var typeId = $("#selOptionsTypes").val();
    if (typeId != null && typeId != "")
        typeList.push(typeId);

    var areaList = new Array();
    var areaId = $("#selOptionsAreas").val();
    if (areaId != null && areaId != "")
        areaList.push(areaId);

    $("#joblist").empty();
    $("#pagination").empty();

    $.ajax({
        type: 'post',
        url: '/Job/GetList/',
        data: {
            Keyword: $("#keyword").val(),
            TypeIds: typeList,
            AreaIds: areaList,
            index: index,
            count: count
        },
        success: function (data) {
            // job list
            var list = "";
            if (data.List.length == 0) {
                list = utilsApp.toMessage("JobListDefaultMsg");
            }
            for (i = 0; i < data.List.length; i++) {
                list += '<span class="label label-danger">' + data.List[i].Type + '</span>';
                list += '<div class="jumbotron">';
                if (data.List[i].Photo != null && data.List[i].Photo != "")
                    list += '<img id="photo" name="photo" class="img-rounded img-responsive" style="max-height: 200px;" src="/MediaUpload/JobPhoto/' + data.List[i].Photo + '"/>'
                else
                    list += '<img id="photo" name="photo" class="img-rounded img-responsive" style="max-height: 200px;" src="/Content/Images/Trails.jpg">';

                list += '<h3><a class="text-danger" href="#" OnClick="GoToJobPage(' + data.List[i].Id + ')">' + data.List[i].Name + '</a></h3>';
                list += '<h5><span class="glyphicon glyphicon-time"></span><label>&nbsp;' + data.List[i].UpdateDate + '</label></h5>';
                list += '<p class="text-muted">' + data.List[i].Detail.replace(/\n/g, '<br />') + '</p>';
                list += '<div class="well well-sm"><a class="text-danger" href="#" OnClick="GoToCompanyPage(' + data.List[i].CompanyId + ')">' + data.List[i].CompanyName + '</a></div></div>';
            }
            $("#joblist").append(list);
            $("#count").text(data.Count);

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
                    page += '<li><a  role="button" onclick="Search(' + ((pageRange - 1) * rangeSize) + ',' + count + ')">...</a></li>';

                for (i = firstPage; i < lastPage ; i++) {
                    page += '<li id="btnPage' + i + '" name="btnPage' + i + '"><a  role="button" OnClick="Search(' + i + ',' + count + ')">' + (i + 1) + '</a></li>'
                }

                if (Math.floor(maxPage / rangeSize) > pageRange)
                    page += '<li><a  role="button" onclick="Search(' + ((pageRange + 1) * rangeSize) + ',' + count + ')">...</a></li>';

                page += '<li><a  role="button" onclick="Search(' + (maxPage - 1) + ',' + count + ')">>></a></li>';

                $("#pagination").append(page);
                $("#btnPage" + index).addClass("active");
            }
        }
    });
}