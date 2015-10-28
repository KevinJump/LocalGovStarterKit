angular.module("umbraco")
    .controller("Jumoo.LocalGov311.SettingsController",
    function ($scope, eventsService, open311SettingService, appState) {

        open311SettingService.getSettings().then(function (response) {
            $scope.ApiRoot = response.data.ApiRootUrl;
            $scope.useEsdAsId = response.data.useEsdAsId;
            $scope.fields = response.data.Fields;

            open311SettingService.getCacheSize(response.data.ApiRootUrl).then(function (response) {
                $scope.cacheCount = response.data.length;
            });
        });
    });