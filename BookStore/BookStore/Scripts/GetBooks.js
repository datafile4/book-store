app.controller('GetBooks', function ($scope, $http) {
    
    console.log("Geldiiii 1 \n");
    var res = $http.get("api/bookstore/GetAllBooks");
    console.log("Geldiiii 2 \n");
    res.success(function (response) {
        console.log("Geldiiii ela \n");
        $scope.Books = response;
        console.log( JSON.stringify(response));
    });
    res.error(function (response) {
        console.log("Geldiiii berbat \n");
        console.log("In controller !!" + JSON.stringify(response));
    })
    
})
