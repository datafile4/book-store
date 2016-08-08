

function login() {
    var username = $('#username').val()
    var password = $('#password').val()

    var model = { Username: username, Password: password }
    model = JSON.stringify(model)
    $.ajax({
        type: "POST",
        data: model,
        url: "http://localhost:2587/api/UserManagement/LoginUser",
        contentType: "application/json",
        error: function (xhr, textStatus, errorThrown) {
            console.log(JSON.parse(xhr.responseText).Message);
        },

        success: function (data) {
            console.log("You have sucessfully logged in. Credentials are found on our linux-based extermely advaned server's oracle database");
        }

    });
};



function registerMe() {
    var email = $('#email').val()
    var lastName = $('#lastName').val()
    var username = $('#username').val()
    var password = $('#password').val()
    var firstName = $('#firstName').val()


    var model =
        {
            Username: username,
            Password: password,
            Email: email,
            FirstName: firstName,
            LastName: lastName
        };

    model = JSON.stringify(model)
    console.log(model);
    $.ajax({
        type: "PUT",
        data: model,
        url: "api/UserManagement/RegisterUser",
        contentType: "application/json",
        error: function (xhr, textStatus, errorThrown) {
            console.log(xhr.responseText);
        },
        success:
            function (data) {
                console.log(data)
            }
    });
};