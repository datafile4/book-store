app.controller('RegisterCtrl', function ($scope, $http, $location, $rootScope) {
    
    $scope.Register = function () {
        
        var RegisterData = {
            Firstname: $scope.firstname,
            Lastname: $scope.lastname,
            Username: $scope.username,
            Email: $scope.email,
            Password: $scope.password
        };

        console.log(
            "FirstName : " +  RegisterData.Firstname +
            "   LastName: " + RegisterData.Lastname +
            "   Username: " + RegisterData.Username +
            "     Email : " + RegisterData.Email +
            "   Password: " + RegisterData.Password
            );

        var res = $http.post("api/BookStore/Register", RegisterData);

        res.success(function (data) {
            console.log("success: " + JSON.stringify(data));

            $rootScope.isLogged = data.success;
            if ($rootScope.isLogged) {
                $location.path('/Dashboard');
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