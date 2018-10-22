$(function () {
    var levels = [];
    $.ajax({
        type: 'post',
        url: '/System/LevelList',
        success: function (data) {
            // level list
            var list = "";
            for (i = 0; i < data.length; i++) {
                list += '<label class="checkbox-inline"><input id="level' + data[i].Value +
                    '" name="' + data[i].Value + '" type="checkbox" value="' + data[i].Value + '">' +
                    data[i].Display + '</label>';

                levels.push({ Id: data[i].Value, Display: data[i].Display });
            }
            $("#levelList").append(list);
        }
    });

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
            password: {
                required: true,
                minlength: 6,
                maxlength: 20
            },
            comPassword: {
                equalTo: "#password"
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
            password: {
                required: utilsApp.toMessage("PasswordIsNull"),
                minlength: utilsApp.toMessage("CantSmallThan6Word"),
                maxlength: utilsApp.toMessage("CantBigThan20Word")
            },
            comPassword: utilsApp.toMessage("OldAndNewPWDIsDifferent")
        }
    });

    $("#btnAdd").click(
        function () {
            var $btn = $("#btnAdd");

            if ($("#commentForm").valid() === false) {
                return;
            }

            $btn.button("loading");

            var selectedlevels = [];
            for (var i = 0; i < levels.length; i++) {
                if ($('#level' + levels[i].Id).is(':checked')) {
                    selectedlevels.push(levels[i]);
                }
            }

            $.ajax({
                type: 'post',
                url: '/User/RegisterSubmit',
                data: {
                    Name: $("#name").val(),
                    Email: $("#email").val(),
                    Password: $("#password").val(),
                    Levels: selectedlevels
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess) {
                        alert(utilsApp.toMessage("Success"));
                        window.location = '/User/Login';
                    } else {
                        alert(utilsApp.toMessage(data.ErrorCode));
                    }
                }
            });
        });
});