app.controller('BookPage', function ($scope, $http, $routeParams, $window) {

    var SetBookData = function (data) {
        $scope.ImageURL = data.ImageURL;
        $scope.BookName = data.Name;
        $scope.Author = data.Author;
        $scope.Genre = data.Genre;
        $scope.Language = data.Language;
        $scope.Price = data.Price;
        $scope.SellerName = data.Uploader.FirstName;
    };

    
    if ($routeParams.ID) {
        $http.get("api/BookStore/GetBookInfo", { params: { ID: $routeParams.ID } })
            .then(function (response) {
                console.log(response.data);
                SetBookData(response.data);
            }, function (reason) {
                console.log(reason);
            });
    } else {
        console.log("Sehf var Linkdaki ID alinmadi  !!!!")
    }

});