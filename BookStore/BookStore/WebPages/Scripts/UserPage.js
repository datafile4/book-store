app.controller('UserPage', function ($scope, $http, $window) {
    
    var res = $http.get("api/BookStore/GetBooksForConfirmation");
    res.success(function (response) {
        $scope.Books = response;
           console.log(JSON.stringify(response));
    });
    res.error(function (response) {
         console.log("In controller !!" + JSON.stringify(response));
    })

    $scope.CheckAll = function () {
        $scope.AllSelected = !$scope.AllSelected;
        angular.forEach($scope.Books, function (book) {
            book.Selected = $scope.AllSelected;
        })
    }

    $scope.Confirm = function (index) {
        console.log("BOOK[0] " + $scope.Books[index].ID);
        var res = $http.post("api/BookStore/ConfirmBook?id=" + $scope.Books[index].ID);
        res.success(function (response) {
            console.log(JSON.stringify(response));
            var res = $http.get("api/BookStore/GetBooksForConfirmation");
            res.success(function (response) {
                $scope.Books = response;
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

    $scope.Delete = function (index) {
        var DeleteUser = $window.confirm('Are you absolutely sure you want to delete?');

        if (DeleteUser) {
            var res = $http.post("api/BookStore/DeleteBook?id=" + $scope.Books[index].ID);
            res.success(function (response) {
                console.log(JSON.stringify(response));
                var res = $http.get("api/BookStore/GetBooksForConfirmation");
                res.success(function (response) {
                    $scope.Books = response;
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

    $scope.ConfirmSelected = function () {
        ID = [];
        angular.forEach($scope.Books, function (book) {
            if (book.Selected) {
                ID.push(book.ID);
            }
        })
        //console.log(" smth1 ");
        var res = $http.post("api/BookStore/ConfirmCheckedBook", ID);
        //console.log(" smth2 ");
        res.success(function (response) {
            console.log(" smth3 ");
            var res = $http.get("api/BookStore/GetBooksForConfirmation");
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
