var app = angular.module('app', ['ngRoute']);

app.config(['$routeProvider', function ($routeProvider, $rootScope) {
    $routeProvider
    .when('/', {templateUrl : 'index.html'})
    .when('/login', {
        templateUrl: 'login.html',
        controller: 'loginCtrl'

    })
        .when('/register', {
            templateUrl: 'register.html',
        })

     .when('/dashboard', {
         resolve: {
             "check": function ($location, $rootScope) {
                 console.log("rootscope.isLogged: " + $rootScope.isLogged);
                 if (!$rootScope.isLogged) {
                     $location.path('/');
                 }
             }
         },
         templateUrl: 'products_in.html'
     })
    .otherwise({
        redirectTo: '/'
    });
}]);