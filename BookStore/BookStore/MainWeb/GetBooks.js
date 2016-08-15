app.controller('GetBooks', function ($scope, $http) {
    $scope.getbooks = function () {

        var res = $http.get("http://localhost:52521/api/BookStore/GetAllBooks");
        res.success(function (response) {
            $scope.books = response.data;
            Console.log(books);
        })
    }
})
