app.controller('GetBooks', function ($scope, $http, $rootScope) {
    var f = $scope;


    $rootScope.StartPage = 1;
    $rootScope.ActivePage = 1;
    $rootScope.PageLength = 5;

    $scope.range = function (min, max, step) {
        step = step || 1;
        var input = [];
        for (var i = min; i <= max; i += step) {
            input.push(i);
        }
        return input;
    };

    $scope.FindPage = function (pageNumber) {
        var res = $http.post("api/bookstore/GetBooks", { PageNumber: pageNumber - 1, PageLength: $rootScope.PageLength });

        res.success(function (response) {
            f.Books = response;
            console.log("Geldi kisi !!!" + JSON.stringify(response));
        });
        res.error(function (response) {
            console.log("In controller error!!" + JSON.stringify(response));
        })

    };



    //$scope.FindPage($rootScope.ActivePage);

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

    $rootScope.ApplySelectedFilters = function (pageNumber) {
        console.log("ApplySelectedFilters");
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

        f.countOf = function (text) {
            var s = text ? text.split(/\s+/) : 0;
            return s ? s.length : '';
        };

        var SearchTerms = [];
        f.myWordSplitter = function () {
            var count_loop = f.countOf(f.SearchTerm)
            for (var i = 0 ; i < count_loop ; i++) {
                SearchTerms.push(f.SearchTerm.split(" ")[i]);
            }
        };


        f.myWordSplitter();
        

        var Filter = {
            GenreIDs: GenresID,
            LangIDs: LangsID,
            SearchTerms: SearchTerms,
            LowPrice: 1,
            HighPrice: 999,
            Pagination: {
                PageNumber: pageNumber-1,
                PageLength: $rootScope.PageLength
            }
        };

        console.log($scope.SearchTerms);
        //console.log("Slalammlar Kisi !  " + JSON.stringify(Filter));
        $http.post("api/BookStore/GetFilteredBooks", Filter)
        .then(
        function (response) {
            f.Books = response.data.Books;
            f.AmountPage = Math.floor(response.data.TotalCount / $rootScope.PageLength);
            console.log("BURDAYAM  -> " + f.AmountPage);
            console.log("F.BOOKS :   !  " + JSON.stringify(f.Books));
        },
        function (response) {
            console.log(response);
        });
    }

    $scope.ApplySelectedFilters();

    var PaginationAlgorithm = function () {
        f.startPage = 1;
        f.endPage = 15;
        f.currentPage = 9;
        if (totalPages <= 7) {
            // less than 10 total pages so show all
            startPage = 1;
            endPage = totalPages;
        } else {
            // more than 10 total pages so calculate start and end pages
            if (currentPage <= 6) {
                startPage = 1;
                endPage = 10;
            } else if (currentPage + 4 >= totalPages) {
                startPage = totalPages - 9;
                endPage = totalPages;
            } else {
                startPage = currentPage - 5;
                endPage = currentPage + 4;
            }
        }

    }
});