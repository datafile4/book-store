app.controller("FavoriteBooks", function ($scope, $http, $routeParams, $window) {


    var GetCartItems = function () {
        $http.get("api/BookStore/GetCartItems")
        .then(
        function (response) {
            $scope.Books = response.data;
        },
        function (response) {
            console.log(response);
        });
    }
    GetCartItems();

    $scope.RemoveFromFavorite = function (bookID) {
        console.log("Getirilidi !");

        $http.post("api/BookStore/RemoveFromCart", null, { params: { ID: bookID } })
            .then(
            function (response) {
                GetCartItems();
            },
            function (response) {
                console.log(response);
            });
    }

});