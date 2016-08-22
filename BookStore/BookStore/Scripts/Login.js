app.controller('LoginCtrl',
    function ($scope, $http, $location, $rootScope) {

        $scope.Login = function () {

            var LoginData = {
                Username: $scope.Username,
                Password: $scope.Password
            };

            var res = $http.post("api/BookStore/Login", LoginData);
            console.log("Parameters : " + LoginData.Username + "   Password: " + LoginData.Password);
            res.success(function (data) {
                console.log("success: " + JSON.stringify(data));
                $location.path('/Dashboard');
                $rootScope.UpdateRoleID();

            });

            res.error(function (data) {

                console.log("Not Logged In ! Some Error Happened !");

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
