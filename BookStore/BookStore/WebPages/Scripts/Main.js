var app = angular.module('app', ['ngRoute']);

app.config(['$routeProvider', function ($routeProvider, $rootScope) {
    $routeProvider
    .when('/', {templateUrl : 'HomePage.html'})
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
        controller: 'GetBooks'
    })
    .otherwise({
        redirectTo: '/'
    });
}]);