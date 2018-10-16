$(function () {

    id = utilsApp.getUrlLastSegment();

    $.ajax({
        type: 'post',
        url: '/User/ProfileInit/' + id,
        success: function (data) {
            if (data.IsSuccess) {
                $("#email").text(data.ReturnObject.Email);
                $("#name").text(data.ReturnObject.Name);
                $("#display").text(data.ReturnObject.Display);
                $("#visa").text(data.ReturnObject.Visa);
                $("#phone").text(data.ReturnObject.Phone);
                if (data.ReturnObject.Introduction != null) {
                    $("#introduction").html(data.ReturnObject.Introduction.replace(/\n/g, "<br />"));
                }
                $("#age").text(data.ReturnObject.Age);
                $("#createdate").text(data.ReturnObject.CreateDate);
                $("#updatedate").text(data.ReturnObject.UpdateDate);
                if (data.ReturnObject.Photo != null && data.ReturnObject.Photo != "")
                    $('#photo').attr('src', "/MediaUpload/UserPhoto/" + data.ReturnObject.Photo);
                else
                    $('#photo').attr('src', "/Content/Images/Trails.jpg");
                if (data.ReturnObject.IsMarried)
                    $("#ismarried").text(utilsApp.toMessage("Married"));
                else
                    $("#ismarried").text(utilsApp.toMessage("NoMarried"));

                // experience list
                var experiencelist = '<div class="jumbotron">';
                for (i = 0; i < data.ReturnObject.ExperienceList.length; i++) {
                    experiencelist += '<h3><label class="text-danger">' + data.ReturnObject.ExperienceList[i].Name + '</label></h3>';
                    experiencelist += '<p class="text-muted"><label>' + data.ReturnObject.ExperienceList[i].JobTitle + '</label></p>';

                    var startDate = data.ReturnObject.ExperienceList[i].StartDate;
                    var endDate = data.ReturnObject.ExperienceList[i].EndDate;
                    if (startDate != null || endDate != null) {
                        if (startDate == null)
                            startDate = "";
                        if (endDate == null)
                            endDate = "";
                        experiencelist += '<h5><span class="glyphicon glyphicon-time"></span><label>' + startDate;
                        experiencelist += '~' + endDate + '</label></h5>';
                    }

                    var detail = data.ReturnObject.ExperienceList[i].Detail;
                    if (detail != null) {
                        detail = detail.replace(/\n/g, '<br />')
                        experiencelist += '<p class="text-muted"><label>' + detail + '</label></p>';
                    }
                }
                experiencelist += '</div>';
                $("#experiencelist").append(experiencelist);

                // works list
                var workslist = '<div class="jumbotron">';
                for (i = 0; i < data.ReturnObject.WorksList.length; i++) {
                    workslist += '<h3><a class="text-danger">' + data.ReturnObject.WorksList[i].Name + '</a></h3>';

                    var startDate = data.ReturnObject.WorksList[i].StartDate;
                    var endDate = data.ReturnObject.WorksList[i].EndDate;
                    if (startDate != null || endDate != null) {
                        if (startDate == null)
                            startDate = "";
                        if (endDate == null)
                            endDate = "";
                        workslist += '<h5><span class="glyphicon glyphicon-time"></span><label>' + startDate;
                        workslist += '~' + endDate + '</label></h5>';
                    }

                    var detail = data.ReturnObject.WorksList[i].Detail;
                    if (detail != null) {
                        detail = detail.replace(/\n/g, '<br />')
                        workslist += '<p class="text-muted">' + detail + '</p>';
                    }
                }
                workslist += '</div>';
                $("#workslist").append(workslist);

                // school list
                var schoollist = '<div class="jumbotron">';
                for (i = 0; i < data.ReturnObject.SchoolList.length; i++) {
                    schoollist += '<h3><a class="text-danger">' + data.ReturnObject.SchoolList[i].Name + '</a></h3>';
                    schoollist += '<p class="text-muted">' + data.ReturnObject.SchoolList[i].Department + '</p>';

                    var startDate = data.ReturnObject.SchoolList[i].StartDate;
                    var endDate = data.ReturnObject.SchoolList[i].EndDate;
                    if (startDate != null || endDate != null) {
                        if (startDate == null)
                            startDate = "";
                        if (endDate == null)
                            endDate = "";
                        schoollist += '<h5><span class="glyphicon glyphicon-time"></span><label>' + startDate;
                        schoollist += '~' + endDate + '</label></h5>';
                    }

                    var detail = data.ReturnObject.SchoolList[i].Detail;
                    if (detail != null) {
                        detail = detail.replace(/\n/g, '<br />')
                        schoollist += '<p class="text-muted">' + detail + '</p>';
                    }
                }
                schoollist += '</div>';
                $("#schoollist").append(schoollist);

                // skill list
                var skilllist = '<div class="jumbotron">';
                for (i = 0; i < data.ReturnObject.SkillList.length; i++) {
                    skilllist += '<p><label>' + data.ReturnObject.SkillList[i].Display + '</label></p>';
                }
                skilllist += '</div>';
                $("#skilllist").append(skilllist);

                // language list
                var languagelist = '<div class="jumbotron">';
                for (i = 0; i < data.ReturnObject.LanguageList.length; i++) {
                    languagelist += '<p><label>' + data.ReturnObject.LanguageList[i].Display + '</label></p>';
                }
                languagelist += '</div>';
                $("#languagelist").append(languagelist);

                // license list
                var licenselist = '<div class="jumbotron">';
                for (i = 0; i < data.ReturnObject.LicenseList.length; i++) {
                    licenselist += '<p><label>' + data.ReturnObject.LicenseList[i].Display + '</label></p>';
                }
                licenselist += '</div>';
                $("#licenselist").append(licenselist);
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