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
        $scope.AllSelected = !$scope.AllSelected;
        angular.forEach( $scope.books, function (book) {
            book.Selected = $scope.AllSelected;
        })
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

    var ID = [];
    //$scope.toggle_book_id = function (id) {
    //    console.log("inside toggle_book_id");
    //    if (ID.indexOf(id) == -1) {
    //        ID.push(id);
    //        console.log("push");
    //    }
    //    else {
    //        ID.splice(ID.indexOf(id), 1);
    //        console.log("splice");
    //    }
    //    console.log("ID.length = " + ID.length)
    //    for
    //}

    $scope.confirmSelected = function () {
        ID = [];
        angular.forEach($scope.books, function (book) {
            if (book.Selected) {
                ID.push(book.ID);
            }
        })

        

        console.log(" smth1 ");
        var res = $http.post("api/BookStore/confirmCheckedBook", ID);
        console.log(" smth2 ");
        res.success(function (response) {
            console.log(" smth3 ");
            var res = $http.get("api/BookStore/GetBooksForAdmin");
            console.log(" smth ");
            res.success(function (response) {
                $scope.books = response;
            
            });
            res.error(function (response) {
                console.log("In controller !!" + JSON.stringify(response));
            })
        });
        res.error(function (response) {
            console.log(JSON.stringify(response));
        })
    }

});

