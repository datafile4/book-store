app.controller('HomePage', function ($scope, $http, $location, $rootScope, $routeParams) {
    console.log("Beli, Kontrollerdeyik !!!");
    var s = $scope;
    

    $http.get("api/BookStore/GetRandomBooks", { params: { pageLength: 6 } })
.then(
function (response) {
    s.Books = response.data;
    console.log(response.data);
},
function (response) {
    console.log(response);
});
});