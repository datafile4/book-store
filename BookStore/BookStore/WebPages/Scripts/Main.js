var app = angular.module('app', ['ngRoute']);

app.config(['$routeProvider', function ($routeProvider, $rootScope) {
    $routeProvider
    .when('/', { templateUrl: 'HomePage.html' })
    .when('/Login', {
        templateUrl: 'Login.html',
        controller: 'LoginCtrl'

    })
    .when('/Register', {
        templateUrl: 'Register.html',
        controller: 'RegisterCtrl'
    })

    .when('/Dashboard', {
        //resolve: {
        //    "check": function ($location, $rootScope) {
        //        console.log("rootscope.isLogged: " + $rootScope.isLogged);
        //        if (!$rootScope.isLogged) {
        //            $location.path('/');
        //        }
        //    }
        //},
        templateUrl: 'AllProducts.html',
        controller: 'GetBooks'
    })
    .when('/UserPage', {
        templateUrl: 'UserPage.html',
        controller: 'UserPage'
    })
    .when('/UploadBook', {
        templateUrl: 'UploadBook.html',
        controller: 'UploadBook'
    })
    .otherwise({
        redirectTo: '/'
    });
}]).run(function ($rootScope, $location, $http) {


    $rootScope.UpdateRoleID = function () {

        var res = $http.get("../../api/bookstore/GetUserRole");
        res.success(function (data) {

            $rootScope.RoleID = parseInt(data);
            console.log("RoleID : " + $rootScope.RoleID);

            //$rootScope.$on("$routeChangeStart", function (event, next, current) {
            //    /*
            //    if ($rootScope.RoleID == 0) { //noone
            //        $location.path("/Login");
            //    }
            //    else if ($rootScope.RoleID == 3) { // admin
            //        $location.path("/Dashboard");
            //    }
            //    else if ($rootScope.RoleID == 2) {//moderator
            //        $location.path("/");
            //    }
            //    else if ($rootScope.RoleID == 1) {//
            //        $location.path("/GetBooks");
            //    }
            //    else {
            //        console.log("else : " + $rootScope.RoleID);
            //    }
            //    */
            //});



        });
        res.error(function (data) {
            console.log("Error in GetUserRole")
        });
    }

    $rootScope.UpdateRoleID();
});