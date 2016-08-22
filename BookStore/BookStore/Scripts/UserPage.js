app.controller('UserPage', function ($scope, $http, $window) {
    
    var res = $http.get("api/BookStore/GetBooksForConfirmation");
    console.log("Confirm Bura geldi Tebrikler ! \n");
    res.success(function (response) {
        $scope.Books = response;
           console.log(JSON.stringify(response));
           console.log("Confirm alindi ela! \n");
    });


    res.error(function (response) {
         console.log("In controller !!" + JSON.stringify(response));
         console.log("Confirm alinmadi beerbat! \n");
    })

    var res = $http.post("api/BookStore/GetCurrentUserInfo");

    res.success(function (data) {
        $scope.FirstName = data.FirstName;
        $scope.LastName = data.LastName;
        $scope.Email = data.Email;
        $scope.Username = data.Username;
    })

   

    $scope.CheckAll = function () {
        $scope.AllSelected = !$scope.AllSelected;
        angular.forEach($scope.Books, function (book) {
            book.Selected = $scope.AllSelected;
        })
    }

    

    $scope.Confirm = function (index) {
        console.log("BOOK[0] " + $scope.Books[index].ID);
        var res = $http.post("api/BookStore/ConfirmBook?bookID=" + $scope.Books[index].ID);
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
            var res = $http.post("api/BookStore/DeleteBook?ID=" + $scope.Books[index].ID);
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

    var BooksID = [];

    $scope.ConfirmSelected = function () {
        BooksID = [];
        angular.forEach($scope.Books, function (book) {
            if (book.Selected) {
                BooksID.push(book.ID);
            }
        })

        //console.log(" smth1 ");
        var res = $http.post("api/BookStore/ConfirmBook", BooksID);
        //console.log(" smth2 ");
        res.success(function (response) {
            var res = $http.get("api/BookStore/GetBooksForConfirmation");
            res.success(function (response) {
                $scope.Books = response;
                console.log(JSON.stringify(response));
                console.log("Confirm alindi ela! \n");
            });
            res.error(function (response) {
                console.log("In controller !!" + JSON.stringify(response));
                console.log("Confirm alinmadi beerbat! \n");
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
