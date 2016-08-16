app.controller('GetBooks', function ($scope, $http) {
    console.log("In controller !!" );
    
    var res = $http.get("api/BookStore/GetAllBooks");
    res.success(function (response) {
        $scope.books = response;
        console.log( JSON.stringify(response));

    });
    res.error(function (response) {
        console.log("In controller !!" + JSON.stringify(response));
    })
    
})
