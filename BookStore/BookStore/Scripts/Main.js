var app = angular.module('app', ['ngRoute']);

app.config(['$routeProvider', function ($routeProvider, $rootScope) {
    $routeProvider
    .when('/', { templateUrl: 'HTML/HomePage.html' })
    .when('/Login', {
        resolve: {
            "check": function ($location, $rootScope) {
                console.log("$rootScope.RoleID: " + $rootScope.RoleID);
                if ($rootScope.RoleID > 0) {
                    $location.path('/UserPage');
                }
            }
        },
        templateUrl: 'HTML/Login.html',
        controller: 'LoginCtrl'

    })
    .when('/Register', {
        templateUrl: 'HTML/Register.html',
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
        templateUrl: 'HTML/AllProducts.html',
        controller: 'GetBooks'
    })
    .when('/UserPage', {
        resolve: {
            "check": function ($location, $rootScope) {
                console.log("$rootScope.RoleID: " + $rootScope.RoleID);
                if ($rootScope.RoleID < 1) {
                    $location.path('/Login');
                }
            }
        },
        templateUrl: 'HTML/UserPage.html',
        controller: 'UserPage'
    })
    .when('/UploadBook', {
        templateUrl: 'HTML/UploadBook.html',
        controller: 'UploadBook'
    })
    .when('/SetRole', {
        templateUrl: 'HTML/SetRole.html',
        controller: 'setRoleCtrl'
    })

    .otherwise({
        redirectTo: '/'
    });
}]).run(function ($rootScope, $location, $http) {


    $rootScope.UpdateRoleID = function () {

        $http.get("api/bookstore/GetCurrentUserInfo").success(function(data) {
            $rootScope.Username = data.Username;
        })



       var res = $http.get("api/bookstore/GetUserRole");
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