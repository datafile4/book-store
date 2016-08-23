app.controller('UserPage', function ($scope, $http, $window) {
    
    var GetUnconfirmedBooks = function () {
        $http.get("api/BookStore/GetUnconfirmedBooks").then(
        function (response) {
            $scope.Books = response.data;
        },
        function (reason) {
            console.log(reason);
        })
    };

    GetUnconfirmedBooks();
    $http.get("api/BookStore/GetCurrentUserInfo")
        .success(function (data) {
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

    var confirmOrdelete = function (url, obj) {
        
        var config = null;
        var data = null;

        if (obj.params)
            config = obj
        else
            data = obj

        $http.post(url, data, config).then(
            function (response) {
                console.log(response.data)
                if (response.data.success) {
                    GetUnconfirmedBooks();
                }
            },
            function (reason) {
                console.log(reason.data);
            }
    )
    };

    $scope.Confirm = function(index) {
        confirmOrdelete('api/BookStore/ConfirmBook', { params: { ID: $scope.Books[index].ID } });
    }

    $scope.Delete = function (index) {
        var DeleteUser = $window.confirm('Are you absolutely sure you want to delete?');

        if (DeleteUser) {
            confirmOrdelete('api/BookStore/DeleteBook', { params: { ID: $scope.Books[index].ID } });
        }
    }

    $scope.ConfirmSelected = function () {
        var BooksID = [];
        angular.forEach($scope.Books, function (book) {
            if (book.Selected) {
                BooksID.push(book.ID);
            }
        })

        confirmOrdelete("api/BookStore/ConfirmBooks", BooksID);
    }

});