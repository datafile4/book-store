app.controller('BookPage', function ($scope, $http, $routeParams, $window, $location, $rootScope) {

    var SetBookData = function (data) {
        $scope.ID = data.ID;
        $scope.ImageURL = data.ImageURL;
        $scope.BookName = data.Name;
        $scope.Author = data.Author;
        $scope.Genre = data.Genre;
        $scope.Language = data.Language;
        $scope.Price = data.Price;
        $scope.SellerName = data.Uploader.Username;
        $scope.SellerID = data.Uploader.ID;
    };


    if ($routeParams.ID) {
        $http.get("api/BookStore/GetBookInfo", { params: { ID: $routeParams.ID } })
            .then(function (response) {
                console.log(response.data);
                SetBookData(response.data);
                isInFavorite($scope.ID);
            }, function (reason) {
                console.log(reason);
            });
    } else {
        console.log("Sehf var Linkdaki ID alinmadi  !!!!")
    }

    $scope.AddToCart = function (bookID) {
        
        if ($rootScope.RoleID < 1) {
            $location.path('/Login');
            return;
        }


        $http.post("api/BookStore/AddToCart", null, { params: { ID: bookID } })
        .then(
        function (response) {
            isInFavorite($scope.ID);
        },
        function (response) {
            console.log(response);
        });
    }

    var isInFavorite = function (bookID) {
        $http.get("api/BookStore/IsInCart", { params: { ID: $scope.ID } })
    .then(function (response) {
        $scope.IsInFavor = response.data;

    }, function (reason) {
        console.log(reason);
    });
    }

    $scope.RemoveFromFavorite = function (bookID) {

        $http.post("api/BookStore/RemoveFromCart", null, { params: { ID: bookID } })
            .then(
            function (response) {
                isInFavorite();
            },
            function (response) {
                console.log(response);
            });
    }

});