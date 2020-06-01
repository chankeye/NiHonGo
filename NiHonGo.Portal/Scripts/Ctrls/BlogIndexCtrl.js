$(function () {

    $.ajax({
        type: 'post',
        url: '/Blog/GetCatalogList/',
        success: function (data) {
            var list = '<option value="" selected="selected">記事の種類</option>';
            for (i = 0; i < data.length; i++) {
                list += '<option value="' + data[i].Value + '">' + data[i].Display + '</option>';
            }
            $("#selOptionsCataloges").append(list);
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

function GoToBlogPage(id) {
    window.location = "/Blog/Detail/" + id;
}

function Search(index, count) {
    $("#bloglist").empty();
    $("#pagination").empty();

    $.ajax({
        type: 'post',
        url: '/Blog/GetList/',
        data: {
            Keyword: $("#keyword").val(),
            index: index,
            count: count,
            catalogId: $("#selOptionsCataloges").val()
        },
        success: function (data) {
            // blog list
            var list = "";
            if (data.List.length == 0) {
                list = utilsApp.toMessage("BlogListDefaultMsg");//utilsApp.toMessage("BlogListDefaultMsg");
            }
            for (i = 0; i < data.List.length; i++) {
                list += '<span class="label label-danger">' + data.List[i].Catalog + '</span>';
                list += '<div class="jumbotron">';
                if (data.List[i].Photo != null && data.List[i].Photo != "")
                    list += '<img id="photo" name="photo" class="img-rounded img-responsive" style="max-height: 200px;" src="/MediaUpload/BlogPhoto/' + data.List[i].Photo + '"/>'
                else
                    list += '<img id="photo" name="photo" class="img-rounded img-responsive" style="max-height: 200px;" src="/Content/Images/Trails.jpg">';

                list += '<h3><a class="text-danger" href="#" OnClick="GoToBlogPage(' + data.List[i].Id + ')">' + data.List[i].Title + '</a></h3>';
                list += '<div class="text-muted">' + data.List[i].Detail + '</div></div>';
            }
            $("#bloglist").append(list);
            $("#count").text(data.Count);

            // pagination
            if (data.Count > 0) {
                var rangeSize = 10;
                var page = '<li><a role="button" onclick="Search(0,' + count + ')"><<</a></li>';
                var maxPage = Math.ceil(data.Count / count);
                var pageRange = Math.floor(index / rangeSize);
                var firstPage = pageRange * rangeSize;
                var lastPage = pageRange * rangeSize + rangeSize;
                if (maxPage < lastPage)
                    lastPage = maxPage;

                if (pageRange != 0)
                    page += '<li><a role="button" onclick="Search(' + ((pageRange - 1) * rangeSize) + ',' + count + ')">...</a></li>';

                for (i = firstPage; i < lastPage ; i++) {
                    page += '<li id="btnPage' + i + '" name="btnPage' + i + '"><a  role="button" OnClick="Search(' + i + ',' + count + ')">' + (i + 1) + '</a></li>'
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