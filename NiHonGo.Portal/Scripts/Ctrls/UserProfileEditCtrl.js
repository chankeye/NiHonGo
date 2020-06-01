$(function () {

    $("#commentForm").validate({
        rules: {
            email: {
                required: true,
                email: true,
                maxlength: 50
            },
            name: {
                required: true,
                maxlength: 20
            },
            display: {
                maxlength: 20
            },
            introduction: {
                maxlength: 3000
            },
            mobile: {
                maxlength: 20
            }
        },
        messages: {
            email: {
                required: utilsApp.toMessage("EmailIsNull"),
                email: utilsApp.toMessage("EmailFormatIsWrong"),
                maxlength: utilsApp.toMessage("CantBigThan50Word")
            },
            name: {
                required: utilsApp.toMessage("NameIsNull"),
                maxlength: utilsApp.toMessage("CantBigThan20Word")
            },
            display: {
                maxlength: utilsApp.toMessage("CantBigThan20Word")
            },
            introduction: {
                maxlength: utilsApp.toMessage("CantBigThan3000Word")
            },
            mobile: {
                maxlength: utilsApp.toMessage("CantBigThan20Word")
            }
        }
    });

    $("#experienceCommentForm").validate({
        rules: {
            companyname: {
                required: true,
                maxlength: 50
            },
            jobtitlename: {
                required: true,
                maxlength: 50
            },
            experiencedetail: {
                maxlength: 3000
            }
        },
        messages: {
            companyname: {
                required: utilsApp.toMessage("ExperienceCompanyNameIsNull"),
                maxlength: utilsApp.toMessage("CantBigThan50Word")
            },
            jobtitlename: {
                required: utilsApp.toMessage("ExperienceJobTitleIsNull"),
                maxlength: utilsApp.toMessage("CantBigThan50Word")
            },
            experiencedetail: {
                maxlength: utilsApp.toMessage("CantBigThan3000Word")
            }
        }
    });

    $("#worksCommentForm").validate({
        rules: {
            worksname: {
                required: true,
                maxlength: 50
            },
            worksdetail: {
                maxlength: 3000
            }
        },
        messages: {
            worksname: {
                required: utilsApp.toMessage("WorksNameIsNull"),
                maxlength: utilsApp.toMessage("CantBigThan50Word")
            },
            worksdetail: {
                maxlength: utilsApp.toMessage("CantBigThan3000Word")
            }
        }
    });

    $("#skillCommentForm").validate({
        rules: {
            skill: {
                required: true,
                maxlength: 50
            }
        },
        messages: {
            skill: {
                required: utilsApp.toMessage("SkillNameIsNull"),
                maxlength: utilsApp.toMessage("CantBigThan50Word")
            }
        }
    });

    $("#languageCommentForm").validate({
        rules: {
            language: {
                required: true,
                maxlength: 50
            }
        },
        messages: {
            language: {
                required: utilsApp.toMessage("LanguageNameIsNull"),
                maxlength: utilsApp.toMessage("CantBigThan50Word")
            }
        }
    });

    $("#licenseCommentForm").validate({
        rules: {
            license: {
                required: true,
                maxlength: 50
            }
        },
        messages: {
            license: {
                required: utilsApp.toMessage("LicenseNameIsNull"),
                maxlength: utilsApp.toMessage("CantBigThan50Word")
            }
        }
    });

    $("#schoolCommentForm").validate({
        rules: {
            schoolname: {
                required: true,
                maxlength: 50
            },
            department: {
                maxlength: 50
            },
            schooldetail: {
                maxlength: 3000
            }
        },
        messages: {
            schoolname: {
                required: utilsApp.toMessage("SchoolNameIsNull"),
                maxlength: utilsApp.toMessage("CantBigThan50Word")
            },
            department: {
                maxlength: utilsApp.toMessage("CantBigThan50Word")
            },
            schooldetail: {
                maxlength: utilsApp.toMessage("CantBigThan3000Word")
            }
        }
    });

    var ajax1 = $.ajax({
        type: 'post',
        url: '/System/VisaList/',
        success: function (data) {
            var list = '<option value="" selected="selected">Select VISA</option>';
            for (i = 0; i < data.length; i++) {
                list += '<option value="' + data[i].Value + '">' + data[i].Display + '</option>';
            }
            $("#selOptionsVisas").append(list);
        }
    });

    var der = $.when(ajax1);
    der.done($.ajax({
        type: 'post',
        url: '/User/ProfileEditInit',
        success: function (data) {
            if (data.IsSuccess) {
                $("#email").val(data.ReturnObject.Email);
                $("#name").val(data.ReturnObject.Name);
                $("#display").val(data.ReturnObject.Display);
                $("#mobile").val(data.ReturnObject.Phone);
                $("#introduction").val(data.ReturnObject.Introduction);
                $("#age").val(data.ReturnObject.Age);
                $("#selOptionsVisas").children().each(function () {
                    if ($(this).val() == data.ReturnObject.VisaId) {
                        $(this).attr("selected", true);
                        this.selected = true;
                    }
                });
                if (data.ReturnObject.Photo != null && data.ReturnObject.Photo != "")
                    $('#photo').attr('src', "/MediaUpload/UserPhoto/" + data.ReturnObject.Photo);
                else
                    $('#photo').attr('src', "/Content/Images/Trails.jpg");
                if (data.ReturnObject.IsMarried)
                    $("#ismarried").prop('checked', true);
                else
                    $("#ismarried").prop('checked', false);

                // experiencelist list
                var experiencelist = "";
                for (i = 0; i < data.ReturnObject.ExperienceList.length; i++) {
                    experiencelist += AddExperience(data.ReturnObject.ExperienceList[i])
                }
                $("#experiencelist").append(experiencelist);

                // works list
                var workslist = "";
                for (i = 0; i < data.ReturnObject.WorksList.length; i++) {
                    workslist += AddWorks(data.ReturnObject.WorksList[i]);
                }
                $("#workslist").append(workslist);

                // school list
                var schoollist = "";
                for (i = 0; i < data.ReturnObject.SchoolList.length; i++) {
                    schoollist += AddSchool(data.ReturnObject.SchoolList[i]);
                }
                $("#schoollist").append(schoollist);

                // skill list
                var skilllist = '<div class="jumbotron">';
                for (i = 0; i < data.ReturnObject.SkillList.length; i++) {
                    skilllist += AddSkill(data.ReturnObject.SkillList[i]);
                }
                skilllist += '</div>';
                $("#skilllist").append(skilllist);

                // language list
                var languagelist = '<div class="jumbotron">';
                for (i = 0; i < data.ReturnObject.LanguageList.length; i++) {
                    languagelist += AddLanguage(data.ReturnObject.LanguageList[i]);
                }
                languagelist += '</div>';
                $("#languagelist").append(languagelist);

                // license list
                var licenselist = '<div class="jumbotron">';
                for (i = 0; i < data.ReturnObject.LicenseList.length; i++) {
                    licenselist += AddLicense(data.ReturnObject.LicenseList[i]);
                }
                licenselist += '</div>';
                $("#licenselist").append(licenselist);
            } else {
                alert(utilsApp.toMessage(data.ErrorCode));
                window.location = '/';
            }
        }
    }));

    $("#btnAdd").click(
        function () {
            var $btn = $("#btnAdd");

            if ($("#commentForm").valid() == false) {
                return;
            }

            $btn.button("loading");

            $.ajax({
                type: 'post',
                url: '/User/ProfileEditSubmit',
                data: {
                    Name: $("#name").val(),
                    Display: $("#display").val(),
                    Phone: $("#mobile").val(),
                    Email: $("#email").val(),
                    Introduction: $("#introduction").val(),
                    VisaId: $("#selOptionsVisas").val(),
                    Age: $("#age").val(),
                    IsMarried: $("#ismarried").is(':checked'),
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        alert(utilsApp.toMessage("Success"));
                        window.location = '/User/ProfileEdit';
                    } else {
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

    $("#btnExperienceAdd").click(
        function () {
            var $btn = $("#btnExperienceAdd");

            if ($("#experienceCommentForm").valid() == false) {
                return;
            }

            $btn.button("loading");

            $.ajax({
                type: 'post',
                url: '/User/ExperienceEditSubmit',
                data: {
                    Name: $("#companyname").val(),
                    JobTitle: $("#jobtitlename").val(),
                    Detail: $("#experiencedetail").val(),
                    StartDate: $("#experiencestartdate").val(),
                    EndDate: $("#experienceenddate").val()
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        alert(utilsApp.toMessage("Success"));
                        var object = $("#experiencelist").find($("#experiencename_" + data.ReturnObject.Name.replace(/\s/g, "")))
                        if (object.length == 0) {
                            var experience = AddExperience(data.ReturnObject);
                            $(experience).appendTo($("#experiencelist"));
                        }
                        else {
                            var name = data.ReturnObject.Name;
                            $("#jobtitlename_" + name.replace(/\s/g, "")).text(data.ReturnObject.JobTitle);

                            var startDate = data.ReturnObject.StartDate;
                            var endDate = data.ReturnObject.EndDate;
                            if (startDate == null && endDate == null)
                                $("#experiencedate_" + name.replace(/\s/g, "")).text("");
                            else {
                                if (startDate == null)
                                    startDate = "";
                                if (endDate == null)
                                    endDate = "";
                                $("#experiencedate_" + name.replace(/\s/g, "")).text(startDate + "~" + endDate);
                            }

                            var detail = data.ReturnObject.Detail;
                            if (detail != null) {
                                detail = detail.replace(/\n/g, '<br />');
                            }
                            else {
                                detail = "";
                            }

                            $("#experiencedetail_" + name.replace(/\s/g, "")).html(detail);
                        }

                        $("#companyname").prop('disabled', false);
                        $("#companyname").val("");
                        $("#jobtitlename").val("");
                        $("#experiencestartdate").val("");
                        $("#experienceenddate").val("");
                        $("#experiencedetail").val("");
                    } else {
                        alert(utilsApp.toMessage(data.ErrorCode));
                    }
                }
            });
        });

    $("#btnExperienceCancel").click(
        function ($location) {
            $("#companyname").prop('disabled', false);
            $("#companyname").val("");
            $("#jobtitlename").val("");
            $("#experiencestartdate").val("");
            $("#experienceenddate").val("");
            $("#experiencedetail").val("");
        });

    $("#btnWorksAdd").click(
        function () {
            var $btn = $("#btnWorksAdd");

            if ($("#worksCommentForm").valid() == false) {
                return;
            }

            $btn.button("loading");

            $.ajax({
                type: 'post',
                url: '/User/WorksEditSubmit',
                data: {
                    Name: $("#worksname").val(),
                    Detail: $("#worksdetail").val(),
                    StartDate: $("#worksstartdate").val(),
                    EndDate: $("#worksenddate").val()
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        alert(utilsApp.toMessage("Success"));
                        var object = $("#workslist").find($("#worksname_" + data.ReturnObject.Name.replace(/\s/g, "")))
                        if (object.length == 0) {
                            var works = AddWorks(data.ReturnObject);
                            $(works).appendTo($("#workslist"));
                        }
                        else {
                            var name = data.ReturnObject.Name;

                            var startDate = data.ReturnObject.StartDate;
                            var endDate = data.ReturnObject.EndDate;
                            if (startDate == null && endDate == null)
                                $("#worksdate_" + name.replace(/\s/g, "")).text("");
                            else {
                                if (startDate == null)
                                    startDate = "";
                                if (endDate == null)
                                    endDate = "";
                                $("#worksdate_" + name.replace(/\s/g, "")).text(startDate + "~" + endDate);
                            }

                            var detail = data.ReturnObject.Detail;
                            if (detail != null) {
                                detail = detail.replace(/\n/g, '<br />');
                            }
                            else {
                                detail = "";
                            }

                            $("#worksdetail_" + name.replace(/\s/g, "")).html(detail);
                        }

                        $("#worksname").prop('disabled', false);
                        $("#worksname").val("");
                        $("#worksstartdate").val("");
                        $("#worksenddate").val("");
                        $("#worksdetail").val("");
                    } else {
                        alert(utilsApp.toMessage(data.ErrorCode));
                    }
                }
            });
        });

    $("#btnWorksCancel").click(
        function ($location) {
            $("#worksname").prop('disabled', false);
            $("#worksname").val("");
            $("#worksstartdate").val("");
            $("#worksenddate").val("");
            $("#worksdetail").val("");
        });

    $("#btnSchoolAdd").click(
        function () {
            var $btn = $("#btnSchoolAdd");

            if ($("#schoolCommentForm").valid() == false) {
                return;
            }

            $btn.button("loading");

            $.ajax({
                type: 'post',
                url: '/User/SchoolEditSubmit',
                data: {
                    Name: $("#schoolname").val(),
                    Detail: $("#schooldetail").val(),
                    Department: $("#department").val(),
                    StartDate: $("#schoolstartdate").val(),
                    EndDate: $("#schoolenddate").val()
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        alert(utilsApp.toMessage("Success"));
                        var object = $("#schoollist").find($("#schoolname_" + data.ReturnObject.Name.replace(/\s/g, "")))
                        if (object.length == 0) {
                            var school = AddSchool(data.ReturnObject);
                            $(school).appendTo($("#schoollist"));
                        }
                        else {
                            var name = data.ReturnObject.Name;
                            $("#department_" + name.replace(/\s/g, "")).text(data.ReturnObject.Department);
                            var startDate = data.ReturnObject.StartDate;
                            var endDate = data.ReturnObject.EndDate;
                            if (startDate == null && endDate == null)
                                $("#schooldate_" + name.replace(/\s/g, "")).text("");
                            else {
                                if (startDate == null)
                                    startDate = "";
                                if (endDate == null)
                                    endDate = "";
                                $("#schooldate_" + name.replace(/\s/g, "")).text(startDate + "~" + endDate);
                            }

                            var detail = data.ReturnObject.Detail;
                            if (detail != null) {
                                detail = detail.replace(/\n/g, '<br />');
                            }
                            else {
                                detail = "";
                            }

                            $("#schooldetail_" + name.replace(/\s/g, "")).html(detail);
                        }

                        $("#schoolname").prop('disabled', false);
                        $("#schoolname").val("");
                        $("#department").val("");
                        $("#schoolstartdate").val("");
                        $("#schoolenddate").val("");
                        $("#schooldetail").val("");
                    } else {
                        alert(utilsApp.toMessage(data.ErrorCode));
                    }
                }
            });
        });

    $("#btnSchoolCancel").click(
        function ($location) {
            $("#schoolname").prop('disabled', false);
            $("#schoolname").val("");
            $("#department").val("");
            $("#schoolstartdate").val("");
            $("#schoolenddate").val("");
            $("#schooldetail").val("");
        });

    $("#btnSkillAdd").click(
        function () {
            var $btn = $("#btnSkillAdd");

            if ($("#skillCommentForm").valid() == false) {
                return;
            }

            $btn.button("loading");

            $.ajax({
                type: 'post',
                url: '/User/SkillEditSubmit',
                data: {
                    Display: $("#skill").val(),
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        alert(utilsApp.toMessage("Success"));
                        var object = $("#skilllist div").find($("label:contains('" + data.ReturnObject.Display + "')"))
                        if (object.length == 0) {
                            var skill = AddSkill(data.ReturnObject);
                            $(skill).appendTo($("#skilllist div"));
                        }
                        $("#skill").val("");
                    } else {
                        alert(utilsApp.toMessage(data.ErrorCode));
                    }
                }
            });
        });

    $("#btnLanguageAdd").click(
        function () {
            var $btn = $("#btnLanguageAdd");

            if ($("#languageCommentForm").valid() == false) {
                return;
            }

            $btn.button("loading");

            $.ajax({
                type: 'post',
                url: '/User/LanguageEditSubmit',
                data: {
                    Display: $("#language").val(),
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        alert(utilsApp.toMessage("Success"));
                        var object = $("#languagelist div").find($("label:contains('" + data.ReturnObject.Display + "')"))
                        if (object.length == 0) {
                            var language = AddLanguage(data.ReturnObject);
                            $(language).appendTo($("#languagelist div"));
                        }
                        $("#language").val("");
                    } else {
                        alert(utilsApp.toMessage(data.ErrorCode));
                    }
                }
            });
        });

    $("#btnLicenseAdd").click(
        function () {
            var $btn = $("#btnLicenseAdd");

            if ($("#licenseCommentForm").valid() == false) {
                return;
            }

            $btn.button("loading");

            $.ajax({
                type: 'post',
                url: '/User/LicenseEditSubmit',
                data: {
                    Display: $("#license").val(),
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        alert(utilsApp.toMessage("Success"));
                        var object = $("#licenselist div").find($("label:contains('" + data.ReturnObject.Display + "')"))
                        if (object.length == 0) {
                            var license = AddLicense(data.ReturnObject);
                            $(license).appendTo($("#licenselist div"));
                        }
                        $("#license").val("");
                    } else {
                        alert(utilsApp.toMessage(data.ErrorCode));
                    }
                }
            });
        });
});

function AddExperience(experience) {
    var experiencelist = "";
    var name = experience.Name;

    experiencelist += '<div class="navbar-right">';
    experiencelist += '<a OnClick="EditExperience(\'' + name + '\')" class="btn btn-danger">Edit</a>&nbsp;&nbsp;';
    experiencelist += '<a OnClick="DeleteExperience(\'' + name + '\')" class="btn btn-default">Delete</a></div>';

    experiencelist += '<div class="jumbotron">';
    experiencelist += '<h3><label class="text-danger" id="experiencename_' + name.replace(/\s/g, "") + '">' + name + '</label></h3>';
    experiencelist += '<p class="text-muted"><label id="jobtitlename_' + name.replace(/\s/g, "") + '">' + experience.JobTitle + '</label></p>';

    var startDate = experience.StartDate;
    var endDate = experience.EndDate;
    if (startDate == null)
        startDate = "";
    if (endDate == null)
        endDate = "";
    experiencelist += '<h5>';
    if (startDate != "" || endDate != "")
        experiencelist += '<span class="glyphicon glyphicon-time"></span>'
    experiencelist += '<label id="experiencedate_' + name.replace(/\s/g, "") + '">' + startDate;
    if (startDate != "" || endDate != "")
        experiencelist += '~';
    experiencelist += endDate + '</label></h5>';

    var detail = experience.Detail;
    if (detail != null)
        detail = detail.replace(/\n/g, '<br />');
    else
        detail = "";
    experiencelist += '<p class="text-muted"><label id="experiencedetail_' + name.replace(/\s/g, "") + '">' + detail + '</label></p>';
    experiencelist += '</div>';

    return experiencelist;
}

function EditExperience(name) {
    $("#companyname").prop('disabled', true);
    $("#companyname").val(name);
    $("#jobtitlename").focus();
    $("#jobtitlename").val($("#jobtitlename_" + name.replace(/\s/g, "")).text());

    var dates = $("#experiencedate_" + name.replace(/\s/g, "")).text().split('~');
    $("#experiencestartdate").val(dates[0]);
    $("#experienceenddate").val(dates[1]);

    $("#experiencedetail").html($("#experiencedetail_" + name.replace(/\s/g, "")).html().replace(/<br\s*[\/]?>/gi, "\n"));
}

function DeleteExperience(name) {
    var r = confirm(utilsApp.toMessage("ConfirmDelete"));
    if (r == true) {
        $.ajax({
            type: 'post',
            url: '/User/DeleteExperience',
            data: {
                Name: name,
            },
            success: function (data) {
                if (data.IsSuccess) {
                    alert(utilsApp.toMessage("Success"));
                    window.location = '/User/ProfileEdit';
                } else {
                    alert(utilsApp.toMessage(data.ErrorCode));
                }
            }
        });
    }
}

function AddWorks(works) {
    var workslist = "";
    var name = works.Name;

    workslist += '<div class="navbar-right">';
    workslist += '<a OnClick="EditWorks(\'' + name + '\')" class="btn btn-danger">Edit</a>&nbsp;&nbsp;';
    workslist += '<a OnClick="DeleteWorks(\'' + name + '\')" class="btn btn-default">Delete</a></div>';

    workslist += '<div class="jumbotron">';
    workslist += '<h3><a class="text-danger" id="worksname_' + name.replace(/\s/g, "") + '">' + name + '</a></h3>';

    var startDate = works.StartDate;
    var endDate = works.EndDate;
    if (startDate == null)
        startDate = "";
    if (endDate == null)
        endDate = "";
    workslist += '<h5>';
    if (startDate != "" || endDate != "")
        workslist += '<span class="glyphicon glyphicon-time"></span>';
    workslist += '<label id="worksdate_' + name.replace(/\s/g, "") + '">' + startDate;
    if (startDate != "" || endDate != "")
        workslist += '~';
    workslist += endDate + '</label></h5>';

    var detail = works.Detail;
    if (detail != null)
        detail = detail.replace(/\n/g, '<br />')
    else
        detail = "";
    workslist += '<p class="text-muted" id="worksdetail_' + name.replace(/\s/g, "") + '">' + detail + '</p>';
    workslist += '</div>';

    return workslist;
}

function EditWorks(name) {
    $("#worksname").prop('disabled', true);
    $("#worksname").val(name);
    $("#worksdetail").focus();

    var dates = $("#worksdate_" + name.replace(/\s/g, "")).text().split('~');
    $("#worksstartdate").val(dates[0]);
    $("#worksenddate").val(dates[1]);

    $("#worksdetail").html($("#worksdetail_" + name.replace(/\s/g, "")).html().replace(/<br\s*[\/]?>/gi, "\n"));
}

function DeleteWorks(name) {
    var r = confirm(utilsApp.toMessage("ConfirmDelete"));
    if (r == true) {
        $.ajax({
            type: 'post',
            url: '/User/DeleteWorks',
            data: {
                Name: name,
            },
            success: function (data) {
                if (data.IsSuccess) {
                    alert(utilsApp.toMessage("Success"));
                    window.location = '/User/ProfileEdit';
                } else {
                    alert(utilsApp.toMessage(data.ErrorCode));
                }
            }
        });
    }
}

function AddSchool(school) {
    var schoollist = "";
    var name = school.Name;

    schoollist += '<div class="navbar-right">';
    schoollist += '<a OnClick="EditSchool(\'' + name + '\')" class="btn btn-danger">Edit</a>&nbsp;&nbsp;';
    schoollist += '<a OnClick="DeleteSchool(\'' + name + '\')" class="btn btn-default">Delete</a></div>';

    schoollist += '<div class="jumbotron">';
    schoollist += '<h3><a class="text-danger" id="schoolname_' + name.replace(/\s/g, "") + '">' + name + '</a></h3>';
    schoollist += '<p class="text-muted" id="department_' + name.replace(/\s/g, "") + '">' + school.Department + '</p>';

    var startDate = school.StartDate;
    var endDate = school.EndDate;
    if (startDate == null)
        startDate = "";
    if (endDate == null)
        endDate = "";
    schoollist += '<h5>';
    if (startDate != "" || endDate != "")
        schoollist += '<span class="glyphicon glyphicon-time"></span>';
    schoollist += '<label id="schooldate_' + name.replace(/\s/g, "") + '">' + startDate;
    if (startDate != "" || endDate != "")
        schoollist += '~';
    schoollist += endDate + '</label></h5>';

    var detail = school.Detail;
    if (detail != null)
        detail = detail.replace(/\n/g, '<br />')
    else
        detail = "";
    schoollist += '<p class="text-muted" id="schooldetail_' + name.replace(/\s/g, "") + '">' + detail + '</p>';
    schoollist += '</div>';

    return schoollist;
}

function EditSchool(name) {
    $("#schoolname").prop('disabled', true);
    $("#schoolname").val(name);
    $("#department").val($("#department_" + name.replace(/\s/g, "")).text());
    $("#department").focus();

    var dates = $("#schooldate_" + name.replace(/\s/g, "")).text().split('~');
    $("#schoolstartdate").val(dates[0]);
    $("#schoolenddate").val(dates[1]);

    $("#schooldetail").html($("#schooldetail_" + name.replace(/\s/g, "")).html().replace(/<br\s*[\/]?>/gi, "\n"));
}

function DeleteSchool(name) {
    var r = confirm(utilsApp.toMessage("ConfirmDelete"));
    if (r == true) {
        $.ajax({
            type: 'post',
            url: '/User/DeleteSchool',
            data: {
                Name: name,
            },
            success: function (data) {
                if (data.IsSuccess) {
                    alert(utilsApp.toMessage("Success"));
                    window.location = '/User/ProfileEdit';
                } else {
                    alert(utilsApp.toMessage(data.ErrorCode));
                }
            }
        });
    }
}

function AddSkill(skill) {
    var skilllist = "";
    skilllist += '<p><a class="btn btn-default" OnClick="DeleteSkill(\'' + skill.Id + '\')">Delete</a>&nbsp;&nbsp;';
    skilllist += '<label>' + skill.Display + '</label></p>';

    return skilllist;
}

function DeleteSkill(id) {
    var r = confirm(utilsApp.toMessage("ConfirmDelete"));
    if (r == true) {
        $.ajax({
            type: 'post',
            url: '/User/DeleteSkill',
            data: {
                SkillId: id,
            },
            success: function (data) {
                if (data.IsSuccess) {
                    alert(utilsApp.toMessage("Success"));
                    window.location = '/User/ProfileEdit';
                } else {
                    alert(utilsApp.toMessage(data.ErrorCode));
                }
            }
        });
    }
}

function AddLanguage(language) {
    var languagelist = "";
    languagelist += '<p><a class="btn btn-default" OnClick="DeleteLanguage(\'' + language.Id + '\')">Delete</a>&nbsp;&nbsp;';
    languagelist += '<label>' + language.Display + '</label></p>';

    return languagelist;
}
function DeleteLanguage(id) {
    var r = confirm(utilsApp.toMessage("ConfirmDelete"));
    if (r == true) {
        $.ajax({
            type: 'post',
            url: '/User/DeleteLanguage',
            data: {
                LanguageId: id,
            },
            success: function (data) {
                if (data.IsSuccess) {
                    alert(utilsApp.toMessage("Success"));
                    window.location = '/User/ProfileEdit';
                } else {
                    alert(utilsApp.toMessage(data.ErrorCode));
                }
            }
        });
    }
}

function AddLicense(license) {
    var licenselist = "";
    licenselist += '<p><a class="btn btn-default" OnClick="DeleteLicense(\'' + license.Id + '\')">Delete</a>&nbsp;&nbsp;';
    licenselist += '<label>' + license.Display + '</label></p>';

    return licenselist;
}

function DeleteLicense(id) {
    var r = confirm(utilsApp.toMessage("ConfirmDelete"));
    if (r == true) {
        $.ajax({
            type: 'post',
            url: '/User/DeleteLicense',
            data: {
                LicenseId: id,
            },
            success: function (data) {
                if (data.IsSuccess) {
                    alert(utilsApp.toMessage("Success"));
                    window.location = '/User/ProfileEdit';
                } else {
                    alert(utilsApp.toMessage(data.ErrorCode));
                }
            }
        });
    }
}