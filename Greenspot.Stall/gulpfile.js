/// <binding />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    less = require("gulp-less"),
    uglify = require("gulp-uglify");



var contentPath = "./Content/";
var staticPath = "./Static/";

var lessGroups = [{ src: "less/*.less", dest: "middle-css" }];
var jsGroups = [{ src: "middle-js/**/*.js", exclude: "middle-js/*/*.min.js", dest: "js/stall.min.js" },
                { src: "semantic-components/**/*.js", exclude: "", dest: "js/semantic-components.min.js" }];

var cssGroups = [{ src: "middle-css/*.css", exclude: "middle-css/*.min.css", dest: "css/stall.min.css" },
                  { src: "semantic-components/*.css", exclude: "", dest: "css/semantic-components.min.css" }];

/*clean*/
gulp.task("clean:css", function (cb) {
    rimraf(staticPath + "css/*", cb);
});
gulp.task("clean:dist", function (cb) {
    rimraf(staticPath + "js/*", cb);
});
gulp.task("clean", ["clean:css", "clean:dist"]);


/*less*/
gulp.task("less", function () {
    for (var i = 0; i < lessGroups.length; i++) {
        gulp.src(contentPath + lessGroups[i].src)
                .pipe(less())
                .pipe(gulp.dest(contentPath + lessGroups[i].dest));
    }
});

/*minify*/
gulp.task("min:js", function () {
    for (var i = 0; i < jsGroups.length; i++) {
        gulp.src([contentPath + jsGroups[i].src, "!" + contentPath + jsGroups[i].exclude])
                .pipe(concat(staticPath + jsGroups[i].dest))
                .pipe(uglify())
                .pipe(gulp.dest("."));
    }
});

gulp.task("min:css", function () {
    for (var i = 0; i < cssGroups.length; i++) {
        gulp.src([contentPath + cssGroups[i].src, "!" + contentPath + cssGroups[i].exclude])
                .pipe(concat(staticPath + cssGroups[i].dest))
                .pipe(cssmin())
                .pipe(gulp.dest("."));
    }
});

gulp.task("min", ["min:js", "min:css"]);