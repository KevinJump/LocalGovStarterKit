function simpleStyledTextController($scope) 
{
    $scope.limit = parseInt($scope.model.config.charCount);

    $scope.model.hideLabel = ($scope.model.config.hideLabel == 1);

    $scope.getClass = function () {
        if ($scope.limit > 0 && $scope.model.value != undefined)
        {
            if ($scope.model.value.length >= $scope.limit)
            {
                return 'text-error';
            }
            return 'muted';
        }
    }

    $scope.checkCharLimit = function () {
        if ($scope.limit > 0 && $scope.model.value != undefined) {
            if ($scope.model.value.length > $scope.limit) {

                if ($scope.model.config.enforceLimit == 1) {
                    $scope.model.value = $scope.model.value.substr(0, $scope.limit);
                    $scope.info = 'you have reached the limit of ' + $scope.limit + ' characters'
                }
                else {
                    $scope.info = 'You have gone over the recommended length by ' +
                        ($scope.model.value.length - $scope.limit) + ' characters';
                }
            }
            else {
                $scope.info = ($scope.limit - $scope.model.value.length) + ' characters left';
            }

        }
    }

    $scope.checkCharLimit();
}

angular.module('umbraco')
    .controller('jumoo.propEditor.styledTextController', simpleStyledTextController);