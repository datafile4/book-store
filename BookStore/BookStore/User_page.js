app.controller('user_page', function ($scope, $http, $window) {
    //console.log("In controller user_page !!");
    var res = $http.get("api/BookStore/GetBooksForAdmin");
    res.success(function (response) {
        $scope.books = response;
     //   console.log(JSON.stringify(response));
    });
    res.error(function (response) {
       // console.log("In controller !!" + JSON.stringify(response));
    })

    $scope.checkAll = function () {
        if ($scope.selectedAll) {
            $scope.selectedAll = false;
        } else {
            $scope.selectedAll = true;
        }
        angular.forEach($scope.books, function (book) {
            book.Selected = $scope.selectedAll;
        });
    }

    
    $scope.confirm = function (index) {
        console.log("BOOK[0] " + $scope.books[index].ID);
        var res = $http.post("api/BookStore/confirmBook?id=" + $scope.books[index].ID);
        res.success(function (response) {
            console.log(JSON.stringify(response));
            var res = $http.get("api/BookStore/GetBooksForAdmin");
            res.success(function (response) {
                $scope.books = response;
                console.log(JSON.stringify(response));
            });
            res.error(function (response) {
                console.log("In controller !!" + JSON.stringify(response));
            })
        });
        res.error(function (response) {
            console.log(JSON.stringify(response));
        })
    }

    $scope.delete = function (index) {
        var deleteUser = $window.confirm('Are you absolutely sure you want to delete?');

        if (deleteUser) {
            var res = $http.post("api/BookStore/deleteBook?id=" + $scope.books[index].ID);
            res.success(function (response) {
                console.log(JSON.stringify(response));
                var res = $http.get("api/BookStore/GetBooksForAdmin");
                res.success(function (response) {
                    $scope.books = response;
                    console.log(JSON.stringify(response));
                });
                res.error(function (response) {
                    console.log("In controller !!" + JSON.stringify(response));
                })
            });
            res.error(function (response) {
                console.log(JSON.stringify(response));
            })
        }
    }
    
})

