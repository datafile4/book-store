app.controller('UploadBook', function ($scope, $http) {
     var vm = $scope;


     $http.get('api/BookStore/GetLanguages').success(function (data) {
         vm.languages = data
     });
     $http.get('api/BookStore/GetGenres').success(function (data) {
         vm.genres = data
     });

     vm.upload = function () {


         var file = document.getElementById("BookImage").files[0];
         var r = new FileReader();
         r.onloadend = function (e) {


             var arr = Array.from(new Uint8Array(e.target.result));


             var uploadData = {
                 Name: vm.bookName,
                 Author: vm.Author,
                 ImageBytes: arr,
                 LanguageID: vm.selectedLang.ID,
                 GenreID: vm.selectedGenre.ID,
                 Price: vm.Price

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
         r.readAsArrayBuffer(file);
     }

 })