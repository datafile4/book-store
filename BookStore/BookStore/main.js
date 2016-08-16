﻿var app = angular.module('app', ['ngRoute']);

app.config(['$routeProvider', function ($routeProvider, $rootScope) {
    $routeProvider
    .when('/', {templateUrl : 'main.html'})
    .when('/login', {
        templateUrl: 'login.html',
        controller: 'loginCtrl'

    })
        .when('/register', {
            templateUrl: 'register.html',
            controller: 'registerCtrl'
        })

     .when('/dashboard', {
         //resolve: {
         //    "check": function ($location, $rootScope) {
         //        console.log("rootscope.isLogged: " + $rootScope.isLogged);
         //        if (!$rootScope.isLogged) {
         //            $location.path('/');
         //        }
         //    }
         //},
         templateUrl: 'products_in.html',
         controller: 'GetBooks'
     })
        .when('/user_page', {
            templateUrl: 'user_page.html',
            controller: 'GetBooks'
        })
    .otherwise({
        redirectTo: '/'
    });
}]);