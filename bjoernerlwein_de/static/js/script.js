(function(global) {
  "use strict";
  var ajax = global.nanoajax.ajax,
    Vue = global.Vue,
    location = global.location,
    stash = {
      staticpages: []
    },
    headerTitle,
    pgTitle;

  var getId = function() {
    return location.hash.split('/').reverse()[0];
  };


  var setPostForCurrentElement = function() {
    var id = getId();

    var post = stash.posts.filter(function(elem) {
      return elem.id === id;
    })[0];

    this.title = post.title;
    this.date = post.date;
    this.content = post.content;

    headerTitle.setTitle(post.title);
    pgTitle.title = post.title;

  };

  pgTitle = new Vue({
    el: '#pgtitle',
    template: '<small class="sub-header">{{title}}</small>',
    data: {
      title: 'Dummytitle never to be seen'
    }
  });

  headerTitle = new Vue({
    el: '#title',
    template: '<title>{{title}}</title>',
    data: {
      title: 'Bjoernerlwein.de'
    },
    methods: {
      setTitle: function(title) {
        if (typeof title === 'undefined' || title === '') {
          this.title = 'Bjoernerlwein.de';
          return;
        }
        this.title = "Bjoernerlwein.de | " + title;
      }
    }
  });

  new Vue({
    el: '#staticpageslist',
    template: '#staticpagelist-tpl',
    data: {
      staticpages: []
    },
    ready: function() {
      var _this = this;

      if (typeof stash.staticpagelist === 'undefined') {
        ajax({
          url: '/staticpages',
          method: 'GET'
        }, function(code, responseText, request) {
          if (code === 200) {
            var resp = JSON.parse(responseText);
            stash.staticpagelist = resp;
            _this.staticpages = resp;
            pgTitle.title = 'Posts';
          }
        });
      } else {
        _this.staticpages = stash.staticpagelist;
      }
    }

  });

  Vue.component('posts', {
    template: '#posts-index-tpl',
    data: function() {
      return {
        posts: []
      };
    },
    ready: function() {
      var _this = this;
      if (typeof stash.posts === 'undefined') {
        ajax({
          url: '/posts',
          method: 'GET'
        }, function(code, responseText, request) {
          if (code === 200) {
            var resp = JSON.parse(responseText);

            resp.map(function(elem) {
              var d = new Date(elem.date);
              elem.date = d.toLocaleString();
              return elem;
            });

            stash.posts = resp;
            _this.posts = resp;
            headerTitle.setTitle('Posts');

          } else {
            location.hash = '/';
          }
        });
      } else {
        _this.posts = stash.posts;
        headerTitle.setTitle('Posts');
        pgTitle.title = 'Posts';
      }
    }
  });

  Vue.component('post', {
    template: '#post-tpl',
    data: function() {
      return {
        title: '',
        date: '',
        content: ''
      }
    },
    ready: function() {
      var _this = this;
      if (typeof stash.posts === 'undefined') {
        ajax({
          url: '/posts',
          method: 'GET'
        }, function(code, responseText, request) {
          if (code === 200) {
            var resp = JSON.parse(responseText);

            resp.map(function(elem) {
              var d = new Date(elem.date);
              elem.date = d.toLocaleString();
              return elem;
            });
            stash.posts = resp;

            setPostForCurrentElement.call(_this);

          } else {
            location.hash = '/';
          }
        });
      } else {
        setPostForCurrentElement.call(_this);
      }
    }
  });

  Vue.component('staticpage', {
    template: '#post-tpl',
    data: function() {
      return {
        title: '',
        content: ''
      }
    },
    ready: function() {
      var _this = this,
        id = getId(),
        maybeStaticpage = stash.staticpages.filter(function(elem) {
          return elem.id === id;
        });

      if (maybeStaticpage.length > 0) { //in cache
        this.title = maybeStaticpage[0].title;
        this.content = maybeStaticpage[0].content;

        headerTitle.setTitle(this.title);
        pgTitle.title = this.title;
      } else {
        ajax({
          url: '/staticpage/' + id,
          method: 'GET'
        }, function(code, responseText, request) {
          if (code === 200) {
            var resp = JSON.parse(responseText);

            stash.staticpages.push(resp);

            _this.title = resp.title;
            _this.content = resp.content;

            headerTitle.setTitle(resp.title);
            pgTitle.title = resp.title;

          } else {
            //location.hash = '/';
          }
        });
      }

    }
  });

  Vue.component('twilightimperium', {
    template: '#ti-tpl',
    data: function() {
      return {
        slots: [],
        results: []
      }
    },
    ready: function(){
      for (var i = 0; i < 8; i++){
        this.slots.push(i);
      }
    },
    methods: {
      submit: function() {
        var _this = this;
        var players = Array.prototype.slice.call(document.getElementsByClassName("ti-player")).map(function(elem){
          return elem.value;
        }).filter(function(name){
          return name != "";
        }).join("|");

        if(players !== '') {
          ajax({
            url: '/ti',
            method: 'POST',
            body: 'players=' + encodeURIComponent(players)
          }, function(code, responseText, request) {
            if (code === 200) {
              var resp = JSON.parse(responseText);
              _this.results = resp;

            } else {
              //location.hash = '/';
            }
          });
        }
      }
    }
  })

  var app = new Vue({
    el: '#app',
    data: {
      currentView: ''

    }
  });

  var setView = function(hash) {
    var path = hash.split('/').slice(1, 2).join(); //0,1 would be the #
    if (path === 'post') {
      app.currentView = 'post';
    } else if (path === 'staticpage') {
      app.currentView = 'staticpage';
    } else if (path === 'ti') {
      app.currentView = 'twilightimperium';
    } else {
      app.currentView = 'posts';
    }

  }

  global.addEventListener('hashchange', function() {
    setView(location.hash);
  });

  setView(location.hash);

}(window))
