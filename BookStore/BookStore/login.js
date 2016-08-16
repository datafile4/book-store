app.controller('loginCtrl',
    function ($scope, $http, $location, $rootScope) {

        $scope.Login_ = function () {

            var loginData = {
                Username: $scope.Username,
                password: $scope.password

            };
            var res = $http.post("api/bookstore/login", loginData);
            console.log("Parameters : " + loginData.Username + "   Password: " + loginData.password);
            res.success(function (data) {
                console.log("success: " + JSON.stringify(data));

                $rootScope.isLogged = data.success;
                if ($rootScope.isLogged) {
                    $location.path('/dashboard');
                } else {
                    $scope.message = data.message;
                    $scope.emiraslan = {
                        color: 'red'
                    }
                }


            });

            res.error(function (data) {
                console.log("error");
            });
        };

    });
////////////////////////////////////////////////////////


//$http(
//    {
//        method: 'POST',
//       url: 'http://localhost:52521/api/bookstore/login',
//        data: loginData,
//    }
//    ).then(function (myresponse, $rootScope) {
//        var ServerResponseObject = myresponse.data;
//        var mySuccess = ServerResponseObject.success;
//        var msg = ServerResponseObject.message;
//        if (mySuccess) {
//            $rootScope.test= true;
//            $scope.message = msg;
//            $scope.emiraslan = {
//                color: 'green'
//            }
//            $location.path('/dashboard');
//        }
//        else {
//            $scope.message = msg;
//            $scope.emiraslan = {
//                color: 'red'
//            }
//        }

//    })

//$http.get('/ServerRequest/GetData', config)
//.success(function (data, status, headers, config) {
//    $scope.Details = data;
//})
//.error(function (data, status, header, config) {
//    $scope.ResponseDetails = 'Data: ' + data +
//        '<hr />status: ' + status +
//        '<hr />headers: ' + header +
//        '<hr />config: ' + config;
//});

app.controller('registerCtrl', function ($scope, $http, $location, $rootScope) {

    $scope.Register_ = function () {
        console.log("success !!!" );
        var registerData = {
            Firstname: $scope.firstname,
            Lastname: $scope.lastname,
            Username: $scope.username,
            Email: $scope.email,
            Password: $scope.password
        };
        console.log(
            "FirstName : " + registerData.Firstname +
            "   LastName: " + registerData.Lastname +
            "   Username: " + registerData.Username +
            "     Email : " + registerData.Email +
            "   Password: " + registerData.Password

            );
        var res = $http.post("api/bookstore/register", registerData);

        res.success(function (data) {
            console.log("success: " + JSON.stringify(data));

            $rootScope.isLogged = data.success;
            if ($rootScope.isLogged) {
                $location.path('/dashboard');
            } else {
                $scope.message = data.message;
                $scope.emiraslan = {
                    color: 'red'
                }
            }


        });

        res.error(function (data) {
            console.log("error");
        });

    };

});