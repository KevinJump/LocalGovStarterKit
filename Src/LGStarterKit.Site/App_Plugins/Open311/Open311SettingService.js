angular.module('umbraco.resources').factory('open311SettingService',
    function ($q, $http) {
        return {
            getSettings: function()
            {
                return $http.get('backoffice/open311/Open311SettingsApi/GetSettings');
            },
            getCacheSize: function(Apiroot)
            {
                return $http.get('/' + Apiroot + '/Inquiry/');
            }

        };
    });