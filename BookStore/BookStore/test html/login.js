var app = angular.module("app", []).controller("HttpGetController", function ($scope, $http) {

    $scope.Login_ = function () {

        var loginData = {
            Username: $scope.Username,
            Password: $scope.password
        };

        $http(
            {
                method: 'POST',
                url: '../api/bookstore/login',
                data: loginData,
            }
            ).then(function(myresponse) {
                var ServerResponseObject = myresponse.data;
                var mySuccess = ServerResponseObject.success;
                var msg = ServerResponseObject.message;
                if (mySuccess)
                {
                    $scope.message = msg;
                    $scope.emiraslan = {
                        color: 'green'
                    }
                } 
                else {
                    $scope.message = msg;
                    $scope.emiraslan = {
                        color: 'red'
                    }
                }

            })

        //$http.get('/ServerRequest/GetData', config)
        //.success(function (data, status, headers, config) {
        //    $scope.Details = data;
        //})
        //.error(function (data, status, header, config) {
        //    $scope.ResponseDetails = "Data: " + data +
        //        "<hr />status: " + status +
        //        "<hr />headers: " + header +
        //        "<hr />config: " + config;
        //});
    };

});