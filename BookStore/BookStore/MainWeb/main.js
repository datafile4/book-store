var app = angular.module('app', ['ngRoute']);
app.config(['$routeProvider', function ($routeProvider, $rootScope) {
    $routeProvider
    .when('/index', {
        templateUrl: 'index.html',
        controller: 'loginCtrl'

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
         templateUrl: 'buy.html'
     })
    .otherwise({
        redirectTo: '/index'
    });
}]);