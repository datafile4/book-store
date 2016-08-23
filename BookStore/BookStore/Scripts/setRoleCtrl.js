
app.controller('setRoleCtrl', function ($scope, $http) {
    var vm = $scope;
    $http.post('api/BookStore/GetRoles').then(function (response) {
        vm.roles = response.data
    })
    vm.apply = function () {
        $http.post('api/BookStore/SetRole?username=' + vm.username + '&RoleID=' + vm.selectedRoleID.RoleID).then(
            function (response) {
                vm.message = response.data.message,
                vm.messageStyle = { color: (response.data.success ? 'green' : 'red') };
            },
            function (reason) {
                console.log(reason);
            })
    }
})