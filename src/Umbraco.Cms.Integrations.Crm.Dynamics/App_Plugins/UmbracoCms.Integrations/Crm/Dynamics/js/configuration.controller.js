﻿function configurationController($scope, notificationsService, umbracoCmsIntegrationsCrmDynamicsResource) {
    var vm = this;

    vm.oauthConfig = {};
    vm.oauthSuccessEventCount = 0;

    umbracoCmsIntegrationsCrmDynamicsResource.checkOAuthConfiguration().then(function (response) {
        if (response && response.isAuthorized) {
            vm.oauthConfig.isConnected = true;
            vm.oauthConfig.fullName = response.fullName;

            if (typeof $scope.connected === "function")
                $scope.connected();
        } else if (response.message.length > 0) {
            if (typeof $scope.connected === "undefined")
                notificationsService.error("Dynamics Configuration", response.message);
        }
    });

    vm.onConnectClick = function () {

        umbracoCmsIntegrationsCrmDynamicsResource.getAuthorizationUrl().then(function (response) {
            vm.authWindow = window.open(response,
                "Authorize", "width=900,height=700,modal=yes,alwaysRaised=yes");
        });
    }

    vm.onRevokeToken = function () {

        umbracoCmsIntegrationsCrmDynamicsResource.revokeAccessToken().then(function () {
            vm.oauthConfig.isConnected = false;

            // if directive runs from property editor, the notifications should be hidden, because they will not be displayed properly behind the overlay window.
            // if directive runs from data type, the notifications are displayed
            if (typeof $scope.connected === "undefined")
                notificationsService.success("Dynamics Configuration", "OAuth connection revoked.");

            if (typeof $scope.revoked === "function")
                $scope.revoked();
        });
    }

    // authorization listener
    window.addEventListener("message", function (event) {
        if (event.data.type === "hubspot:oauth:success") {
            vm.oauthSuccessEventCount += 1;

            if (vm.oauthSuccessEventCount == 1) {
                umbracoCmsIntegrationsCrmDynamicsResource.getAccessToken(event.data.code).then(function (response) {

                    if (response.startsWith("Error:")) {

                        // if directive runs from property editor, the notifications should be hidden, because they will not be displayed properly behind the overlay window.
                        // if directive runs from data type, the notifications are displayed
                        if (typeof $scope.connected === "undefined")
                            notificationsService.error("Dynamics Configuration", response);
                    } else {
                        vm.oauthConfig.isConnected = true;

                        // if directive runs from property editor, the notifications should be hidden, because they will not be displayed properly behind the overlay window.
                        // if directive runs from data type, the notifications are displayed
                        if (typeof $scope.connected === "undefined")
                            notificationsService.success("Dynamics Configuration", "OAuth connected.");

                        umbracoCmsIntegrationsCrmDynamicsResource.getSystemUserFullName().then(function (response) {
                            vm.oauthConfig.fullName = response;
                        });

                        if (typeof $scope.connected === "function")
                            $scope.connected();
                    }
                });
            }
        }
    }, false);
}

angular.module("umbraco")
    .controller("Umbraco.Cms.Integrations.Crm.Dynamics.ConfigurationController", configurationController);