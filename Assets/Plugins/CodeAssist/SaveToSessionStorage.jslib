mergeInto(LibraryManager.library, {
  Set: function (object) {
    localStorage.setItem("test", new object());
  },

  Get: function () {
    localStorage.getItem("test");
  },

  Hello: function () {
    window.alert("Hello, world!");
  },
});
