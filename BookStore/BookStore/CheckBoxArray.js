

app.controller('CheckboxCtrl', function ($scope) {
    var res = $http.get("api/BookStore/GetBooksForAdmin");
    res.success(function (response) {
        $scope.books = response;
        //   console.log(JSON.stringify(response));
    });
    res.error(function (response) {
        // console.log("In controller !!" + JSON.stringify(response));
    })
    $scope.otherbook= {
        ID: []
    };

    $scope.toggle_user_id = function (id) {
        if ($scope.otherbook.ID.indexOf(id) == -1) {
            $scope.otherbook.ID.push(id);
        }
        else {
            $scope.otherbook.ID.splice($scope.otherbook.ID.indexOf(id), 1);
        }
    };

    $scope.checkAll = function () {
        for (i = 0; i < $scope.books.length; i++)
            $scope.checkNth(i);
        if ($scope.otherbook.ID.indexOf($scope.books[i]) == -1) {
            $scope.otherbook.ID.push($scope.books[i]);
        }
    };

    $scope.checkNth = function (i) {
        $scope.books[i].checked = !$scope.books[i].checked;
        $scope.toggle_user_id($scope.books[i]);
    }

    $scope.uncheckAll = function () {
        for (i = 0; i < $scope.books.length; i++)
            $scope.books[i].checked = false;
        $scope.user.books = [];
    };
});
