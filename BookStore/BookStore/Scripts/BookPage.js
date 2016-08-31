app.controller('BookPage', function ($scope, $http, $routeParams, $window) {


       
if ($routeParams.ID) {
    $http.get("api/BookStore/GetBookInfo", { params: { ID: $routeParams.ID } })
        .success(SetBookData)
} else {
    console.log("adam olun adami eseblesdirmiyin , heyvanlar. Essekler !!!!")
}
 
});