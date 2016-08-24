
app.controller('setRoleCtrl', function ($scope, $http) {
    var vm = $scope;
    $http.get('api/BookStore/GetRoles').then(function (response) {
        vm.roles = response.data
    })
    vm.apply = function () {
        var setRoleModel = {
            username: vm.username,
            roleID: vm.selectedRoleID.ID
        }

        $http.post('api/BookStore/SetRole', null, { params: setRoleModel }).then(
            function (response) {
                vm.message = response.data.message,
                vm.messageStyle = { color: (response.data.success ? 'green' : 'red') };
            },
            function (reason) {
                console.log(reason);
            })
    }
});