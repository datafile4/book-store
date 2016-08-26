app.controller('GetBooks', function ($scope, $http) {

    console.log("Geldiiii 1 \n");
    var res = $http.get("api/bookstore/GetAllBooks");
    console.log("Geldiiii 2 \n");
    res.success(function (response) {
        console.log("Geldiiii ela \n");
        $scope.Books = response;
        console.log(JSON.stringify(response));
    });
    res.error(function (response) {
        console.log("Geldiiii berbat \n");
        console.log("In controller !!" + JSON.stringify(response));
    })




    var f = $scope;


    $http.get('api/BookStore/GetLanguages').success(function (data) {
        f.Languages = data
    });
    $http.get('api/BookStore/GetGenres').success(function (data) {
        f.Genres = data
    });

    f.upload = function () {

        var uploadData = {
            Name: f.bookName,
            Author: f.Author,
            ImageURL: f.ImageURL,
            LanguageID: f.SelectedLang.ID,
            GenreID: f.SelectedGenre.ID,
            Price: f.Price
        }

        $http.post('api/BookStore/UploadBook', uploadData)
        .then(
        function (response) {
            vm.message = response.data.message,
            vm.messageStyle = { color: (response.data.success ? 'green' : 'red') };
        },

        function (reason) {

            console.log(reason);
        })

    }


})
