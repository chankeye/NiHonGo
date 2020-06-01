$(document).ready(function () {

    $("#commentForm").validate({
        rules: {
            oldPwd: {
                required: true,
                minlength: 6,
                maxlength: 20
            },
            newPwd: {
                required: true,
                minlength: 6,
                maxlength: 20
            },
            comPwd: {
                equalTo: "#newPwd"
            }
        },
        messages: {
            oldPwd: {
                required: utilsApp.toMessage("OldPasswordIsNull"),
                minlength: utilsApp.toMessage("CantSmallThan6Word"),
                maxlength: utilsApp.toMessage("CantBigThan20Word")
            },
            newPwd: {
                required: utilsApp.toMessage("NewPasswordIsNull"),
                minlength: utilsApp.toMessage("CantSmallThan6Word"),
                maxlength: utilsApp.toMessage("CantBigThan20Word")
            },
            comPwd: utilsApp.toMessage("NewAndConfirmPWDIsDifferent")
        }
    });

    $("#btnConfirm").click(
        function () {
            var $btn = $("#btnConfirm");
            var $oldPWD = $("#oldPwd");
            var $newPWD = $("#newPwd");
            var $comPWD = $("#comPwd");

            if ($("#commentForm").valid() == false) {
                return;
            }

            if ($comPWD.val() != $newPWD.val()) {
                alert(utilsApp.toMessage("NewAndConfirmPWDIsDifferent"));
                return;
            }

            if ($oldPWD.val() == $newPWD.val()) {
                alert(utilsApp.toMessage("OldAndNewPWDIsTheSame"));
                return;
            }

            $btn.button("loading");

            $.ajax({
                type: 'post',
                url: '/User/ChangePasswordSubmit',
                data: {
                    oldPassword: $("#oldPwd").val(),
                    newPassword: $("#newPwd").val()
                },
                success: function (data) {
                    $btn.button("reset");
                    if (data.IsSuccess === false) {
                        alert(data.ErrorMessage);
                        return;
                    }
                    alert(utilsApp.toMessage("PWDChangeSuccess"));
                    window.location = '/User/Logout';
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