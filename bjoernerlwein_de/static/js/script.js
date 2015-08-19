var bjoernerlweinde = angular.module('bjoernerlweinde', ["ngRoute", "viewhead"]);

bjoernerlweinde.config(["$routeProvider", function($routeProvider) {
    $routeProvider.when("/", {
        templateUrl: '/views/posts/index.html'
    });
    $routeProvider.when("/post/:post", {
        templateUrl: '/views/posts/show.html'
    });
    $routeProvider.when("/staticpage/:page", {
        templateUrl: '/views/staticpages/show.html'
    });
    $routeProvider.otherwise({redirectTo: '/'});
}]);

bjoernerlweinde.controller("staticPagesController", ["$scope", "$http", "$sce", "$routeParams", function($scope, $http, $sce, $routeParams) {

    $scope.to_trusted = function(html_code) {
        return $sce.trustAsHtml(html_code);
    };

    $scope.index = function(){
        $http({
            method: "GET",
            url: "/staticpages"
        }).
        success(function(response){
            $scope.pages = response.pages;

        });
    };

     $scope.show = function(){

        $http({
            method: "GET",
            url: "/staticpage/" + $routeParams.page
        }).
        success(function(response){
            $scope.page = response;
        });
    };

}]);

bjoernerlweinde.controller("postsController",  ["$scope", "$http", "$sce", "$routeParams", function($scope, $http, $sce, $routeParams) {

    $scope.toDateString = function(string){
        return (new Date(string)).toLocaleString();
    };
    $scope.to_trusted = function(html_code) {
        return $sce.trustAsHtml(html_code);
    };

    $scope.show = function(){

        $http({
            method: "GET",
            url: "/post/" + $routeParams.post
        }).
        success(function(response){
            $scope.post = response;

        });
    };

    $scope.index = function(){
        $http({
            method: "GET",
            url: "/posts"
        }).
        success(function(response){
            $scope.posts = response.posts;

        });
    };


}]);

