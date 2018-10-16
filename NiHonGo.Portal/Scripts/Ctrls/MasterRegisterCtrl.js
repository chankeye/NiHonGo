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
            password: {
                required: true,
                minlength: 6,
                maxlength: 20
            },
            comPassword: {
                equalTo: "#password"
            },
            display: {
                maxlength: 20
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
            password: {
                required: utilsApp.toMessage("PasswordIsNull"),
                minlength: utilsApp.toMessage("CantSmallThan6Word"),
                maxlength: utilsApp.toMessage("CantBigThan20Word")
            },
            comPassword: utilsApp.toMessage("OldAndNewPWDIsDifferent"),
            display: {
                maxlength: utilsApp.toMessage("CantBigThan20")
            },
            mobile: {
                maxlength: utilsApp.toMessage("CantBigThan20")
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
                url: '/User/MasterRegisterSubmit',
                data: {
                    Name: $("#name").val(),
                    Display: $("#display").val(),
                    Phone: $("#mobile").val(),
                    Email: $("#email").val(),
                    Password: $("#password").val(),
                    IsDisable: false
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
