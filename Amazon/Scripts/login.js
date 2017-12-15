var Login = function () {
    var show = function () {
        $('body').on('click', '#login-submit', function () {
            login_submit();
        });

        $('body').on('keydown', '.login-input', function (e) {
            if (e.keyCode === 13) {
                login_submit();
            }
        });

        var login_submit = function () {
            var loginid = $('#login_id').val();
            var loginpwd = $('#login_pwd').val();
            var remeberme = $("#remember_me").prop('checked');
            if (loginid && loginpwd) {
                loginid = loginid.toUpperCase();
                if (loginid.indexOf("@FINISAR.COM") !== -1) {
                    $.post('/Login/DoLogin',
                    {
                        loginid: loginid,
                        loginpwd: loginpwd,
                        remember_me: remeberme
                    },
                    function (output) {
                        console.log(output);
                        if (output.success) {
                            window.location.href = '/WorkFlow/AllWorkFlow';
                        }
                        else {
                            alert(output.msg);
                        }
                    });
                }
                else {
                    alert("Please input Your Finisar email address!")
                }
            }
            else {
                alert("UserName and Password need to be input !");
            }
        }

        $('body').on('click', '.forget-pwd', function () {
            var loginid = $('#login_id').val();
            if (loginid) {
                loginid = loginid.toUpperCase();
                if (loginid.toUpperCase().indexOf("@FINISAR.COM") !== -1) {
                    $.post('/Login/ResetPWD',
                    {
                        loginid: loginid
                    },
                    function (output) {
                        alert(output.msg);
                        return false;
                    });
                }
                else {
                    alert("Please input Your Finisar email adress!")
                }
            }
            else
            {
                alert("UserName need to be input !");
            }
        });


        $('body').on('click', '#resetpwd-submit', function () {
            var loginid = $('#login_id').val();
            var newpwd = $('#new_pwd').val();
            var cpwd = $('#confirm_pwd').val();

            if (loginid && newpwd && cpwd)
            {
                loginid = loginid.toUpperCase();
                if (newpwd === cpwd)
                {
                    if (loginid.indexOf("@FINISAR.COM") !== -1)
                    {
                            $.post('/Login/ResetPWDLink',
                            {
                                loginid: loginid,
                                loginpwd: newpwd
                            },
                            function(output) {
                                if (output.success) {
                                    window.location.href = '/WorkFlow/AllWorkFlow';
                                }
                                else {
                                    alert(output.msg);
                                }
                            });
                    }
                    else 
                    {
                        alert("Please input Your Finisar email adress!")
                    }
                }
                else {
                    alert("New Password is not same as confirm password!")
                }
            }
            else {
                alert("UserName and Password need to be input !");
            }
            return false;
        });
    }

    return {
        init: function () {
            show();
        }
    };
}();