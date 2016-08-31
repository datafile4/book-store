app.controller('GetBooks', function ($scope, $http, $rootScope) {
    var f = $scope;

    $rootScope.StartPage = 1;
    $rootScope.ActivePage= 1;

    $scope.range = function (min, max, step) {
        step = step || 1;
        var input = [];
        for (var i = min; i <= max; i += step) {
            input.push(i);
        }
        return input;
    };

    $scope.FindPage = function (pageNumber) {
        var res = $http.post("api/bookstore/GetBooks", { PageNumber: pageNumber-1, PageLength: 5 });

        res.success(function (response) {
            f.Books = response;
            console.log(response);
        });
        res.error(function (response) {
            console.log("In controller error!!" + JSON.stringify(response));
        })

    };

    $scope.FindPage($rootScope.ActivePage);

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

    $scope.ApplySelectedFilters = function () {
        var LangsID = [];
        angular.forEach(f.Languages, function (lang) {
            if (lang.Selected) {
                LangsID.push(lang.ID);
            }
        })

        var GenresID = [];
        angular.forEach(f.Genres, function (genre) {
            if (genre.Selected) {
                GenresID.push(genre.ID);
            }
        })

        var Filter = {
            GenreIDs: GenresID,
            LangIDs: LangsID,
            SearchTerms: [], 
            LowPrice: f.LowPrice,
            HighPrice: f.HighPrice,
            Pagination: {
                PageNumber: 0,
                PageLength: 10
            }
        };

        $http.post("api/BookStore/GetFilteredBooks", Filter)
        .then(
        function (response) {
            f.Books = response.data;
        },
        function (response) {
            console.log(response);
        });
    }
});